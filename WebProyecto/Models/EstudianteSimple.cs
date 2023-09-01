using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class EstudianteSimple
    {
        [Required]
        [MaxLength(25)]
        public string tipo_ID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Identificacion { get; set; }

        [Required]
        [MaxLength(20)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(20)]
        public string primerApellido { get; set; }

        [Required]
        [MaxLength(20)]
        public string SegundoApellido { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }
    }
}