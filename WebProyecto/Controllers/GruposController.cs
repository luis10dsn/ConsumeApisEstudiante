using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using ProyectoPrograV;
using WebProyecto.Models;

namespace WebProyecto.Controllers
{
    public class GruposController : ApiController
    {
        private tiusr5pl_Proyecto1PrograVEntities db = new tiusr5pl_Proyecto1PrograVEntities();

        // GET: api/Grupos
        public IQueryable<Grupos> GetGrupos()
        {
            return db.Grupos;
        }

        [Route("api/Grupos/ListaGrupos")]
        [HttpGet]
        public IHttpActionResult getgrupos()
        {
            try
            {
                // db.Periodoes.OrderByDescending(periodo => periodo.Fecha_Inicio);

                //orderby ord1.Fecha_Inicio descending
                var idQuery = from grupos in db.Grupos
                              from curso in db.Cursos
                              from carrera in db.Carreras
                              from profesor in db.Profesores
                              from periodo in db.Periodoes
                              orderby grupos.Anno ascending
                              orderby grupos.NumeroPeriodo ascending
                              where grupos.Tipo_ID_Profeso == profesor.Tipo_ID && grupos.Identificacion_Profesor == profesor.Identificacion
                              && grupos.Codigo_Curs == curso.Codigo_Curso && curso.Codigo_Carrera == carrera.Codigo_Carrera
                              && grupos.Anno == periodo.Anno && grupos.NumeroPeriodo == periodo.NumeroPeriodo
                              // && periodo.Estado == "A" || periodo.Estado == "F" 
                              select new
                              {
                                  grupos.Numero_Grupo,
                                  grupos.Codigo_Curs,
                                  curso.Nombre_Curso,
                                  carrera.Nombre_Carrera,
                                  periodo.Anno,
                                  periodo.NumeroPeriodo,
                                  grupos.Horario,
                                  profesor.Nombre,
                                  profesor.Primer_Apellido,
                                  profesor.Tipo_ID,
                                  profesor.Identificacion,
                              };

                if (idQuery.Count() > 0)
                {
                    return Ok(idQuery);
                }
                else
                {
                    return NotFound();
                }


            }
            catch (Exception)
            {

                throw;
            }


        }




        // GET: api/Grupos/5
        [ResponseType(typeof(Grupos))]
        public async Task<IHttpActionResult> GetGrupos(byte id,string Codigo )
        {
            Grupos grupos = await db.Grupos.FindAsync(id,Codigo);
            if (grupos == null)
            {
                return NotFound();
            }

            return Ok(grupos);
        }

        // PUT: api/Grupos/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGrupos(grupos G)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!PeriodoExists(G.anno) || !PeriodoExistsNuPeriodo(G.numeroPeriodo))
            {
                return NotFound();
            }


            Grupos e2 = new Grupos()
            {
              Numero_Grupo = G.numerogrupo,
              Codigo_Curs = G.codigocurso,
              Identificacion_Profesor = G.Identificacion,
              Horario = G.Horario,
              Anno = G.anno,
              NumeroPeriodo = G.numeroPeriodo,
              Tipo_ID_Profeso = G.tipo_ID

            };

            db.Entry(e2).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
                return Ok(e2);
            }

            catch (DbUpdateConcurrencyException)
            {

                throw;
            }
        }

        // POST: api/Grupos
        [ResponseType(typeof(Grupos))]
        public async Task<IHttpActionResult> PostGrupos(Grupos grupos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Grupos.Add(grupos);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GruposExists(grupos.Numero_Grupo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = grupos.Numero_Grupo }, grupos);
        }


        [HttpPost]
        [Route("api/Grupos/CrearGrupo")]
        [ResponseType(typeof(grupos))]
        public async Task<IHttpActionResult> CrearGrupo
        ([FromBody] grupos g)

        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

     

            //Validamos que los campos foranenos en la tabla existan

            //Validar que el curso exista
            if (!CursoExists(g.codigocurso))
            {
                return NotFound();
            }

            /// Validamos que el profesor exista
            if (!ProfesoreExists(g.tipo_ID) || !ProfesoreExists2(g.Identificacion))
            {
                return BadRequest("El profesor no existe");
            }

            //Validar que el periodo exista
            if (!PeriodoExists(g.anno) || !PeriodoExistsNuPeriodo(g.numeroPeriodo))
            {
                return BadRequest("El periodo no existe");
            }
            //Validar que el periodo sea activo o futuro

            string Estadoperiodo = ConsultaEstadoPeriodo(g.anno, g.numeroPeriodo);

            if (Estadoperiodo.Contains('P'))
            {
                return BadRequest("El periodo no puede ser un periodo pasado");
            }

            Grupos G1 = new Grupos()
            {
                NumeroPeriodo = g.numeroPeriodo,
                Numero_Grupo = g.numerogrupo,
                Codigo_Curs = g.codigocurso,
                Identificacion_Profesor = g.Identificacion,
                Horario = g.Horario,
                Anno = g.anno,
               Tipo_ID_Profeso = g.tipo_ID
            };

            db.Grupos.Add(G1);
            try
            {
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { Controller = "Grupos"}, g);
            }
            catch (DbUpdateException)
            {
                if (GruposExists(g.numerogrupo) & GruposExistsCodigoCurso(g.codigocurso))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

          

        }




            // DELETE: api/Grupos/5
            [ResponseType(typeof(Grupos))]
        public async Task<IHttpActionResult> DeleteGrupos(string id)
        {
            string[] llaves = id.Split('_');
            int numerogrupo = int.Parse(llaves[0]);
            string codigocurso = llaves[1];

            Grupos G = await db.Grupos.FindAsync(numerogrupo,codigocurso);
            if (G == null)
            {
                return NotFound();
            }

            db.Grupos.Remove(G);
            await db.SaveChangesAsync();

            return Ok(G);
        }
        //--------------------------------


        [Route("api/Grupos/DatosGrupo")]
        [HttpGet]
        public async Task<IHttpActionResult> getDatosGrupo(int NumGrupo, string CodCurso)
        {
            Grupos grupos = await db.Grupos .FindAsync(NumGrupo, CodCurso);

            if (grupos == null)
            {
                return NotFound();
            }

            var idQuery =
           from ord1 in db.Cursos
           from ord in db.Grupos
           from ord2 in db.Profesores
           where NumGrupo==ord.Numero_Grupo&& CodCurso==ord.Codigo_Curs&& ord.Identificacion_Profesor == ord2.Identificacion && ord.Tipo_ID_Profeso == ord2.Tipo_ID && ord.Codigo_Curs==ord1.Codigo_Curso 
           select new { ord.Numero_Grupo,ord1.Codigo_Curso,ord1.Nombre_Curso,ord.Periodo,ord.Horario,ord2.Nombre,ord2.Primer_Apellido,ord2.Segundo_apellido};



            return Ok(idQuery);
        }

        //------------------------- GetGruposPorProfesor

        [Route("api/Grupos/GetGruposPorProfesor")]
        [HttpGet]
        public async Task<IHttpActionResult> getGruposPorProfesor(string TipoID,String Id)
        {
            Profesore prof = await db.Profesores.FindAsync(TipoID,Id);

            if (prof == null)
            {
                return NotFound();
            }

            var idQuery =
           from ord1 in db.Grupos
           from ord in db.Profesores
           from ord2 in db.Cursos
           where TipoID == ord.Tipo_ID && 
           Id == ord.Identificacion && TipoID==ord1.Tipo_ID_Profeso && 
           Id==ord1.Identificacion_Profesor &&ord1.Codigo_Curs==ord2.Codigo_Curso
           select new { ord1.Numero_Grupo, ord2.Codigo_Curso, ord2.Nombre_Curso,
               ord1.NumeroPeriodo,ord1.Horario,ord.Nombre,ord.Primer_Apellido,
               ord.Segundo_apellido,ord.Tipo_ID,ord.Identificacion};


            return Ok(idQuery);
        }

        //------------------------------

        //------------------------- GetGruposPorCarrera

        [Route("api/Grupos/GetGruposPorCarrera")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGruposPorCarrera(string CodCarrera)
        {
            Carrera carrera = await db.Carreras.FindAsync(CodCarrera);

            if (carrera == null)
            {
                return NotFound();
            }

            var idQuery =
           from ord1 in db.Grupos
           from ord in db.Carreras
           from ord2 in db.Cursos
           from ord3 in db.Profesores
           where CodCarrera == ord.Codigo_Carrera && CodCarrera ==ord2.Codigo_Carrera &&
           ord2.Codigo_Curso==ord1.Codigo_Curs
           select new
           {
               ord1.Numero_Grupo,
               ord2.Codigo_Curso,
               ord2.Nombre_Curso,
               ord1.NumeroPeriodo,
               ord1.Horario,
               ord3.Nombre,
               ord3.Primer_Apellido,
               ord3.Segundo_apellido,
               ord3.Tipo_ID,
               ord3.Identificacion
           };


            return Ok(idQuery);
        }

        //------------------------------
        //------------------------- GetGruposPorPeriodo

        [Route("api/Grupos/GetGruposPorPeriodo")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGruposPorPeriodo(int Anno,int Periodo)
        {
            Periodo periodo = await db.Periodoes.FindAsync(Anno,Periodo);

            if (periodo == null)
            {
                return NotFound();
            }

            var idQuery =
           from ord1 in db.Grupos
           from ord in db.Periodoes
           from ord2 in db.Cursos
           from ord3 in db.Profesores
           where Anno == ord.Anno && Periodo == ord.NumeroPeriodo &&
           Anno==ord1.Anno && Periodo == ord1.NumeroPeriodo
           
           select new
           {
               ord1.Numero_Grupo,
               ord2.Codigo_Curso,
               ord2.Nombre_Curso,
               ord1.Horario,
               ord3.Nombre,
               ord3.Primer_Apellido,
               ord3.Segundo_apellido,
               ord3.Tipo_ID,
               ord3.Identificacion,
               ord.Anno,
               ord1.NumeroPeriodo

           };


            return Ok(idQuery);
        }

        //------------------------------


        //------------------------- GetGruposPorCurso

        [Route("api/Grupos/GetGruposPorCurso")]
        [HttpGet]
        public async Task<IHttpActionResult> GetGruposPorCurso(string CodCurso)
        {
            Curso curso = await db.Cursos.FindAsync(CodCurso);

            if (curso == null)
            {
                return NotFound();
            }

            var idQuery =
           from ord1 in db.Grupos
           from ord in db.Carreras
           from ord2 in db.Cursos
           from ord3 in db.Profesores
           where CodCurso == ord2.Codigo_Curso  &&
           CodCurso == ord1.Codigo_Curs
           select new
           {
               ord1.Numero_Grupo,
               ord2.Codigo_Curso,
               ord2.Nombre_Curso,
               ord1.NumeroPeriodo,
               ord1.Anno,
               ord1.Horario,
               ord3.Nombre,
               ord3.Primer_Apellido,
               ord3.Segundo_apellido,
               ord3.Tipo_ID,
               ord3.Identificacion
              
           };


            return Ok(idQuery);
        }

        //------------------------------

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GruposExists(byte id)
        {
            return db.Grupos.Count(e => e.Numero_Grupo == id) > 0;
        }

        private bool GruposExistsCodigoCurso(string id)
        {
            return db.Grupos.Count(e => e.Codigo_Curs == id) > 0;
        }

        private bool CursoExists(string id)
        {
            return db.Cursos.Count(e => e.Codigo_Curso == id) > 0;
        }

       

        private bool ProfesoreExists(string tipoid)
        {
            return db.Profesores.Count(e => e.Tipo_ID == tipoid) > 0;
        }

        private bool ProfesoreExists2(string id)
        {
            return db.Profesores.Count(e => e.Identificacion == id) > 0;
        }

        private bool PeriodoExists(int id)
        {
            return db.Periodoes.Count(e => e.Anno == id) > 0;
        }

        private bool PeriodoExistsNuPeriodo(byte id)
        {
            return db.Periodoes.Count(e => e.NumeroPeriodo == id) > 0;
        }

        private string ConsultaEstadoPeriodo(int anno, byte numeroperiodo) {
            var estado =
             (from ord1 in db.Periodoes
              where ord1.Anno == anno && ord1.NumeroPeriodo == numeroperiodo
              select ord1.Estado);
            string estad = "";
            foreach (var item in estado)
            {
                estad = string.Concat(item);

            }

            return estad;
        }



    }
}