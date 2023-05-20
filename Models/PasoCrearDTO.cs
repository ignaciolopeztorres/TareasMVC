using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Models
{
    public class PasoCrearDTO
    {
        [Required]
        public string descripcion { get; set; }

        public bool Realizado { get; set; }
    }
}