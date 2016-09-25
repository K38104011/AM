using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountManagement.Models
{
    public class AccountModel
    {
        //check hợp lệ của username
        [RegularExpression(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+.[A-Z]{2,4}", ErrorMessage = @"Username is a email address")]

        // username là bắt buộc nhập
        [Required(ErrorMessage = "Username is required")]
        //kiểm tra xem username đã trước đó chưa
        [Remote("isAccountExists", "Account", ErrorMessage = "Email already exists")]
        public string Username { get; set; }

        [StringLength(30, ErrorMessage = "Group name must be in range 6 to 30 characters", MinimumLength = 6)]
        public string pw { get; set; }
    }
}