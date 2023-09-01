using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class profesorConsulta
    {
    
        public string tipo_ID { get; set; }


        public string Identificacion { get; set; }

        public string Nombre { get; set; }

        
        public string primer_Apellido { get; set; }

      
        public string segundo_apellido { get; set; }

  
        public DateTime fecha_Nacimiento { get; set; }

    }
}