using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManagement.Abstract
{
    public interface IAuth
    {
        void DoAuth(string userName, bool remember);
        void LogOff();
    }
}