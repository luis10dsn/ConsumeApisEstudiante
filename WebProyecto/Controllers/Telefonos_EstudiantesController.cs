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
    public class Telefonos_EstudiantesController : ApiController
    {
        private tiusr5pl_Proyecto1PrograVEntities db = new tiusr5pl_Proyecto1PrograVEntities();

        // GET: api/Telefonos_Estudiantes
        public IQueryable<Telefonos_Estudiantes> GetTelefonos_Estudiantes()
        {
            return db.Telefonos_Estudiantes;
        }

        // GET: api/Telefonos_Estudiantes/5
        [ResponseType(typeof(Telefonos_Estudiantes))]
        public async Task<IHttpActionResult> GetTelefonos_Estudiantes(string telefono, string TipoID, string ID)
        {
            Telefonos_Estudiantes telefonos_Estudiantes = await db.Telefonos_Estudiantes.FindAsync(telefono,TipoID,ID);
            if (telefonos_Estudiantes == null)
            {
                return NotFound();
            }

            return Ok(telefonos_Estudiantes);
        }

        // PUT: api/Telefonos_Estudiantes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTelefonos_Estudiantes(string id, Telefonos_Estudiantes telefonos_Estudiantes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != telefonos_Estudiantes.Numero_Telefono)
            {
                return BadRequest();
            }

            db.Entry(telefonos_Estudiantes).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Telefonos_EstudiantesExists(id))
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

       
        // DELETE: api/Telefonos_Estudiantes/5
        [ResponseType(typeof(Telefonos_Estudiantes))]
        public async Task<IHttpActionResult> DeleteTelefonos_Estudiantes(string telefono, string TipoID, string ID)
        {
            Telefonos_Estudiantes telefonos_Estudiantes = await db.Telefonos_Estudiantes.FindAsync(telefono,TipoID,ID);
            if (telefonos_Estudiantes == null)
            {
                return NotFound();
            }

            db.Telefonos_Estudiantes.Remove(telefonos_Estudiantes);
            await db.SaveChangesAsync();

            return Ok(telefonos_Estudiantes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Telefonos_EstudiantesExists(string id)
        {
            return db.Telefonos_Estudiantes.Count(e => e.Numero_Telefono == id) > 0;
        }
    }
}