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
using System.Web.Http.ModelBinding;
using System.Web.OData;
using ServicoREST.Models;

namespace ServicoREST.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using ServicoREST.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Pessoas>("PessoasOData");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PessoasODataController : ODataController
    {
        private DadosModel db = new DadosModel();

        // GET: odata/PessoasOData
        [EnableQuery]
        public IQueryable<Pessoas> GetPessoasOData()
        {
            return db.Pessoas;
        }

        // GET: odata/PessoasOData(5)
        [EnableQuery]
        public SingleResult<Pessoas> GetPessoas([FromODataUri] int key)
        {
            return SingleResult.Create(db.Pessoas.Where(pessoas => pessoas.idPessoa == key));
        }

        // PUT: odata/PessoasOData(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Pessoas> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Pessoas pessoas = await db.Pessoas.FindAsync(key);
            if (pessoas == null)
            {
                return NotFound();
            }

            patch.Put(pessoas);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PessoasExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(pessoas);
        }

        // POST: odata/PessoasOData
        public async Task<IHttpActionResult> Post(Pessoas pessoas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Pessoas.Add(pessoas);
            await db.SaveChangesAsync();

            return Created(pessoas);
        }

        // PATCH: odata/PessoasOData(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Pessoas> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Pessoas pessoas = await db.Pessoas.FindAsync(key);
            if (pessoas == null)
            {
                return NotFound();
            }

            patch.Patch(pessoas);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PessoasExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(pessoas);
        }

        // DELETE: odata/PessoasOData(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Pessoas pessoas = await db.Pessoas.FindAsync(key);
            if (pessoas == null)
            {
                return NotFound();
            }

            db.Pessoas.Remove(pessoas);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PessoasExists(int key)
        {
            return db.Pessoas.Count(e => e.idPessoa == key) > 0;
        }
    }
}
