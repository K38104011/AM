using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccountManagement.Abstract;
using AccountManagement.Models;
using AccountManagement.Controllers;
using System.Web.Mvc;
using System.Linq;

namespace AccountManagement.UnitTests
{
    /// <summary>
    /// Summary description for UserUnitTest
    /// </summary>
    [TestClass]
    public class UserUnitTest
    {
        [TestMethod]
        public void Can_Add_Users()
        {
            // Arrange
            IGroupRepository groupsFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository()
            {
                CreateUser = (usersArray) => { return true; }
            };

            usersFakes.Users = new User[]
            {
                new User { Account = "a1", FirstName = "f1", LastName = "l1", Phone = "1234567891", Dn = "d1", Email = "e1"},
                new User { Account = "a2", FirstName = "f2", LastName = "l2", Phone = "1234567891", Dn = "d2", Email = "e2"},
                new User { Account = "a3", FirstName = "f3", LastName = "l3", Phone = "1234567891", Dn = "d3", Email = "e3"},
            }.AsQueryable();

            UserController target = new UserController(groupsFakes, usersFakes);

            User[] users = new User[] {
                new User { Account = "a4", FirstName = "f", LastName = "l", Phone = "1234567891", Dn = "d4", Email = "e4"},
                new User { Account = "a5", FirstName = "f", LastName = "l", Phone = "1234567891", Dn = "d5", Email = "e5"},
                new User { Account = "a6", FirstName = "f", LastName = "l", Phone = "1234567891", Dn = "d6", Email = "e6"},
            };

            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                
                System.Web.Mvc.Fakes.ShimUrlHelper UrlHelper = new System.Web.Mvc.Fakes.ShimUrlHelper()
                {
                    ActionStringString = (action, controller) => "/Home/Index"
                };

                // Act
                target.Url = UrlHelper;
                var result = target.Add(users);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(JsonResult));
                Assert.AreEqual("/Home/Index", ((JsonResult)result).Data);
                Assert.AreEqual("Add users success!", target.TempData["message-success"]);
            }
        }

        [TestMethod]
        public void Can_Edit_User()
        { // Arrange
            IGroupRepository groupsFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository()
            {
                EditUser = (user) => { return true; }
            };

            groupsFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();
            usersFakes.Users = new User[]
            {
                new User { Account = "a1", FirstName = "f", LastName = "l", Phone = "1", Dn = "d1", Email = "e"},
                new User { Account = "a2", FirstName = "f", LastName = "l", Phone = "1", Dn = "d2", Email = "e"},
                new User { Account = "a3", FirstName = "f", LastName = "l", Phone = "1", Dn = "d3", Email = "e"},
            }.AsQueryable();

            EditUserView editUserView = new EditUserView
            {
                Account = "a1",
                FirstName = "new",
                LastName = "new",
                Phone = "2",
                Dn = "d1",
                Email = "E"
            };

            // Act
            UserController target = new UserController(groupsFakes, usersFakes);
            var result = (RedirectToRouteResult)target.EditUser(editUserView);

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Edit user 'a1' success", target.TempData["message-success"]);
        }

        [TestMethod]
        public void Edit_User_Invalid_Input_Test()
        {
            IGroupRepository groupsFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository();
            usersFakes.Users = new User[]
            {
                new User { Account = "a1", FirstName = "f", LastName = "l", Phone = "1", Dn = "d1", Email = "e"},
                new User { Account = "a2", FirstName = "f", LastName = "l", Phone = "1", Dn = "d2", Email = "e"},
                new User { Account = "a3", FirstName = "f", LastName = "l", Phone = "1", Dn = "d3", Email = "e"},
            }.AsQueryable();
            UserController target = new UserController(groupsFakes, usersFakes);
            target.ModelState.AddModelError("error", "error");
            EditUserView editUserView = new EditUserView
            {
                FirstName = "new",
                LastName = "new",
                Phone = "2",
                Dn = "d3",
                Email = "E"
            };
            //act
            ActionResult result = target.EditUser(editUserView);
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Edit_User_None_Existent() {    
            // Arrange
            IGroupRepository groupsFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository();
            usersFakes.Users = new User[]
            {
                new User { Account = "a1", FirstName = "f", LastName = "l", Phone = "1", Dn = "d1", Email = "e"},
                new User { Account = "a2", FirstName = "f", LastName = "l", Phone = "1", Dn = "d2", Email = "e"},
                new User { Account = "a3", FirstName = "f", LastName = "l", Phone = "1", Dn = "d3", Email = "e"},
            }.AsQueryable();

            UserController target = new UserController(groupsFakes, usersFakes);
            EditUserView editUserView = new EditUserView
            {
                Account = "a4",
                FirstName = "new",
                LastName = "new",
                Phone = "2",
                Dn = "d2",
                Email = "E"
            };

            // Act
            ActionResult result = target.EditUser(editUserView);
           
            //// Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("User 'a4' doesn't exist", target.TempData["message-fail"]);
        }
    }
}
