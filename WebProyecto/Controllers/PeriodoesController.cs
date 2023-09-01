using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using ProyectoPrograV;
using WebProyecto.Models;

namespace WebProyecto.Controllers
{
    public class PeriodoesController : ApiController
    {
        private tiusr5pl_Proyecto1PrograVEntities db = new tiusr5pl_Proyecto1PrograVEntities();

        // GET: api/Periodoes
        public IQueryable<Periodo> GetPeriodoes()
        {
            return db.Periodoes;
        }

        // GET: api/Periodoes/5
        [ResponseType(typeof(Periodo))]
        public async Task<IHttpActionResult> GetPeriodo(int id,int NumPerido)
        {
            Periodo periodo = await db.Periodoes.FindAsync(id,NumPerido);
            if (periodo == null)
            {
                return NotFound();
            }

            return Ok(periodo);
        }

        [Route("api/Periodoes/periodosordenados")]
        [HttpGet]
        public IHttpActionResult getPeriodos()
        {
            try
            {
               // db.Periodoes.OrderByDescending(periodo => periodo.Fecha_Inicio);
                
                var idQuery = from ord1 in db.Periodoes
                              orderby ord1.Fecha_Inicio descending
                              select new
                              {
                                  ord1.Anno,
                                  ord1.Fecha_Inicio,
                                  ord1.Fecha_Fin,
                                 //fechafin = DateTime.Parse(ord1.Fecha_Fin.ToString()).ToString("dd/MM/yyyy"),
                                  ord1.NumeroPeriodo,
                                  ord1.Estado,
                                
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





        // PUT: api/Periodoes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPeriodo(periodos p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!PeriodoExists(p.anno) || !PeriodoExistsNuPeriodo(p.numeroPeriodo))
            {
                return NotFound();
            }

            if (EstadoActivoExist(p.estado.ToUpper()))
            {
                return BadRequest("Solo puede existir un periodo activo");

            }

            Periodo e2 = new Periodo()
            {
                Anno = p.anno,
                NumeroPeriodo = p.numeroPeriodo,
                Fecha_Inicio = p.fechaInicio,
                Fecha_Fin = p.fechafinal,
                Estado = p.estado,

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

    

        [HttpPost]
        [Route("api/Periodoes/Crearperiodo")]
        [ResponseType(typeof(periodos))]
        public async Task<IHttpActionResult> Crearperiodo
      ([FromBody] periodos p)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Validamos que solo exista un estado activo en el periodo

            if (EstadoActivoExist(p.estado.ToUpper()))
            {
                return BadRequest("Solo puede existir un periodo activo");

            }
            Periodo P1 = new Periodo()
            {
                Anno = p.anno,
                NumeroPeriodo = p.numeroPeriodo,
                Fecha_Inicio = p.fechaInicio,
                Fecha_Fin = p.fechafinal,
                Estado = p.estado
            };
            db.Periodoes.Add(P1);

            try
            {
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { Controller = "Periodoes", id = P1.Anno, P1.NumeroPeriodo }, P1);
            }
            catch (DbUpdateException)
            {
                if (PeriodoExists(p.anno) & PeriodoExistsNuPeriodo(p.numeroPeriodo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

           


        }




        // DELETE: api/Periodoes/5
        [ResponseType(typeof(Periodo))]
        public async Task<IHttpActionResult> DeletePeriodo(string id)
        {
            string[] llaves = id.Split('-');
            int anno = int.Parse(llaves[0]);
            int numperiodo = int.Parse(llaves[1]);

            Periodo periodo = await db.Periodoes.FindAsync(anno, numperiodo);
            if (periodo == null)
            {
                return NotFound();
            }

            db.Periodoes.Remove(periodo);
            await db.SaveChangesAsync();

            return Ok(periodo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PeriodoExists(int id)
        {
            return db.Periodoes.Count(e => e.Anno == id) > 0;
        }

        private bool PeriodoExistsNuPeriodo(int id)
        {
            return db.Periodoes.Count(e => e.NumeroPeriodo == id) > 0;
        }

        private bool EstadoActivoExist(string estado)
        {
            if (estado == "A")
            {
             return db.Periodoes.Count(e => e.Estado == estado) > 0;
            }

            return false;
        }
       
    }
}