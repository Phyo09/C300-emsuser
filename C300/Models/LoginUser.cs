using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace C300.Models
{
   public class LoginUser
   {
      [Required(ErrorMessage = "Email address cannot be empty!")]
      public string Email { get; set; }

      [Required(ErrorMessage = "Password cannot be empty!")]
      [DataType(DataType.Password)]
      public string Password { get; set; }

   }
}
