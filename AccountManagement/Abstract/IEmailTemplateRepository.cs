using AccountManagement.EmailTemplateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManagement.Abstract
{
    public interface IEmailTemplateRepository
    {
        IQueryable<EmailTemplate> emailTemplates { get; set; }
        void Create(EmailTemplate model);
        void Edit(EmailTemplate model);
        EmailTemplate Find(long? id);
        void DeleteDeleteConfirmed(EmailTemplate model);
    }
}