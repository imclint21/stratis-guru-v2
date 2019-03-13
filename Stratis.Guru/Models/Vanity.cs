using System;
using System.ComponentModel.DataAnnotations;

namespace Stratis.Guru.Models
{
    public class Vanity
    {
        public DateTime CreationDatetime { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "The prefix is required.")]
        // [DataType(DataType.)]
        [StringLength(4, MinimumLength = 1, ErrorMessage = "4 chars is the maximum allowed prefix.")]
        public string Prefix { get; set; }
        
        // [Required(ErrorMessage = "The email address is required.")]
        // [EmailAddress]
        // public string Email { get; set; }
    }
}