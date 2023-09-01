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
    public class Correos_ProfesoresController : ApiController
    {
        private tiusr5pl_Proyecto1PrograVEntities db = new tiusr5pl_Proyecto1PrograVEntities();

        // GET: api/Correos_Profesores
        public IQueryable<Correos_Profesores> GetCorreos_Profesores()
        {
            return db.Correos_Profesores;
        }

        // GET: api/Correos_Profesores/5
        [ResponseType(typeof(Correos_Profesores))]
        public async Task<IHttpActionResult> GetCorreos_Profesores(string correo, string tipoID, string id)
        {
            Correos_Profesores correos_Profesores = await db.Correos_Profesores.FindAsync(correo,tipoID,id);
            if (correos_Profesores == null)
            {
                return NotFound();
            }

            return Ok(correos_Profesores);
        }

        // PUT: api/Correos_Profesores/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCorreos_Profesores(string id, Correos_Profesores correos_Profesores)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != correos_Profesores.Corre_Electronico)
            {
                return BadRequest();
            }

            db.Entry(correos_Profesores).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Correos_ProfesoresExists(id))
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

       

        // DELETE: api/Correos_Profesores/5
        [ResponseType(typeof(Correos_Profesores))]
        public async Task<IHttpActionResult> DeleteCorreos_Profesores(string id)
        {
            Correos_Profesores correos_Profesores = await db.Correos_Profesores.FindAsync(id);
            if (correos_Profesores == null)
            {
                return NotFound();
            }

            db.Correos_Profesores.Remove(correos_Profesores);
            await db.SaveChangesAsync();

            return Ok(correos_Profesores);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Correos_ProfesoresExists(string id)
        {
            return db.Correos_Profesores.Count(e => e.Corre_Electronico == id) > 0;
        }
    }
}