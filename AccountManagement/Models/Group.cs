using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountManagement.Models
{
    public class Group
    {
        public string name { get; set; }
        public string dn { get; set; }
        public string parent { get; set; }
        public string parentDn { get; set; }
    }

    public class EditGroupView
    {
        [RegularExpression(@"^[^\\/?%*:|""<>\.]+$", ErrorMessage = @"Group name must not contain \ / ? % * : | "" < >")]
        [StringLength(50, ErrorMessage = "Group name must be in range 1 to 50 characters", MinimumLength = 1)]
        [Required(ErrorMessage = "Group name is required")]
        [Remote("isExists", "Group", ErrorMessage = "Group name already exists")]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [Remote("isHasChild", "Group", ErrorMessage = "Can't edit group has child")]
        [HiddenInput(DisplayValue = false)]
        public string Dn { get; set; }
        [Display(Name = "Parent Group")]
        public string Parent { get; set; }
    }

    public class AddGroupView
    {
        [RegularExpression(@"^[^\\/?%*:|""<>\.]+$", ErrorMessage = @"Group name must not contain \ / ? % * : | "" < >")]
        [StringLength(50, ErrorMessage = "Group name must be in range 1 to 50 characters", MinimumLength = 1)]
        [Required(ErrorMessage = "Group name is required")]
        [Remote("isExists", "Group", ErrorMessage = "Group name already exists")]
        [Display(Name = "Group Name")]
        public string Name { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ParentDn { get; set; }
    }

    public class MoveGroupView
    {
        [Required(ErrorMessage = "Group name is required")]
        [Display(Name = "Current Group")]
        public string Name { get; set; }

        [Remote("isHasChild", "Group", ErrorMessage = "Can't move group has child!")]
        [HiddenInput(DisplayValue = false)]
        public string Dn { get; set; }

        public List<Group> _groupList { get; set; }

        public string selectedDn { get; set; }

        public IEnumerable<SelectListItem> GroupList
        {
            get { return new SelectList(_groupList, "dn", "name"); }
        }
    }
}