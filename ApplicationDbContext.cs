using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;

namespace TareasMVC
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        //configura que la clase tareas sea una entidad en la base de datos
        public DbSet<Tarea> Tareas { get; set; }
    }
}