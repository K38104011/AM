using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountManagement.Models
{
    public class EmailTemplateModel
    {
        public long Id { get; set; }
        [RegularExpression(@"^[^\\/?%*:|""<>\.]+$", ErrorMessage = @"Template Name must not contain \ / ? % * : | "" < >")]
        [StringLength(50, ErrorMessage = "Template Name must be in range 1 to 50 characters", MinimumLength = 1)]
        [Required(ErrorMessage = "Template Name is required")]
        [Remote("isExists", "EmailTemplates", ErrorMessage = "Template Name already exists")]
        [Display(Name = "Template Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "To Email is required")]
        [Display(Name = "To")]
        public string ToEmail { get; set; }
        [Display(Name = "Cc Email")]
        public string CcEmail { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class EditEmailTemplateModel
    {
        public long Id { get; set; }
        [RegularExpression(@"^[^\\/?%*:|""<>\.]+$", ErrorMessage = @"Template Name must not contain \ / ? % * : | "" < >")]
        [StringLength(50, ErrorMessage = "Template Name must be in range 1 to 50 characters", MinimumLength = 1)]
        [Required(ErrorMessage = "Template Name is required")]
        [Display(Name = "Template Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "To Email is required")]
        [Display(Name = "To")]
        public string ToEmail { get; set; }
        [Display(Name = "Cc Email")]
        public string CcEmail { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Choose Template")]
        public SelectList lstEmailTemplate { get; set; }
    }

    public class DeleteEmailTemplateModel
    {
        public long Id { get; set; }
        public SelectList lstEmailTemplate { get; set; }
    }
}