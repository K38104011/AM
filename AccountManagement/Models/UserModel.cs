using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountManagement.Models
{
    public class UserModel
    {
	[Required(ErrorMessage = "ParentGroup is required!")]
        public string ParentGroup { get; set; }
        [Display(Name = "LDAP")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Save to LDAP is required!")]
        public bool saveToLdap { get; set; }
        [Display(Name = "SVNACM")]
        public bool saveToSvnacm { get; set; }
        public string groupSvn { get; set; }
        public string pjSvn { get; set; }
        [Display(Name = "RedMind")]
        public bool saveToRedmind { get; set; }
        public string groupRm { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}