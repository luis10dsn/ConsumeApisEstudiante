using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ProyectoPrograV;

namespace WebProyecto.Controllers
{
    public class Correos_EstudiantesController : ApiController
    {
        private tiusr5pl_Proyecto1PrograVEntities db = new tiusr5pl_Proyecto1PrograVEntities();

        // GET: api/Correos_Estudiantes
        public IQueryable<Correos_Estudiantes> GetCorreos_Estudiantes()
        {
            return db.Correos_Estudiantes;
        }

        // GET: api/Correos_Estudiantes/5
        [ResponseType(typeof(Correos_Estudiantes))]
        public async Task<IHttpActionResult> GetCorreos_Estudiantes(string correo, string tipoID, string id)
        {
            Correos_Estudiantes correos_Estudiantes = await db.Correos_Estudiantes.FindAsync(correo,tipoID,id);
            if (correos_Estudiantes == null)
            {
                return NotFound();
            }

            return Ok(correos_Estudiantes);
        }

        // PUT: api/Correos_Estudiantes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCorreos_Estudiantes(string id, Correos_Estudiantes correos_Estudiantes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != correos_Estudiantes.Corre_Electronico)
            {
                return BadRequest();
            }

            db.Entry(correos_Estudiantes).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Correos_EstudiantesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        
        // DELETE: api/Correos_Estudiantes/5
        [ResponseType(typeof(Correos_Estudiantes))]
        public async Task<IHttpActionResult> DeleteCorreos_Estudiantes(string id)
        {
            Correos_Estudiantes correos_Estudiantes = await db.Correos_Estudiantes.FindAsync(id);
            if (correos_Estudiantes == null)
            {
                return NotFound();
            }

            db.Correos_Estudiantes.Remove(correos_Estudiantes);
            await db.SaveChangesAsync();

            return Ok(correos_Estudiantes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Correos_EstudiantesExists(string id)
        {
            return db.Correos_Estudiantes.Count(e => e.Corre_Electronico == id) > 0;
        }
    }
}