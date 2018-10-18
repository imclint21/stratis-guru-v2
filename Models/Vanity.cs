using System;
using System.ComponentModel.DataAnnotations;

namespace Stratis.Guru.Models
{
    public class Vanity
    {
        public DateTime CreationDatetime { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "The prefix is required.")]
        [StringLength(4, MinimumLength = 1, ErrorMessage = "You connat overflow the size.")]
        public string Prefix { get; set; }
        
        [Required(ErrorMessage = "The email address is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}