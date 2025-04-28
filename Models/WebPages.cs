using System;
using System.ComponentModel.DataAnnotations;

namespace AI_WebsiteBuilder.Models
{
    public class WebPage
    {
        [Key]
        public int ID { get; set; } // The ID of this page

        [Required]
        public string? Member { get; set; } // Holds the email address of the user who created the page (User cannot edit this)

        [Required]
        [Display(Name = "Page Name")]
        public string? PageName { get; set; } // Website page name

        [Display(Name = "Page Description")]
        public string? PageDescription { get; set; } // Description details of the page

        [Display(Name = "Page Content")]
        public string? PageContent { get; set; } // Full HTML content of the page

        [Required]
        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Automatically set when creating the page
    }
}
