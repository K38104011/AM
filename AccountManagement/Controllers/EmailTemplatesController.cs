using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AccountManagement.EmailTemplateModel;
using AccountManagement.Abstract;
using AccountManagement.Concrete;

namespace AccountManagement.Controllers
{
     [Authorize(Roles = "Admin")]
    public class EmailTemplatesController : Controller
    {
        private IEmailTemplateRepository repository;

        public EmailTemplatesController():this(new EmailTemplateRepository()){
        }
        public EmailTemplatesController(IEmailTemplateRepository repo)
        {
            this.repository = repo;
        }

        // GET: EmailTemplates/Add
        public ActionResult Add()
        {
            AccountManagement.Models.EmailTemplateModel model = new AccountManagement.Models.EmailTemplateModel();
            return View(model);
        }

        // POST: EmailTemplates/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Id,Name,ToEmail,CcEmail,Subject,Description")]AccountManagement.Models.EmailTemplateModel model)
        {
            if (ModelState.IsValid)
            {
                EmailTemplate emailTemplate = new EmailTemplate()
                {
                    Name = model.Name,
                    ToEmail = model.ToEmail,
                    CcEmail = model.CcEmail,
                    Subject = model.Subject,
                    Description = model.Description
                };
                repository.Create(emailTemplate);
                TempData["message-success"] = string.Format("Add template '{0}' success!", emailTemplate.Name);
                return RedirectToAction("Index", "Home");
            }
            TempData["message-fail"] = string.Format("Add template '{0}' fail!", model.Name);
            return View(model);
        }

        // GET: EmailTemplates/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                 AccountManagement.Models.EditEmailTemplateModel modelt = new AccountManagement.Models.EditEmailTemplateModel();
                 modelt.lstEmailTemplate = new SelectList(repository.emailTemplates.ToList(), "Id", "Name", modelt.Id);
               return View(modelt);
            }
            EmailTemplate emailTemplate = repository.Find(id);
            if (emailTemplate == null)
            {
                TempData["message-fail"] = string.Format("Template didn't exist!");
                return RedirectToAction("Index", "Home");
            }
            TempData["message"] = id;
            AccountManagement.Models.EditEmailTemplateModel model = new AccountManagement.Models.EditEmailTemplateModel()
            {
                Id = emailTemplate.Id,
                Name = emailTemplate.Name,
                ToEmail = emailTemplate.ToEmail,
                CcEmail = emailTemplate.CcEmail,
                Subject = emailTemplate.Subject,
                Description = emailTemplate.Description
            };
            model.lstEmailTemplate = new SelectList(repository.emailTemplates.ToList(), "Id", "Name", model.Id);
            return View(model);
        }

        // POST: EmailTemplates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountManagement.Models.EditEmailTemplateModel model)
        {
            if (ModelState.IsValid)
            {
                EmailTemplate emailTemplate = new EmailTemplate()
                {
                    Id = model.Id,
                    Name = model.Name,
                    ToEmail = model.ToEmail,
                    CcEmail = model.CcEmail,
                    Subject = model.Subject,
                    Description = model.Description
                };
                repository.Edit(emailTemplate);
                TempData["message-success"] = string.Format("Edit template '{0}' success", emailTemplate.Name);
                return RedirectToAction("Index", "Home");
            }
            TempData["message-fail"] = string.Format("Edit template '{0}' fail", model.Name);
            return View(model);
        }

        // GET: EmailTemplates/Delete/5
        public ActionResult Delete(long? id)
        {
            AccountManagement.Models.DeleteEmailTemplateModel model = new AccountManagement.Models.DeleteEmailTemplateModel()
            {
                lstEmailTemplate = new SelectList(repository.emailTemplates.ToList(), "Id", "Name")
            };
            return View(model); 
        }

        // POST: EmailTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            EmailTemplate emailTemplate = repository.Find(id);
            if (emailTemplate == null)
            {
                TempData["message-fail"] = string.Format("Template didn't exist!");
                return View();
            }
            repository.DeleteDeleteConfirmed(emailTemplate);
            TempData["message-success"] = string.Format("Delete template '{0}' success", emailTemplate.Name);
            return RedirectToAction("Index", "Home");
        }

        public JsonResult isExists(string Name)
        {
            return Json(!repository.emailTemplates.Any(et => et.Name == Name), JsonRequestBehavior.AllowGet);
        }
    }
}
