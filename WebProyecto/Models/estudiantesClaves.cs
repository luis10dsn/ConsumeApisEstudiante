using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class estudiantesClaves
    {
        [Required]
        [MaxLength(25)]
        public string tipo_ID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Identificacion { get; set; }
    }
}