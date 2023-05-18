namespace TareasMVC.Entidades
{
    public class Paso
    {
        public Guid Id { get; set; }
        public int TareaId { get; set; }

        // propiedadd de navegacion que permite navegar hacia una entidad
        public Tarea Tarea { get; set; }

        public string Descripcion { get; set; }
        public bool Realizado { get; set; }
        public int Orden { get; set; }
    }
}