﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;

namespace TareasMVC
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        //configuracion del api fluente
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Tarea>()
            //  .Property(x => x.Titulo)
            //  .HasMaxLength(250)
            //  .IsRequired();
        }

        //configura que la clase tareas sea una entidad en la base de datos
        public DbSet<Tarea> Tareas { get; set; }

        public DbSet<Paso> Pasos { get; set; }

        public DbSet<ArchivoAdjunto> ArchivosAdjuntos { get; set; }
    }
}