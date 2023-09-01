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
using WebProyecto.Models;

namespace WebProyecto.Controllers
{
    public class MatriculasController : ApiController
    {
        private tiusr5pl_Proyecto1PrograVEntities db = new tiusr5pl_Proyecto1PrograVEntities();

        // GET: api/Matriculas
        public IQueryable<Matricula> GetMatriculas()
        {
            return db.Matriculas;
        }

        // GET: api/Matriculas/5
        [ResponseType(typeof(Matricula))]
        public async Task<IHttpActionResult> GetMatricula(string TipoID,string id, string codigoCurso)
        {
            Matricula matricula = await db.Matriculas.FindAsync(TipoID,id,codigoCurso);
            if (matricula == null)
            {
                return NotFound();
            }

            return Ok(matricula);
        }

        // PUT: api/Matriculas/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMatricula(string id, Matricula matricula)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != matricula.Tipo_ID_Estudiante)
            {
                return BadRequest();
            }

            db.Entry(matricula).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatriculaExists(id))
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

        // POST: api/Matriculas
        [ResponseType(typeof(Matricula))]
        public async Task<IHttpActionResult> PostMatricula(Matricula matricula)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Matriculas.Add(matricula);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MatriculaExists(matricula.Tipo_ID_Estudiante))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = matricula.Tipo_ID_Estudiante }, matricula);
        }



        [HttpPost]
        [Route("api/Matriculas/CrearMatricula")]
        [ResponseType(typeof(matricula))]
        public async Task<IHttpActionResult> CrearMatricula
      ([FromBody] matricula m)

        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Validamos llaves foraneas

            //Valida estudiante exista
            if (!EstudianteExists(m.tipo_ID_Estudiante) || !EstudianteExists2(m.Identificacion_Estudiante))
            {
                return NotFound();
            }

            //Validar que el grupo exista
            if (!GruposExists(m.numerogrupo) || !GruposExistsCodigoCurso(m.codigocurso))
            {
                return NotFound();
            }

            //Validar que el grupo pertenezca a un periodo activo
           bool estadoperiodo=  ConsultaEstadoPeriodo(m.numerogrupo, m.codigocurso);
            if (estadoperiodo == false)
            {
                return BadRequest("El grupo que intenta matricular pertenece a un periodo no valido");
            }

            Matricula M1 = new Matricula()
            {
               Numero_Grupo = m.numerogrupo,
               Tipo_Matricula = m.tipomatricula,
               Tipo_ID_Estudiante = m.tipo_ID_Estudiante,
               Identificacion_Estudiante = m.Identificacion_Estudiante,
               Nota = m.nota,
               Codigo_Curso = m.codigocurso
            };
            db.Matriculas.Add(M1);


            try
            {
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { Controller = "Matricula" }, m);



            }
            catch (DbUpdateException)
            {
                if (MatriculaExists(m.tipo_ID_Estudiante) & MatriculaExistsID(m.Identificacion_Estudiante) & MatriculaExistCodigoCurso(m.codigocurso))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

        }




        // DELETE: api/Matriculas/5
        [ResponseType(typeof(Matricula))]
        public async Task<IHttpActionResult> DeleteMatricula(string TipoID, string id, string codigoCurso)
        {
            Matricula matricula = await db.Matriculas.FindAsync(TipoID,id,codigoCurso);
            if (matricula == null)
            {
                return NotFound();
            }

            db.Matriculas.Remove(matricula);
            await db.SaveChangesAsync();

            return Ok(matricula);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MatriculaExists(string id)
        {
            return db.Matriculas.Count(e => e.Tipo_ID_Estudiante == id) > 0;
        }

        private bool MatriculaExistsID(string id)
        {
            return db.Matriculas.Count(e => e.Identificacion_Estudiante == id) > 0;
        }

        private bool MatriculaExistCodigoCurso(string id)
        {
            return db.Matriculas.Count(e => e.Codigo_Curso == id) > 0;
        }

        private bool EstudianteExists(string tipoid)
        {
            return db.Estudiantes.Count(e => e.Tipo_ID == tipoid) > 0;
        }

        private bool EstudianteExists2(string identificacion)
        {
            return db.Estudiantes.Count(e => e.Identificacion == identificacion) > 0;
        }

        private bool GruposExists(byte id)
        {
            return db.Grupos.Count(e => e.Numero_Grupo == id) > 0;
        }

        private bool GruposExistsCodigoCurso(string id)
        {
            return db.Grupos.Count(e => e.Codigo_Curs == id) > 0;
        }

        private bool ConsultaEstadoPeriodo(byte numerogrupo, string codigocurso)
     
        {
            bool bandera = false;
            var estado =
             from G in db.Grupos
             join P in db.Periodoes 
             on new {G.NumeroPeriodo, G.Anno} equals new {P.NumeroPeriodo, P.Anno} 
             where G.Numero_Grupo == numerogrupo && G.Codigo_Curs == codigocurso
             select P.Estado;

            string estad = "";
            foreach (var item in estado)
            {
                estad = string.Concat(item);
            }
            if (estad.Contains("A"))
            {
                bandera = true;
            }
            return bandera;
        
        }

    }
}