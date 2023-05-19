﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;

        public TareasController(ApplicationDbContext context,
            IServicioUsuarios servicioUsuarios)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tarea = await context.Tareas
                .Where(t => t.UsuarioCreacionId == usuarioId)
                .OrderBy(t => t.Orden)
                .Select(t => new { 
                    t.Id,
                    t.Titulo
                })
                .ToListAsync();
            return Ok(tarea);
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var existeTareas = await context.Tareas.AnyAsync(t => t.UsuarioCreacionId == usuarioId);
            var ordenMayor = 0;

            if (existeTareas)
            {
                //obtien eel orden con la base de datos
                ordenMayor = await context.Tareas
                    .Where(t => t.UsuarioCreacionId == usuarioId)
                    .Select(t => t.Orden)
                    .MaxAsync();
            }

            //crea una tarea
            var tarea = new Tarea
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Orden = ordenMayor + 1
            };

            //marca para agregar a la base de datos
            context.Add(tarea);
            //guarda en la base de datos
            await context.SaveChangesAsync();
            return tarea;
        }
    }
}