using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Dtos
{
    public class MovieDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; }
        public byte[] Image { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Description { get; set; }
        [Required(ErrorMessage = "La duración es obligatoria")]
        public int Duration { get; set; }
        public enum ClasificationType { Siete, Trece, Dieciseis, Diciocho }
        public ClasificationType Clasification { get; set; }
        public DateTime CreationOn { get; set; }
        public int CategoryId { get; set; }
    }
}
