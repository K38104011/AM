using AccountManagement.Abstract;
using AccountManagement.Concrete;
using AccountManagement.Infrastructure;
using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace AccountManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private IUserRepository repository;

        public MvcApplication() : this(new LDAPUserRepository()) { }

        public MvcApplication(IUserRepository repo)
        {
            repository = repo;
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        string roles = string.Empty;

                        User user = repository.Users.SingleOrDefault(u => u.Account == username);
                        roles = user.Roles;

                        HttpContext.Current.User = new GenericPrincipal(
                            new GenericIdentity(username, "Forms"), roles.Split(';'));
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

    }
}
