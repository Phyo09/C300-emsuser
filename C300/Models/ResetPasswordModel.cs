using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;

namespace C300.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }

    }
}
