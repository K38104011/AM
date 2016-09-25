using AccountManagement.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult GetUserAndGroupDataJson()
        {
            LDAPHelper helper = new LDAPHelper();
            return Json(helper.GetRootTreeNode("fa fa-folder-open", "fa fa-user"), JsonRequestBehavior.AllowGet);
        }
    }
}