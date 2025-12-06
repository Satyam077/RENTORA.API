using RENTORA.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class EmailTemplateCreateDTO
    {
        [MaxLength(500)]
        public string? Tokens { get; set; }

        [Required]
        [MaxLength(250)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string EmailSubject { get; set; } = string.Empty;

        [Required]
        public string EmailBody { get; set; } = string.Empty;

        [Required]
        public string ApplicableFor { get; set; }
    }

    public class EmailTemplateUpdateDTO
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Tokens { get; set; }

        [Required]
        [MaxLength(250)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string EmailSubject { get; set; } = string.Empty;

        [Required]
        public string EmailBody { get; set; } = string.Empty;

        [Required]
        public string ApplicableFor { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class EmailTemplateResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? Tokens { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string EmailSubject { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;
        public string ApplicableFor { get; set; }
        public string ApplicableForName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
