using AccountManagement.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace AccountManagement.Concrete
{
    public class FormsAuthWrapper : IAuth
    {
        public void DoAuth(string userName, bool remember)
        {
            FormsAuthentication.SetAuthCookie(userName, remember);
        }
        public void LogOff()
        {
            FormsAuthentication.SignOut();
        }
    }
}