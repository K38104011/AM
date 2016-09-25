using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccountManagement.Abstract;
using System.Linq;
using AccountManagement.Models;
using AccountManagement.Controllers;
using System.Web.Mvc;

namespace AccountManagement.UnitTests
{
    [TestClass]
    public class GroupUnitTest
    {
        [TestMethod]
        public void Can_Add_Group()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository()
            {
                CreateGroup = (group) => { return true; }
            };

            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            AddGroupView addGroupView = new AddGroupView
            {
                Name = "G4",
                ParentDn = "dn2"
            };

            // Act
            var result = (RedirectToRouteResult)target.Add(addGroupView);

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Add Group 'G4' success!", target.TempData["message-success"]);
        }

        [TestMethod]
        public void Cannot_Add_Group_Invalid_Inputs()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            target.ModelState.AddModelError("error", "error");
            AddGroupView addGroupView = new AddGroupView
            {
                Name = "G4",
                ParentDn = "dn2"
            };

            // Act
            ActionResult result = target.Add(addGroupView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Group()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository()
            {
                DeleteString = (name) => { return new Group { name = "test", dn = "dn", parent = "parent" }; }
            };

            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            string deleteGroupName = "G1";

            // Act
            GroupController target = new GroupController(repoFakes);
            var result = target.Delete(deleteGroupName);

            // Assert
            Assert.AreEqual("Delete group 'G1' success", target.TempData["message-success"]);
        }

        [TestMethod]
        public void Cannot_Delete_Group_Has_Child()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository()
            {
                IsHasChildString = (name) => { return true; }
            };

            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            string deleteGroupName = "G1";

            // Act
            GroupController target = new GroupController(repoFakes);
            var result = target.Delete(deleteGroupName);

            // Assert
            Assert.AreEqual("Can't delete group has child", target.TempData["message-fail"]);
        }

        [TestMethod]
        public void Cannot_Delete_Nonexistent_Group()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository()
            {
                DeleteString = (name) => { return null; }
            };

            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            string deleteGroupName = "G1";

            // Act
            GroupController target = new GroupController(repoFakes);
            var result = target.Delete(deleteGroupName);

            // Assert
            Assert.AreEqual("Delete group 'G1' fail", target.TempData["message-fail"]);
        }

        [TestMethod]
        public void Can_Edit_Group()
        {
            // Arrange:

            // Create the fake LDAPGroupRepository
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository()
            {
                EditGroup = (group) => { return true; }
            };

            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            EditGroupView editGroupView = new EditGroupView
            {
                Name = "newG1",
                Dn = "dn1",
            };

            // Act
            var result = (RedirectToRouteResult)target.Edit(editGroupView);

            // Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Edit group 'newG1' success", target.TempData["message-success"]);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Group()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            EditGroupView editGroupView = new EditGroupView
            {
                Name = "G4",
                Dn = "dn4",
            };

            // Act
            ActionResult result = target.Edit(editGroupView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Group 'G4' didn't exist!", target.TempData["message-fail"]);
        }

        [TestMethod]
        public void Cannot_Edit_Group_Invalid_Inputs()
        {
            // Arrage
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            target.ModelState.AddModelError("error", "error");
            EditGroupView editGroupView = new EditGroupView
            {
                Name = "newG1",
                Dn = "dn1",
            };

            // Act
            ActionResult result = target.Edit(editGroupView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Move_Group()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository()
            {
                MoveGroupGroup = (group, parentGroup) => { return true; }
            };

            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            MoveGroupView moveGroupView = new MoveGroupView
            {
                Name = "G3",
                Dn = "dn3",
                selectedDn = "dn2"
            };

            // Act
            var result = (RedirectToRouteResult)target.Move(moveGroupView);

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Move Group 'G3' success!", target.TempData["message-success"]);
        }

        [TestMethod]
        public void Cannot_Move_Group_Invalid_Inputs()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            target.ModelState.AddModelError("error", "error");
            MoveGroupView moveGroupView = new MoveGroupView
            {
                Name = "G3",
                Dn = "dn3",
                selectedDn = "dn2"
            };

            // Act
            var result = target.Move(moveGroupView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Move_Nonexistent_Group()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            MoveGroupView moveGroupView = new MoveGroupView
            {
                Name = "G4",
                Dn = "dn4",
                selectedDn = "dn2"
            };

            // Act
            var result = target.Move(moveGroupView);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Group 'G4' didn't exist!", target.TempData["message-fail"]);
        }

        [TestMethod]
        public void Can_Export_Group_To_Excel()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();

            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            string nameGroup = "G1";

            using (Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create())
            {
                AccountManagement.Infrastructure.Fakes.ShimLDAPHelper.
                    AllInstances.ExportGroupToExcelGroup = (group, returnResult) => { return true; };

                // Act
                var result = (RedirectToRouteResult)target.ExportToExcel(nameGroup);

                // Assert
                Assert.AreEqual("Index", result.RouteValues["action"]);
                Assert.AreEqual("Home", result.RouteValues["controller"]);
                Assert.AreEqual("Export group 'G1' success!", target.TempData["message-success"]);
            }
        }

        [TestMethod]
        public void Cannot_Export_Nonexist_Group()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            string nameGroup = "G4";

            // Act
            var result = (RedirectToRouteResult)target.ExportToExcel(nameGroup);

            // Assert
            Assert.AreEqual("Group 'G4' didn't exist!", target.TempData["message-fail"]);
        }

        [TestMethod]
        public void Can_Check_Group_Exists()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            string groupNameExists = "G2";

            // Act
            var result = target.isExists(groupNameExists);

            // Assert
            Assert.IsNotNull(result.Data);
        }

        [TestMethod]
        public void Can_Check_Group_Has_Child()
        {
            // Arrange
            IGroupRepository repoFakes = new AccountManagement.Abstract.Fakes.StubIGroupRepository();
            repoFakes.Groups = new Group[] {
                new Group { name = "G1", dn = "dn1" },
                new Group { name = "G2", dn = "dn2" },
                new Group { name = "G3", dn = "dn3"},
            }.AsQueryable();

            GroupController target = new GroupController(repoFakes);
            string groupNameHasChild = "G2";

            // Act
            var result = target.isHasChild(groupNameHasChild);

            // Assert
            Assert.IsNotNull(result.Data);
        }
    }
}
