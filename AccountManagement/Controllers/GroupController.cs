using AccountManagement.Abstract;
using AccountManagement.Concrete;
using AccountManagement.Infrastructure;
using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AccountManagement.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private IGroupRepository repository;

        public GroupController() : this(new LDAPGroupRepository()) { }

        public GroupController(IGroupRepository repo)
        {
            this.repository = repo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ViewResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(AddGroupView addGroupView)
        {
            if (ModelState.IsValid)
            {
                Group group = new Group();
                group.name = addGroupView.Name;
                group.parentDn = addGroupView.ParentDn;

                if (repository.Create(group))
                {
                    TempData["message-success"] = string.Format("Add Group '{0}' success!", group.name);
                }
                else
                {
                    TempData["message-fail"] = string.Format("Add group '{0}' fail!", group.name);
                }
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(addGroupView);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddManyGroups()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddManyGroups(string[] lstGroupName, string parentDn)
        {
            if (ModelState.IsValid)
            {
                Group newGroup;
                List<Group> lstGroup = new List<Group>();
                try
                {
                    int count = lstGroupName.Length;
                    for (int i = 0; i < count; i++)
                    {
                        newGroup = new Group()
                        {
                            name = lstGroupName[i],
                            parentDn = parentDn
                        };
                        lstGroup.Add(newGroup);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (repository.CreateManyGroups(lstGroup))
                {
                    TempData["message-success"] = string.Format("Add Many Groups success!");
                }
                else
                {
                    TempData["message-fail"] = string.Format("Add Many Groups fail!");
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ViewResult Move()
        {
            MoveGroupView mgv = new MoveGroupView();
            mgv._groupList = repository.Groups.OrderBy(g => g.name).ToList<Group>();
            return View(mgv);
        }

        [HttpPost]
        public ActionResult Move(MoveGroupView moveGroupView)
        {
            if (ModelState.IsValid)
            {
                Group group = repository.Groups.FirstOrDefault(g => g.name == moveGroupView.Name);
                Group parentGroup = repository.Groups.FirstOrDefault(g => g.dn == moveGroupView.selectedDn);

                if (group != null && parentGroup != null)
                {
                    if (repository.Move(group, parentGroup))
                    {
                        TempData["message-success"] = string.Format("Move Group '{0}' success!", moveGroupView.Name);
                    }
                    else
                    {
                        TempData["message-fail"] = string.Format("Move group '{0}' fail!", moveGroupView.Name);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["message-fail"] = string.Format("Group '{0}' didn't exist!", moveGroupView.Name);
                    return View(moveGroupView);
                }
            }
            else
            {
                return View(moveGroupView);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string Name)
        {
            if (Name == "People" || Name == "AM")
            {
                TempData["message-fail"] = string.Format("Group '{0}' can't be modified", Name);
                return RedirectToAction("Index", "Home");
            }

            Group group = repository.Groups.FirstOrDefault(g => g.name == Name);
            EditGroupView editGroup = new EditGroupView();
            
            editGroup.Name = group.name;
            editGroup.Dn = group.dn;
            editGroup.Parent = group.parent;
            
            return View(editGroup);
        }

        [HttpPost]
        public ActionResult Edit(EditGroupView editGroupView)
        {
            if (ModelState.IsValid)
            {
                Group editGroup = repository.Groups.FirstOrDefault(g => g.dn == editGroupView.Dn);
                
                if (editGroup != null)
                {
                    editGroup.name = editGroupView.Name;

                    if (repository.Edit(editGroup))
                    {
                        TempData["message-success"] = string.Format("Edit group '{0}' success", editGroup.name);
                    }
                    else
                    {
                        TempData["message-fail"] = string.Format("Edit group '{0}' fail", editGroup.name);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["message-fail"] = string.Format("Group '{0}' didn't exist!", editGroupView.Name);
                    return View(editGroupView);
                }
            }
            else
            {
                return View(editGroupView);
            }
            
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string Name)
        {
            Group deletedGroup = repository.Groups.FirstOrDefault(g => g.name == Name);
            if (repository.isHasChild(deletedGroup.dn))
            {
                TempData["message-fail"] = string.Format("Can't delete group has child");
            }
            else
            {
                deletedGroup = repository.Delete(Name);
                if (deletedGroup != null)
                {
                    TempData["message-success"] = string.Format("Delete group '{0}' success", Name);
                }
                else
                {
                    TempData["message-fail"] = string.Format("Delete group '{0}' fail", Name);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ExportToExcel(string Name)
        {
            Group group = repository.Groups.FirstOrDefault(g => g.name == Name);
            
            if (group != null)
            {
                LDAPHelper helper = new LDAPHelper();
                if (helper.ExportGroupToExcel(group))
                {
                    TempData["message-success"] = String.Format("Export group '{0}' success!", group.name);
                }
                else
                {
                    TempData["message-fail"] = String.Format("Export group '{0}' fail!", group.name);
                }
            }
            else
            {
                TempData["message-fail"] = String.Format("Group '{0}' didn't exist!", Name);
            }

            return RedirectToAction("Index", "Home");
        }

        public JsonResult isExists(string Name)
        {
            return Json(!repository.isExists(Name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult isHasChild(string Dn)
        {
            return Json(!repository.isHasChild(Dn), JsonRequestBehavior.AllowGet);
        }
    }
}