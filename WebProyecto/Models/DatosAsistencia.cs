using ProyectoPrograV;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProyecto.Models
{
    public class DatosAsistencia
    {
        [Required]

        [MaxLength(25)]
        public string tipo_ID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Identificacion { get; set; }

      public List<Telefonos_Estudiantes> Telefonos_Estudiantes { get; set; }
      public HashSet<Correos_Estudiantes> Correos_Estudiantes { get; set; }

      public HashSet<Asistencia> Asistencias { get; set; }
            
      public HashSet<Matricula> Matriculas { get; set; }
        
    }


}