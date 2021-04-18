using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Smpt4devSample.WebApp.Models
{
    public class ContactFormViewModel
    {
        [Required, DisplayName("First Name")]
        public string FirstName { get; set; }
        
        [Required, DisplayName("Last Name")]
        public string LastName { get; set; }
        
        [Required, DataType(DataType.EmailAddress), DisplayName("Email Address")]
        public string EmailAddress { get; set; }
        
        [Required]
        public string Question { get; set; }
    }
}