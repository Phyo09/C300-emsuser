using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace C300.Models
{
    public partial class Emsuser
    {
        public Emsuser()
        {
            Comment = new HashSet<Comment>();
            Topic = new HashSet<Topic>();
        }

        public int UserId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty!")]
        public string Name { get; set; }

        public string Role { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty!")]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\W)\S{8,15}$", ErrorMessage = "Invalid password format")]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password and password do not match!")]
        public string ConfirmPassword { get; set; }


        public string Picture { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of birth cannot be empty!")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Country cannot be empty!")]
        public string Country { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email cannot be empty!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address!")]
        public string Email { get; set; }
        public Guid? ActivationCode { get; set; }
        public bool IsVerified { get; set; }
        public string ResetPasswordCode { get; set; }
        public string Status { get; set; }
        public int? Phone { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Topic> Topic { get; set; }
    }
}


