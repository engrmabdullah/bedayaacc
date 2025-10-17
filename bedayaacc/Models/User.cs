using System;
using System.ComponentModel.DataAnnotations;

namespace bedayaacc.Models
{
    /// <summary>
    /// User entity model
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool AcceptMarketing { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public string FullName => $"{FirstName} {LastName}";
    }

    /// <summary>
    /// Register model for user registration
    /// </summary>
    public class RegisterModel
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم الأول يجب أن يكون أقل من 50 حرف")]
        [Display(Name = "الاسم الأول")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        [StringLength(50, ErrorMessage = "اسم العائلة يجب أن يكون أقل من 50 حرف")]
        [Display(Name = "اسم العائلة")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "يرجى إدخال بريد إلكتروني صحيح")]
        [StringLength(100, ErrorMessage = "البريد الإلكتروني يجب أن يكون أقل من 100 حرف")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "يرجى إدخال رقم هاتف صحيح")]
        [Display(Name = "رقم الهاتف")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
            ErrorMessage = "كلمة المرور يجب أن تحتوي على حرف كبير وحرف صغير ورقم ورمز خاص")]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("Password", ErrorMessage = "كلمة المرور غير متطابقة")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "يجب الموافقة على الشروط والأحكام")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "يجب الموافقة على الشروط والأحكام")]
        public bool AcceptTerms { get; set; }

        public bool AcceptMarketing { get; set; }
    }

    /// <summary>
    /// Login model
    /// </summary>
    public class LoginModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "يرجى إدخال بريد إلكتروني صحيح")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// Registration result
    /// </summary>
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string>? Errors { get; set; }
    }

    /// <summary>
    /// Login result
    /// </summary>
    public class LoginResult
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>
    /// Email verification token
    /// </summary>
    public class EmailVerificationToken
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Password reset token
    /// </summary>
    public class PasswordResetToken
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// User activity log
    /// </summary>
    public class UserActivityLog
    {
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// OAuth provider
    /// </summary>
    public class OAuthProvider
    {
        public int OAuthProviderId { get; set; }
        public int UserId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}