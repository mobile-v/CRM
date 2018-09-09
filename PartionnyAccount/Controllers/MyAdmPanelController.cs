using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace PartionnyAccount.Controllers
{
    public class MyAdmPanelController : Controller
    {
        // GET: MyAdmPanel
        public ActionResult Index()
        {
            return View();
        }


        #region Исправление остатков

        public async Task<ActionResult> Remans()
        {
            return View("Remans");
        }


        //RemParties - RemPartyMinuses

        public async Task<ActionResult> RemPartiesCheck()
        {
            Classes.Update.Update update = new Classes.Update.Update();
            ViewData["MsgBlack"] = await update.Start("RemPartiesCheck");

            return View("Remans");
        }

        public async Task<ActionResult> RemPartiesCorrect()
        {
            Classes.Update.Update update = new Classes.Update.Update();
            ViewData["MsgBlack"] = await update.Start("RemPartiesCorrect");

            return View("Remans");
        }


        //RemParties - RemRemnants

        public async Task<ActionResult> RemRemnantsCheck()
        {
            Classes.Update.Update update = new Classes.Update.Update();
            ViewData["MsgBlack"] = await update.Start("RemRemnantsCheck");

            return View("Remans");
        }

        public async Task<ActionResult> RemRemnantsCorrect()
        {
            Classes.Update.Update update = new Classes.Update.Update();
            ViewData["MsgBlack"] = await update.Start("RemRemnantsCorrect");

            return View("Remans");
        }

        #endregion


        #region Обновлеие

        public async Task<ActionResult> Update()
        {
            return View("Update");
        }

        public async Task<ActionResult> UpdateStart()
        {
            Classes.Update.Update update = new Classes.Update.Update();
            ViewData["MsgBlack"] = await update.Start("Update");

            return View("Update");
        }

        #endregion

    }
}