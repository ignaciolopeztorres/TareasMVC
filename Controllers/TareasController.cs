using Microsoft.AspNetCore.Mvc;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TareasController(ApplicationDbContext context,
            IServicioUsuarios servicioUsuarios)
        {
            this.context = context;
        }
    }
}