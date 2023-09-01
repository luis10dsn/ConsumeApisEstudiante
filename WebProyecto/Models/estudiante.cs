using ProyectoPrograV;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class estudiante
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

        
         public string CorreoEle { get; set; }

        
        public string NumerosTelefono { get; set; }


        public bool Validarepetidos(string[] Lista) {

            bool repetidos = false;

            for (var x = 0; x < Lista.Length; x++) {
                string a = Lista[x];
                int c = x + 1;
                for (int y = c; y < Lista.Length; y++) { 
                   string b = Lista[y];
                    if (a.Equals(b)) {
                        repetidos = true;
                    }
                }
            }

            return repetidos;
        }

    }

  


        
}