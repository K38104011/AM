using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AccountManagement.Infrastructure;
using AccountManagement.Abstract;
using AccountManagement.Concrete;
using SendMail;

namespace AccountManagement.Controllers
{
    public class AccountController : Controller
    {
        private IUserRepository repository;
        private IAuth auth;

        public AccountController() : this(new LDAPUserRepository(), new FormsAuthWrapper()) { }

        public AccountController(IUserRepository repo, IAuth auth)
        {
            this.repository = repo;
            this.auth = auth;
        }

        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginView model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string username = model.Account;
                string hashedPassword = "";

                using (System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256Managed.Create())
                {
                    hashedPassword = String.Join("", hash
                        .ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password))
                        .Select(item => item.ToString("x2")));
                }

                bool userValid = repository.Users.Any(user => user.Account == username && user.Password == hashedPassword);
                
                if (userValid)
                {
                    auth.DoAuth(username, false);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            auth.LogOff();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordView ForgotPassword)
        {
            if (ModelState.IsValid)
            {
                //random pass
                var pass = Guid.NewGuid().ToString().Substring(0, 8);

                User user = repository.Users.FirstOrDefault(u => u.Account == ForgotPassword.Account);
                user.Password = pass;

                if (repository.Edit(user))
                {
                    string content = user.Password;
                    var toEmail = user.Email; //email ng dung

                    new MailHelper().SendEmail(toEmail, "[no-reply] Your new password", content);
                    TempData["message-success"] = string.Format("Send mail success");
                }
                else
                {
                    TempData["message-fail"] = string.Format("Send mail fail");
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(ForgotPassword);
            }
        }

        public JsonResult isAccountDoesntExist(string Account)
        {
            return Json(repository.Users.Any(user => user.Account == Account), JsonRequestBehavior.AllowGet);
        }
    }
}