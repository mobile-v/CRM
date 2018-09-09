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
using PartionnyAccount.Models;
using PartionnyAccount.Models.Sklad.Doc;

namespace PartionnyAccount.Controllers.Sklad.Doc.DocRetailActWriteOffs
{
    public class DocRetailActWriteOffTabsController : ApiController
    {
        private DbConnectionSklad db = new DbConnectionSklad();

        // GET: api/DocRetailActWriteOffTabs
        public IQueryable<DocRetailActWriteOffTab> GetDocRetailActWriteOffTabs()
        {
            return db.DocRetailActWriteOffTabs;
        }

        // GET: api/DocRetailActWriteOffTabs/5
        [ResponseType(typeof(DocRetailActWriteOffTab))]
        public async Task<IHttpActionResult> GetDocRetailActWriteOffTab(int id)
        {
            DocRetailActWriteOffTab docRetailActWriteOffTab = await db.DocRetailActWriteOffTabs.FindAsync(id);
            if (docRetailActWriteOffTab == null)
            {
                return NotFound();
            }

            return Ok(docRetailActWriteOffTab);
        }

        // PUT: api/DocRetailActWriteOffTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocRetailActWriteOffTab(int id, DocRetailActWriteOffTab docRetailActWriteOffTab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != docRetailActWriteOffTab.DocRetailActWriteOffTabID)
            {
                return BadRequest();
            }

            db.Entry(docRetailActWriteOffTab).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocRetailActWriteOffTabExists(id))
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

        // POST: api/DocRetailActWriteOffTabs
        [ResponseType(typeof(DocRetailActWriteOffTab))]
        public async Task<IHttpActionResult> PostDocRetailActWriteOffTab(DocRetailActWriteOffTab docRetailActWriteOffTab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DocRetailActWriteOffTabs.Add(docRetailActWriteOffTab);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = docRetailActWriteOffTab.DocRetailActWriteOffTabID }, docRetailActWriteOffTab);
        }

        // DELETE: api/DocRetailActWriteOffTabs/5
        [ResponseType(typeof(DocRetailActWriteOffTab))]
        public async Task<IHttpActionResult> DeleteDocRetailActWriteOffTab(int id)
        {
            DocRetailActWriteOffTab docRetailActWriteOffTab = await db.DocRetailActWriteOffTabs.FindAsync(id);
            if (docRetailActWriteOffTab == null)
            {
                return NotFound();
            }

            db.DocRetailActWriteOffTabs.Remove(docRetailActWriteOffTab);
            await db.SaveChangesAsync();

            return Ok(docRetailActWriteOffTab);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocRetailActWriteOffTabExists(int id)
        {
            return db.DocRetailActWriteOffTabs.Count(e => e.DocRetailActWriteOffTabID == id) > 0;
        }
    }
}