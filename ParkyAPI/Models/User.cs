using ParkyAPI.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [ValidEmailDomain(allowedDomain:"mail.com", ErrorMessage = "Your domain is not correct!")]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 30,MinimumLength = 6, ErrorMessage = "Your Password must be between 6 and 30 characters.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }

        [NotMapped]
        public string Token { get; set; }
    }
}
