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
    public class AsistenciasController : ApiController
    {
        private tiusr5pl_Proyecto1PrograVEntities db = new tiusr5pl_Proyecto1PrograVEntities();

        // GET: api/Asistencias
        public IQueryable<Asistencia> GetAsistencias()
        {
            return db.Asistencias;
        }

        

        [ResponseType(typeof(Asistencia))]
        [Route("api/Asistencia/AsistenciaPorGrupo", Name = "getAsistenciaPorGrupo")]

        public HttpResponseMessage getAsistencia(int NumeroGrupo, string CodigoCurso, DateTime Fecha)
        {   //obtiene tipoID segun apellidos Profesor
            try
            {

                var idQuery = (from p in db.Asistencias
                               where p.Numero_Grupo == NumeroGrupo && p.Codigo_Curso == CodigoCurso && p.Fecha_Asistencia==Fecha

                               select new
                               {
                                   p.Numero_Grupo,
                                   p.Codigo_Curso,
                                   p.Fecha_Asistencia,
                                   p.Tipo_Registro,
                                   p.Tipo_ID_Esutiante,
                                   p.Identificacion_Estudiante
                               }
                               ).ToList();
                Asistencia asistencia = db.Asistencias.Where(x => x.Numero_Grupo == NumeroGrupo && x.Codigo_Curso == CodigoCurso && x.Fecha_Asistencia==Fecha).SingleOrDefault();

                if (asistencia != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, idQuery);
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Asistencia no ha sido encontrado");

            }
            catch (Exception ex)
            {

                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }


        }

        // PUT: api/Asistencias/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAsistencia(byte id, Asistencia asistencia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != asistencia.Numero_Grupo)
            {
                return BadRequest();
            }

            db.Entry(asistencia).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AsistenciaExists(id))
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

        // POST: api/Asistencias
        [ResponseType(typeof(Asistencia))]
        public async Task<IHttpActionResult> PostAsistencia(Asistencia asistencia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Asistencias.Add(asistencia);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AsistenciaExists(asistencia.Numero_Grupo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = asistencia.Numero_Grupo }, asistencia);
        }

        // DELETE: api/Asistencias/5
        [ResponseType(typeof(Asistencia))]
        public async Task<IHttpActionResult> DeleteAsistencia(byte CodigoGrupo, string CodCurso, string fecha, string tipoID, string ID)
        {
            Asistencia asistencia = await db.Asistencias.FindAsync(CodigoGrupo, CodCurso, fecha, tipoID, ID);
            if (asistencia == null)
            {
                return NotFound();
            }

            db.Asistencias.Remove(asistencia);
            await db.SaveChangesAsync();

            return Ok(asistencia);
        }

        [HttpPost]
        [Route("api/Asistencias/CrearAsistencia")]
        [ResponseType(typeof(asistencia))]
        public async Task<IHttpActionResult> CrearAsistencia
 ([FromBody] asistencia a)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Validar campos foraneos

            // valida que el estudiante exista
            if (!EstudianteExists(a.tipo_ID) || !EstudianteExists2(a.Identificacion))
            {
                return NotFound();
            }


            //Validar que el grupo y curso exista
            if (!GruposExists(a.numerogrupo) || !GruposExistsCodigoCurso(a.codigocurso))
            {
                return NotFound();
            }

            //Validar que el estudiante este matriculado
            bool bandera = ConsultaMatricula(a.Identificacion, a.tipo_ID, a.codigocurso);
            if (bandera == false)
            {
                return BadRequest("El estudiante no esta matriculado en el curso");
            }

            Asistencia A1 = new Asistencia()
            {
                Codigo_Curso = a.codigocurso,
                Numero_Grupo = a.numerogrupo,
                Fecha_Asistencia = a.fechaAsistencia,
                Tipo_Registro = a.tipoasistencia,
                Identificacion_Estudiante = a.Identificacion,
                Tipo_ID_Esutiante = a.tipo_ID
            };
            db.Asistencias.Add(A1);

            try
            {
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { Controller = "Asistencia" }, a);
            }
            catch (DbUpdateException)
            {
                if (AsistenciaExists(a.numerogrupo) & AsistenciaExistsCurso(a.codigocurso)
                   & AsistenciaExistsFecha(a.fechaAsistencia) & AsistenciaExistEstudiante(a.tipo_ID)
                   & AsistenciaExistEstudianteID(a.Identificacion))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }



        }




        //--------------------------------DatosASistencia


        [Route("api/Asistencia/AsistenciaDesgloce")]
        [HttpGet]
        public async Task<IHttpActionResult> getDatosAsistencia(string id, string TipoID)
        {
           Estudiante estudiante = await db.Estudiantes.FindAsync(TipoID, id);

            if (estudiante == null)
            {
                return NotFound();
            }

            var idQuery =
           from ord1 in db.Estudiantes
           from ord in db.Asistencias
           
           where TipoID== ord.Tipo_ID_Esutiante &&id == ord1.Identificacion && ord.Identificacion_Estudiante == ord1.Identificacion && ord.Tipo_ID_Esutiante == ord1.Tipo_ID
           select new { ord.Tipo_ID_Esutiante, ord.Identificacion_Estudiante, ord1.Nombre, ord1.Primer_Apellido, ord1.Segundo_apellido, ord.Codigo_Curso,ord.Numero_Grupo,ord.Fecha_Asistencia,ord.Tipo_Registro };



            return Ok(idQuery);
        }


        //------------------------

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AsistenciaExists(byte id)
        {
            return db.Asistencias.Count(e => e.Numero_Grupo == id) > 0;
        }

        private bool AsistenciaExistsCurso(string id)
        {
            return db.Asistencias.Count(e => e.Codigo_Curso == id) > 0;
        }

        private bool AsistenciaExistsFecha(DateTime fecha)
        {
            return db.Asistencias.Count(e => e.Fecha_Asistencia == fecha) > 0;
        }

        private bool AsistenciaExistEstudiante(string TipoId)
        {
            return db.Asistencias.Count(e => e.Tipo_ID_Esutiante == TipoId) > 0;
        }
        private bool AsistenciaExistEstudianteID(string Id)
        {
            return db.Asistencias.Count(e => e.Identificacion_Estudiante == Id) > 0;
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

        private bool ConsultaMatricula(string identificacion, string tipoid, string codigocurso)
        {

            if (db.Matriculas.Count(e => e.Tipo_ID_Estudiante == tipoid) > 0
                & db.Matriculas.Count(e => e.Identificacion_Estudiante == identificacion) > 0
                & db.Matriculas.Count(e => e.Codigo_Curso == codigocurso) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}