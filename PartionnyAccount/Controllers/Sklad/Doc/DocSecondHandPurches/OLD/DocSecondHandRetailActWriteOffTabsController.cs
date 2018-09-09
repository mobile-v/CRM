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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandPurches
{
    public class DocSecondHandRetailActWriteOffTabsController : ApiController
    {
        private DbConnectionSklad db = new DbConnectionSklad();

        // GET: api/DocSecondHandRetailActWriteOffTabs
        public IQueryable<DocSecondHandRetailActWriteOffTab> GetDocSecondHandRetailActWriteOffTabs()
        {
            return db.DocSecondHandRetailActWriteOffTabs;
        }

        // GET: api/DocSecondHandRetailActWriteOffTabs/5
        [ResponseType(typeof(DocSecondHandRetailActWriteOffTab))]
        public async Task<IHttpActionResult> GetDocSecondHandRetailActWriteOffTab(int id)
        {
            DocSecondHandRetailActWriteOffTab docSecondHandRetailActWriteOffTab = await db.DocSecondHandRetailActWriteOffTabs.FindAsync(id);
            if (docSecondHandRetailActWriteOffTab == null)
            {
                return NotFound();
            }

            return Ok(docSecondHandRetailActWriteOffTab);
        }

        // PUT: api/DocSecondHandRetailActWriteOffTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRetailActWriteOffTab(int id, DocSecondHandRetailActWriteOffTab docSecondHandRetailActWriteOffTab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != docSecondHandRetailActWriteOffTab.DocSecondHandRetailActWriteOffTabID)
            {
                return BadRequest();
            }

            db.Entry(docSecondHandRetailActWriteOffTab).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocSecondHandRetailActWriteOffTabExists(id))
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

        // POST: api/DocSecondHandRetailActWriteOffTabs
        [ResponseType(typeof(DocSecondHandRetailActWriteOffTab))]
        public async Task<IHttpActionResult> PostDocSecondHandRetailActWriteOffTab(DocSecondHandRetailActWriteOffTab docSecondHandRetailActWriteOffTab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DocSecondHandRetailActWriteOffTabs.Add(docSecondHandRetailActWriteOffTab);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = docSecondHandRetailActWriteOffTab.DocSecondHandRetailActWriteOffTabID }, docSecondHandRetailActWriteOffTab);
        }

        // DELETE: api/DocSecondHandRetailActWriteOffTabs/5
        [ResponseType(typeof(DocSecondHandRetailActWriteOffTab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRetailActWriteOffTab(int id)
        {
            DocSecondHandRetailActWriteOffTab docSecondHandRetailActWriteOffTab = await db.DocSecondHandRetailActWriteOffTabs.FindAsync(id);
            if (docSecondHandRetailActWriteOffTab == null)
            {
                return NotFound();
            }

            db.DocSecondHandRetailActWriteOffTabs.Remove(docSecondHandRetailActWriteOffTab);
            await db.SaveChangesAsync();

            return Ok(docSecondHandRetailActWriteOffTab);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocSecondHandRetailActWriteOffTabExists(int id)
        {
            return db.DocSecondHandRetailActWriteOffTabs.Count(e => e.DocSecondHandRetailActWriteOffTabID == id) > 0;
        }
    }
}