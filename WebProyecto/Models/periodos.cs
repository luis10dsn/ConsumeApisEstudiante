using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class periodos
    {
        [Required]
        public int anno { get; set; }

        [Required]
        public byte numeroPeriodo { get; set; }


        [Required]
        public DateTime fechaInicio { get; set; }


        [Required]
        public DateTime fechafinal { get; set; }

        [Required]
        [MaxLengthAttribute(2)]
        public string estado { get; set; }

    }
}