using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountManagement.Abstract;
using AccountManagement.EmailTemplateModel;

namespace AccountManagement.Concrete
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private EmailTemplateEntities context = new EmailTemplateEntities();

        public IQueryable<EmailTemplate> emailTemplates
        {
            get { return context.EmailTemplates; }
            set { emailTemplates = value; }
        }

        public void Create(EmailTemplate model)
        {
            context.EmailTemplates.Add(model);
            context.SaveChanges();
        }

        public void Edit(EmailTemplate model)
        {
            context.Entry(model).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        public EmailTemplate Find(long? id)
        {
            return context.EmailTemplates.Find(id);
        }

        public void DeleteDeleteConfirmed(EmailTemplate model)
        {
            context.EmailTemplates.Remove(model);
            context.SaveChanges();
        }
    }
}