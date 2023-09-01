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
using System.Web.WebPages;
using ProyectoPrograV;
using WebProyecto.Models;

namespace WebProyecto.Controllers
{
    public class ProfesoresController : ApiController
    {
        private tiusr5pl_Proyecto1PrograVEntities db = new tiusr5pl_Proyecto1PrograVEntities();

        // GET: api/Profesores
        public IQueryable<Profesore> GetProfesores()
        {
            return db.Profesores;
        }

        // GET: api/Profesores/5
        [ResponseType(typeof(profesorConsulta))]
        public async Task<IHttpActionResult> GetProfesore(string id)
        {
            string[] llaves = id.Split('-');
            string id2 = llaves[0];
            string tipoid = llaves[1];

            Profesore profesorConsulta = await db.Profesores.FindAsync(id2,tipoid);
            if (profesorConsulta == null)
            {
                return NotFound();
            }

            return Ok(profesorConsulta);
        }

        [Route("api/Profesores/ConsultaProfesores")]
        [HttpGet]
        public IHttpActionResult getDatosProfesores()
        {
            try
            {
                var idQuery =
                  from ord1 in db.Profesores
                  select new
                  {
                      ord1.Tipo_ID,
                      ord1.Identificacion,
                      ord1.Nombre,
                      ord1.Primer_Apellido,
                      ord1.Segundo_apellido,
                      ord1.Fecha_Nacimiento
                     
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



        // PUT: api/Profesores/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProfesore( estudianteActualiza p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ProfesoreExists(p.tipo_ID) || !ProfesoreExists2(p.Identificacion))
            {
                return NotFound();
            }

            Profesore p2 = new Profesore()
            {
                Nombre = p.Nombre,
                Primer_Apellido = p.primer_Apellido,
                Segundo_apellido = p.segundo_apellido,
                Fecha_Nacimiento = p.fecha_Nacimiento,
               Identificacion = p.Identificacion,
               Tipo_ID = p.tipo_ID
            };


            db.Entry(p2).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;
            }

            return Ok(p2);
        }

        [HttpPost]
        [Route("api/Profesores/CrearProfesor")]
        [ResponseType(typeof(profesor))]
        public async Task<IHttpActionResult> postProfesor
      ([FromBody] profesor p)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Profesore p1 = new Profesore()
            {
                Nombre = p.Nombre,
                Primer_Apellido = p.primerApellido,
                Segundo_apellido = p.SegundoApellido,
                Tipo_ID = p.tipo_ID,
                Identificacion = p.Identificacion,
                Fecha_Nacimiento = p.FechaNacimiento
            };
            db.Profesores.Add(p1);
            //Luego de agregar el estudiante validamos y agregamos el telefono y correo

            string[] telefonos = p.NumerosTelefono.Split();


            //Validamos que no vengan telefonos repetidos 
            bool estadoRepetidosTele = p.Validarepetidos(telefonos);
            if (estadoRepetidosTele == true)
            {
                return BadRequest("No puede ingresar numeros de telefono repetidos");
            }
            else
            {
                foreach (string telefono in telefonos)
                {
                    Telefonos_Profesores T1 = new Telefonos_Profesores()
                    {
                        Identificacion_Profesor = p1.Identificacion,
                        Tipo_ID_Profesor = p1.Tipo_ID,
                        Numero_Telefono = int.Parse(telefono.ToString()),


                    };
                    db.Telefonos_Profesores.Add(T1);

                }

            }

            //Validamos que no vengan correos repetidos
            string[] correos = p.CorreoEle.Split();
            bool EstadoRepetidoCorreo = p.Validarepetidos(correos);
            if (EstadoRepetidoCorreo == true)
            {
                return BadRequest("No puede ingresar correos electronicos repetidos");
            }
            else
            {
                //Si no vienen repetidos recorremos el arreglo de String y agregamos cada uno
                foreach (string correo in correos)
                {
                    Correos_Profesores C1 = new Correos_Profesores()
                    {
                        Identificacion_Profesor = p1.Identificacion,
                        Tipo_ID_Profesor = p1.Tipo_ID,
                        Corre_Electronico = correo.ToString(),
                    };
                    db.Correos_Profesores.Add(C1);

                }

            }

            try
            {
                await db.SaveChangesAsync();

            }
            catch (DbUpdateException)
            {
                if (ProfesoreExists(p.tipo_ID) & ProfesoreExists2(p.Identificacion))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { Controller = "Profesores", id = p1.Identificacion, p1.Tipo_ID }, p1);
        }

            //  var response = Request.CreateResponse(HttpStatusCode.Created);
            //Incluir el url del nuevo recurso creado
            // string uri = Url.Link("InserEstudian", new { id = e2.Identificacion });
            // response.Headers.Location = new Uri(uri);



        //------------------------
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        // DELETE: api/Profesores/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteProfesore(string id)
        {
            string[] llaves = id.Split('-');
            string id2 = llaves[0];
            string TipoID = llaves[1];
            Profesore profesore = await db.Profesores.FindAsync(TipoID, id2);

            if (profesore == null)
            {
                return NotFound();
            }
        
            db.Profesores.Remove(profesore);
            await db.SaveChangesAsync();
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
        //--------------------------------
        [Route("api/Profesores/ProfesorID")]
        [HttpGet]
        public IHttpActionResult getDatosProfesor(string id)
        {
            try
            {
                string[] llaves = id.Split('-');
                string id2 = llaves[0];
                string tipoid = llaves[1];
                var idQuery =
                  from ord1 in db.Profesores where ord1.Identificacion == id2 & ord1.Tipo_ID == tipoid 
                  select new
                  {
                      ord1.Nombre,
                      ord1.Primer_Apellido,
                      ord1.Segundo_apellido,
                      ord1.Fecha_Nacimiento,
                      ord1.Tipo_ID,
                      ord1.Identificacion
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

        [Route("api/Profesores/ProfesorxNombre")]
        [ResponseType(typeof(void))]
        [HttpGet]
        public IHttpActionResult getDatosProfesorNombre(string id)
        {
            try
            {
         
                var idQuery =
                  from ord1 in db.Profesores
                  where ord1.Nombre.Contains(id)
                  select new
                  {
                      ord1.Nombre,
                      ord1.Primer_Apellido,
                      ord1.Segundo_apellido,
                      ord1.Fecha_Nacimiento,
                      ord1.Tipo_ID,
                      ord1.Identificacion
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


        //-----------------------------
        //--------------------------------




        //--------------


        [ResponseType(typeof(Profesore))]
        [Route("api/Profesores/DatosProfesoresPorApellidos", Name ="getProfesoresPorApellidos")]

        public HttpResponseMessage getDatosProfesoresPorApellidos(string Apellido1,string Apellido2)
        {   //obtiene tipoID segun apellidos Profesor
            try {

                var idQuery = (from p in db.Profesores
                               where p.Primer_Apellido == Apellido1 && p.Segundo_apellido == Apellido2

                               select new
                               {
                                   p.Identificacion, p.Tipo_ID,p.Nombre,p.Primer_Apellido,p.Segundo_apellido,
                                   p.Fecha_Nacimiento ,p.Correos_Profesores,p.Telefonos_Profesores
                               }
                               ).ToList();
                Profesore profesor = db.Profesores.Where(x => x.Primer_Apellido == Apellido1 && x.Segundo_apellido == Apellido2).SingleOrDefault();

                if ( profesor != null) {
                    return Request.CreateResponse(HttpStatusCode.OK, idQuery);
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Profesor "+Apellido1+" "+Apellido2+" no ha sido encontrado");

            } catch (Exception ex) {

                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }


        }




        //--------------------------------


        [ResponseType(typeof(Profesore))]
        [Route("api/Profesores/DatosProfesoresPorNombre", Name = "getProfesoresPorNombre")]

        public HttpResponseMessage getDatosProfesoresPorNombre(string Nombre)
        {   //obtiene tipoID segun apellidos Profesor
            try
            {

                var idQuery = (from p in db.Profesores
                               where p.Nombre == Nombre 

                               select new
                               {
                                   p.Identificacion,
                                   p.Tipo_ID,
                                   p.Nombre,
                                   p.Primer_Apellido,
                                   p.Segundo_apellido,
                                   p.Fecha_Nacimiento,
                                   p.Correos_Profesores,
                                   p.Telefonos_Profesores
                               }
                               ).ToList();
                Profesore profesor = db.Profesores.Where(x => x.Nombre == Nombre).SingleOrDefault();

                if (profesor != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, idQuery);
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Profesor " + Nombre + " no ha sido encontrado");

            }
            catch (Exception ex)
            {

                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }


        }


        //--------------

        private bool ProfesoreExists(string tipoid)
        {
            return db.Profesores.Count(e => e.Tipo_ID == tipoid) > 0;
        }

        private bool ProfesoreExists2(string id)
        {
            return db.Profesores.Count(e => e.Identificacion == id) > 0;
        }
    }




}