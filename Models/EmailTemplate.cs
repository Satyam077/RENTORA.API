using RENTORA.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models
{
    public class EmailTemplate : BaseEntity
    {
        [MaxLength(500)]
        public string Tokens { get; set; }

        [Required]
        [MaxLength(250)]
        public string TemplateName { get; set; }

        [Required]
        [MaxLength(250)]
        public string EmailSubject { get; set; }

        [Required]
        public string EmailBody { get; set; }

        public string ApplicableFor { get; set; }
    }
}
