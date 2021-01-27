using ParkyAPI.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Models.DTOs
{
    public class AuthenticateDTO
    {
        [Required]
        [EmailAddress]
        [ValidEmailDomain(allowedDomain: "mail.com", ErrorMessage = "Your domain is not correct!")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 30, MinimumLength = 6, ErrorMessage = "Your Password must be between 6 and 30 characters.")]
        public string Password { get; set; }
    }
}
