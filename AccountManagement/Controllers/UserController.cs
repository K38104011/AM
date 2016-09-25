using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountManagement.Infrastructure;
using AccountManagement.Abstract;
using AccountManagement.Concrete;
using System.Collections;
using System.Web.UI.WebControls;
using System.Net.Mime;

namespace AccountManagement.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private IGroupRepository groupRepository;
        private IUserRepository userRepository;

        public UserController() : this(new LDAPGroupRepository(), new LDAPUserRepository()) { }

        public UserController(IGroupRepository groupRepo, IUserRepository userRepo)
        {
            this.groupRepository = groupRepo;
            this.userRepository = userRepo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ViewResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(User[] users)
        {
            bool success = true;
            User errorUser = new User();

            if (users != null)
            {
                foreach (User u in users)
                {
                    if (!userRepository.Create(u))
                    {
                        success = false;
                        errorUser = u;
                        break;
                    }
                }
            }
            else
            {
                TempData["message-fail"] = string.Format("Cannot add null user");
            }

            if (success)
            {
                TempData["message-success"] = string.Format("Add users success!");
                return Json(Url.Action("Index", "Home"));
            }
            else
            {
                TempData["message-fail"] = string.Format("Add user '{0}' fail!", errorUser.Account);
            }

            return Json(Url.Action("Add", "User"));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Move()
        {
            MoveUserView modelView = new MoveUserView()
            {
                _groupList = groupRepository.Groups.OrderBy(g => g.name).ToList<Group>()
            };
            return View(modelView);
        }

        [HttpPost]
        public ActionResult Move(MoveUserView model)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Account = model.Account,
                    Dn = model.Dn
                };
                Group group = new Group()
                {
                    dn = model.selectedDn
                };
                if (userRepository.Move(user, group))
                {
                    TempData["message-success"] = string.Format("Move User '{0}' success!", model.Account);
                }
                else
                {
                    TempData["message-fail"] = string.Format("Move User '{0}' fail", model.Account);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditUser(String Name)
        {
            User user = userRepository.Users.FirstOrDefault(u => u.Account == Name);
            EditUserView editUserView = new EditUserView();
            editUserView.Email = user.Email;
            editUserView.FirstName = user.FirstName;
            editUserView.LastName = user.LastName;
            editUserView.Phone = user.Phone;
            editUserView.ParentGroup = user.ParentGroup;
            editUserView.Dn = user.Dn;
            editUserView.Account = user.Account;
            return View(editUserView);
        }

        [HttpPost]
        public ActionResult EditUser(EditUserView editUserView)
        {
            bool isEmailExist = userRepository.Users.Any(u => u.Email == editUserView.Email && u.Dn != editUserView.Dn);

            if (isEmailExist)
            {
                ModelState.AddModelError("", "Email is already used!");
                return View(editUserView);
            }
            if (ModelState.IsValid)
            {
                User user = userRepository.Users.FirstOrDefault(u => u.Account == editUserView.Account);

                if (user != null)
                {
                    user.Email = editUserView.Email;
                    user.FirstName = editUserView.FirstName;
                    user.LastName = editUserView.LastName;
                    user.Phone = editUserView.Phone;

                    if (userRepository.Edit(user))
                    {
                        TempData["message-success"] = string.Format("Edit user '{0}' success", user.Account);
                    }
                    else
                    {
                        TempData["message-fail"] = string.Format("Edit user '{0}' fail", user.Account);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["message-fail"] = string.Format("User '{0}' doesn't exist", editUserView.Account);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View(editUserView);
            }
        }


        public ActionResult ChangePassword()
        {
            string name = HttpContext.User.Identity.Name;
            User user = userRepository.Users.FirstOrDefault(u => u.Account == name);
            ChangePasswordView changePW = new ChangePasswordView();

            changePW.Account = user.Account;
            changePW.Dn=user.Dn;

            return View(changePW);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordView changePW)
        {
                User user = userRepository.Users.FirstOrDefault(u => u.Account == changePW.Account);
                user.Password = changePW.newPW;

                string hashedOldPassword = "";
                using (System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256Managed.Create())
                {
                    hashedOldPassword = String.Join("", hash
                        .ComputeHash(System.Text.Encoding.UTF8.GetBytes(changePW.Password))
                        .Select(item => item.ToString("x2")));
                }

                bool userValid = userRepository.Users.Any(m =>m.Account == changePW.Account
                    && m.Password == hashedOldPassword);

                if (userValid)
                {
                    if (userRepository.Edit(user))
                    {
                        TempData["message-success"] = string.Format("Change password success");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The password provided is incorrect.");
                    return View(changePW);
                }
            
        }

        public ActionResult EditProfile()
        {
            string name = HttpContext.User.Identity.Name;
            User user = userRepository.Users.FirstOrDefault(u => u.Account == name);
            EditProfileView editProfileView = new EditProfileView();
            editProfileView.Email = user.Email;
            editProfileView.FirstName = user.FirstName;
            editProfileView.LastName = user.LastName;
            editProfileView.Phone = user.Phone;
            editProfileView.ParentGroup = user.ParentGroup;
            editProfileView.Dn = user.Dn;
            return View(editProfileView);
        }

        [HttpPost]
        public ActionResult EditProfile(EditProfileView editProfileView)
        {
            bool isEmailExist = userRepository.Users.Any(u => u.Email == editProfileView.Email && u.Dn != editProfileView.Dn);

            if (isEmailExist)
            {
                ModelState.AddModelError("", "Email is already used!");
                return View(editProfileView);
            }

            if (ModelState.IsValid)
            {
                User user = userRepository.Users.FirstOrDefault(u => u.Dn == editProfileView.Dn);
                
                if (user != null)
                {

                    user.Email = editProfileView.Email;
                    user.FirstName = editProfileView.FirstName;
                    user.LastName = editProfileView.LastName;
                    user.Phone = editProfileView.Phone;

                    if (userRepository.Edit(user))
                    {
                        TempData["message-success"] = string.Format("Edit user '{0}' success", user.Account);
                    }
                    else
                    {
                        TempData["message-fail"] = string.Format("Edit user '{0}' fail", user.Account);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["message-fail"] = string.Format("User '{0}' doesn't exist", user.Account);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View(editProfileView);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string Name)
        {
            List<User> lst = userRepository.Users.ToList();
            User deletedUser = userRepository.Users.FirstOrDefault(u => u.Account == Name);
            if (deletedUser == null)
            {
                TempData["message-fail"] = string.Format("Can't delete user");
            }
            else
            {
                if (userRepository.Delete(deletedUser))
                {
                    TempData["message-success"] = string.Format("Delete user '{0}' success", Name);
                }
                else
                {
                    TempData["message-fail"] = string.Format("Delete user '{0}' fail", Name);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public JsonResult isEmailExists(string email)
        {
            return Json(userRepository.Users.Any(u => u.Email == email), JsonRequestBehavior.AllowGet);
        }

        public JsonResult isAccountExists(string account)
        {
            return Json(userRepository.Users.Any(user => user.Account == account), JsonRequestBehavior.AllowGet);
        }
    }
}