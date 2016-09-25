using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccountManagement.Abstract;
using AccountManagement.Models;
using System.Linq;
using AccountManagement.Controllers;
using System.Web.Mvc;
using Microsoft.QualityTools.Testing.Fakes;
using System.Security.Policy;
using System.Web.Security;

namespace AccountManagement.UnitTests
{
    [TestClass]
    public class AccountUnitTest
    {
        [TestMethod]
        public void Can_ForgotPassWord()
        {
            IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository();
            IAuth authfakes = new AccountManagement.Abstract.Fakes.StubIAuth();
            usersFakes.Users = new User[]
            {
                new User { Account = "a1", Email="a", Password="b" },
                new User { Account = "a2", Email="a", Password="b" },
            }.AsQueryable();
            ForgotPasswordView fpv = new ForgotPasswordView
            {
                Account = "a1",
            };
            // Act
            AccountController target = new AccountController(usersFakes,authfakes);
            var result = (RedirectToRouteResult)target.ForgotPassword(fpv);

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void Can_Login()
        {
            bool wasDoAuthCalled = false;
            IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository();
            IAuth authfakes = new AccountManagement.Abstract.Fakes.StubIAuth()
            {
                DoAuthStringBoolean = (username, remember) =>
                {
                    wasDoAuthCalled = true;
                }

            };

            usersFakes.Users = new User[]
            {
                // hash 'b' password
                new User { Account = "a1", Password="3e23e8160039594a33894f6564e1b1348bbd7a0088d42c4acb73eeaed59c009d" },
                new User { Account = "a2", Password="3e23e8160039594a33894f6564e1b1348bbd7a0088d42c4acb73eeaed59c009d" },
            }.AsQueryable();

            LoginView lgv = new LoginView
            {
                Account = "a1",
                Password = "b"
            };

            AccountController target = new AccountController(usersFakes, authfakes);

            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                System.Web.Mvc.Fakes.ShimUrlHelper UrlHelper = new System.Web.Mvc.Fakes.ShimUrlHelper()
                {
                    IsLocalUrlString = (url) => { return false; }
                };

                // Act
                target.Url = UrlHelper;
               var result = target.Login(lgv, "test");

                // Assert
                Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
                Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
                Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
                Assert.IsTrue(wasDoAuthCalled);
            }
        }

        [TestMethod]
        public void Cannot_Login_Nonexistent()
        {
            IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository();
            IAuth authfakes = new AccountManagement.Abstract.Fakes.StubIAuth();
            usersFakes.Users = new User[]
            {
                new User { Account = "a1", Password="b" },
                new User { Account = "a2", Password="b" },
            }.AsQueryable();
            AccountController target = new AccountController(usersFakes, authfakes);
            LoginView lgv = new LoginView
            {
                Account = "a3",
                Password ="b"
            };
            // Act

            var result = target.Login(lgv, "test");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("The user name or password provided is incorrect.", "The user name or password provided is incorrect.");
        }

        [TestMethod]
        public void LogOff()
        {
            bool wasCalled = false;
            IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository();
            IAuth authfakes = new AccountManagement.Abstract.Fakes.StubIAuth()
            {
                LogOff = () => { wasCalled = true; }

            };
            AccountController target = new AccountController(usersFakes, authfakes);
            var result = target.LogOff();

            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["Action"]);
            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.IsTrue(wasCalled);
        }
        

    }
}
