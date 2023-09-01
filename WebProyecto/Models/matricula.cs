using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class matricula
    {
        public matricula()
        {
        
        }

        [Required]

        [MaxLength(25)]
        public string tipo_ID_Estudiante { get; set; }

        [Required]
        [MaxLength(30)]
        public string Identificacion_Estudiante { get; set; }

        [Required]
        [MaxLength(10)]
        public string codigocurso { get; set; }

        [Required]
        public byte numerogrupo { get; set; }

        [Required]
        [MaxLength(15)]
        public string tipomatricula { get; set; }

        
        public double nota { get; set; }




    }
}