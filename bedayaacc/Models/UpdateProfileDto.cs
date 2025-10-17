using System;
using System.ComponentModel.DataAnnotations;

namespace bedayaacc.Models
{
    /// <summary>
    /// بيانات تعديل ملف الطالب.
    /// الحقول (nullable) يتم تجاهلها عند التحديث إذا كانت null.
    /// </summary>
    public sealed class UpdateProfileDto
    {
        [Required]
        public int UserId { get; set; }

        // أساسية
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [EmailAddress, MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// موافقة على الرسائل التسويقية (افتراضي false).
        /// </summary>
        public bool AcceptMarketing { get; set; } = false;

        // اختياري: لو هتدعم صورة للملف الشخصي
        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }
    }
}
