using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Compare = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace AccountManagement.Models
{
    public class User
    {
        public string ParentGroup { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        public string Dn { get; set; }
        public string ParentDn { get; set; }
    }
	
	public class MoveUserView
    {
        [HiddenInput(DisplayValue = false)]
        public string Dn { get; set; }
        public string Account { get; set; }
        public string selectedDn { get; set; }
        public List<Group> _groupList { get; set; }
        public IEnumerable<SelectListItem> GroupList
        {
            get { return new SelectList(_groupList, "dn", "name"); }
        }
    }
	
	public class EditProfileView
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Firstname must have no more than 50 characters!")]
        [RegularExpression(@"[a-zA-Z]{1,50}$", ErrorMessage = @"First name is not contains digit and special character")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "Lastname must have no more than 50 characters!")]
        [RegularExpression(@"[a-zA-Z]{1,50}$", ErrorMessage = @"Last name is not contains digit and special character")]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = @"Email: abc@example.com")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"[0]\d{9,10}$", ErrorMessage = @"Phone number range 10 to 11 digit and start with 0")]
        public string Phone { get; set; }

        [HiddenInput(DisplayValue =false)]
        public string Dn { get; set; }
        public string ParentGroup { get; set; }
    }
    public class EditUserView
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Firstname must have no more than 50 characters!")]
        [RegularExpression("[a-zA-Z]{1,50}$", ErrorMessage = "Firstname must not contain digits and special characters!")]
        [Required(ErrorMessage = "Firstname is required!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Lastname must have no more than 50 characters!")]
        [RegularExpression("[a-zA-Z]{1,50}$", ErrorMessage = "Lastname must not contain digits and special characters!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = @"Email: abc@example.com")]
        public string Email { get; set; }

    
        [RegularExpression(@"[0]\d{9,10}$", ErrorMessage = @"Phone number range 10 to 11 digit and start with 0")]
        public string Phone { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Dn { get; set; }

        public string ParentGroup { get; set; }

        public string Account { get; set; }
    }
    public class ChangePasswordView
    {

        public string Account { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(30,MinimumLength =6, ErrorMessage = "Password must be in range 6 to 30 characters")]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s)[0-9a-zA-Z!@#$%^&*()]*$", ErrorMessage = "The password must include a lowercase letter, an uppercase letter and a digit")]
        [Display(Name ="New Password")]
        [DataType(DataType.Password)]
        public string newPW { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("newPW", ErrorMessage = "The new password and confirmation password do not match.")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        public string confirmPW { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Dn { get; set; }
    }
    public class AddUserView
    {
        [Required(ErrorMessage = "ParentGroup is required")]
        public string ParentGroup { get; set; }
        [Range(typeof(bool), "false", "false", ErrorMessage = "SaveToLDAP is required!")]
        [Display(Name = "LDAP")]
        public bool saveToLdap { get; set; }
        [Display(Name = "SVNACM")]
        public bool saveToSvnacm { get; set; }
        [Display(Name = "RedMind")]
        public bool saveToRedmind { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Account")]
        public string Account { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Note")]
        public string Note { get; set; }
        public string Roles { get; set; }
        public string Dn { get; set; }
    }

    public class LoginView
    {
        [Required(ErrorMessage = "Please enter account")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}