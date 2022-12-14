using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterAccountDto
    {
        [Required]
        public string Username { get; set; }
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 8 chracters")]
        [Required]
        public string Password { get; set; }
    }
}