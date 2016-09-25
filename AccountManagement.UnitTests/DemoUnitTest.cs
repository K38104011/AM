using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccountManagement.Abstract;
using AccountManagement.Models;
using System.Linq;
using AccountManagement.Controllers;
using System.Web.Mvc;

namespace AccountManagement.UnitTests
{
    [TestClass]
    public class DemoUnitTest
    {
        [TestMethod]
        public void Can_Edit_Profile()
        {
            //// Arrange
            //IGroupRepository groupsFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();


            //IUserRepository usersFakes = new AccountManagement.Abstract.Fakes.StubIUserRepository();
            //usersFakes.Users = new User[]
            //{
            //    new User { Account = "a1", FirstName = "f", LastName = "l", Phone = "1", Dn = "d1", Email = "e"},
            //    new User { Account = "a2", FirstName = "f", LastName = "l", Phone = "1", Dn = "d2", Email = "e"},
            //    new User { Account = "a3", FirstName = "f", LastName = "l", Phone = "1", Dn = "d3", Email = "e"},
            //}.AsQueryable();

            //EditProfileView edv = new EditProfileView
            //{
            //    FirstName = "new",
            //    LastName = "new",
            //    Phone = "2",
            //    Dn = "d3",
            //    Email = "E"
            //};

            //// Act
            //UserController target = new UserController(groupsFakes, usersFakes);
            //var result = (RedirectToRouteResult)target.EditProfile(edv);

            //// Assert
            //Assert.AreEqual("Index", result.RouteValues["action"]);
            //Assert.AreEqual("Home", result.RouteValues["controller"]);
        }
    }
}
