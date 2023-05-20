using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TareasMVC;
using TareasMVC.Entidades;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/pasos")]
    public class PasosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServicioUsuarios servicioUsuarios;

        public PasosController(ApplicationDbContext context,
            IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            this.servicioUsuarios = servicioUsuarios;
        }

        // POST: Pasos
        [HttpPost("{tareaId: int}")]
        public async Task<ActionResult<Paso>> Post(int tareaId, [FromBody] PasoCrearDTO pasoCrearDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaId);

            if (tarea is null)
            {
                return NotFound();
            }

            if (tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            var existenPasos = await _context.Pasos.AnyAsync(p => p.TareaId == tareaId);
            var ordenMayor = 0;
            if (existenPasos)
            {
                ordenMayor = await _context.Pasos
                                        .Where(p => p.TareaId == tareaId)
                                        .Select(p => p.Orden)
                                        .MaxAsync();
            }
            var paso = new Paso();
            paso.TareaId = tareaId;
            paso.Orden = ordenMayor + 1;
            paso.Descripcion = pasoCrearDTO.descripcion;
            paso.Realizado = pasoCrearDTO.Realizado;

            _context.Add(paso);
            await _context.SaveChangesAsync();
            return paso;
        }
    }
}