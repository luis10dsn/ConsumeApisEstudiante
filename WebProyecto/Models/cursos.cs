using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class cursos
    {
        [Required]
        [MaxLength(10)]
        public string Codigo_Curso { get; set; }

        [Required]
        [MaxLength(30)]
        public string Nombre_Curso { get; set; }


        [Required]
        [MaxLength(10)]
        public string Codigo_Carrera { get; set; }

    }
}