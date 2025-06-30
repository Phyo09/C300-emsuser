using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace C300.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty!")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please write some feedback!")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "You cannot type more than 1000 characters.")]


        //[StringLength(5000, MinimumLength = 1, ErrorMessage = "Name has to be 20 to 5000 characters long.")]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Feedback Type is required!")]
        public int FeedbackType { get; set; }

        public DateTime Datetime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email cannot be empty!")]
        //[DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address!")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]

        public string Email { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = " cannot be empty!")]
        public string Reply { get; set; }

        //[Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Not a valid phone number")]
        [RegularExpression(@"^[89]\d{7}$", ErrorMessage = "Not a valid phone number")]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Contact { get; set; }

        public DateTime? SolvedTime { get; set; }
        public string Solvedby { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Subject cannot be empty!")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Subject shouldn'be not more than 300 characters")]


        public string Subject { get; set; }
    }
}