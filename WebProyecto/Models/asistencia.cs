using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class asistencia
    {

        [Required]

        [MaxLength(25)]
        public string tipo_ID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Identificacion { get; set; }

        [Required]
        [MaxLength(10)]
        public string codigocurso { get; set; }


        [Required]
        public byte numerogrupo { get; set; }

        [Required]
        public DateTime fechaAsistencia { get; set; }

        [Required]
        [MaxLength(25)]
        public string tipoasistencia { get; set; }



    }
}