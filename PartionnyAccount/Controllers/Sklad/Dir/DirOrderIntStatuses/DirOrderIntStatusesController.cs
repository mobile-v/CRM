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
using PartionnyAccount.Models.Sklad.Dir;

namespace PartionnyAccount.Controllers.Sklad.Dir.DirOrderIntStatuses
{
    public class DirOrderIntStatusesController : ApiController
    {
        private DbConnectionSklad db = new DbConnectionSklad();

        // GET: api/DirOrderIntStatuses
        public IQueryable<DirOrderIntStatus> GetDirOrderIntStatuses()
        {
            return db.DirOrderIntStatuses;
        }

        // GET: api/DirOrderIntStatuses/5
        [ResponseType(typeof(DirOrderIntStatus))]
        public async Task<IHttpActionResult> GetDirOrderIntStatus(int id)
        {
            DirOrderIntStatus dirOrderIntStatus = await db.DirOrderIntStatuses.FindAsync(id);
            if (dirOrderIntStatus == null)
            {
                return NotFound();
            }

            return Ok(dirOrderIntStatus);
        }

        // PUT: api/DirOrderIntStatuses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirOrderIntStatus(int id, DirOrderIntStatus dirOrderIntStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dirOrderIntStatus.DirOrderIntStatusID)
            {
                return BadRequest();
            }

            db.Entry(dirOrderIntStatus).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DirOrderIntStatusExists(id))
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

        // POST: api/DirOrderIntStatuses
        [ResponseType(typeof(DirOrderIntStatus))]
        public async Task<IHttpActionResult> PostDirOrderIntStatus(DirOrderIntStatus dirOrderIntStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DirOrderIntStatuses.Add(dirOrderIntStatus);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = dirOrderIntStatus.DirOrderIntStatusID }, dirOrderIntStatus);
        }

        // DELETE: api/DirOrderIntStatuses/5
        [ResponseType(typeof(DirOrderIntStatus))]
        public async Task<IHttpActionResult> DeleteDirOrderIntStatus(int id)
        {
            DirOrderIntStatus dirOrderIntStatus = await db.DirOrderIntStatuses.FindAsync(id);
            if (dirOrderIntStatus == null)
            {
                return NotFound();
            }

            db.DirOrderIntStatuses.Remove(dirOrderIntStatus);
            await db.SaveChangesAsync();

            return Ok(dirOrderIntStatus);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DirOrderIntStatusExists(int id)
        {
            return db.DirOrderIntStatuses.Count(e => e.DirOrderIntStatusID == id) > 0;
        }
    }
}