using System;

namespace bedayaacc.Models
{
    public class StudentProfileDto
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }

        public bool? AcceptMarketing { get; set; }
    }

    public class DashboardCountersDto
    {
        public int TotalExamsAvailable { get; set; }
        public int TotalAttempts { get; set; }
        public int CompletedAttempts { get; set; }
        public int PassedAttempts { get; set; }
        public int FailedAttempts { get; set; }
        public decimal? AvgPercentageAll { get; set; }
        public decimal? AvgPercentageLast10 { get; set; }
        public decimal? BestPercentage { get; set; }
        public int InProgressCount { get; set; }
        public DateTime? LastActivityAt { get; set; }
    }

    public class RecentActivityItemDto
    {
        public string EventType { get; set; } = ""; // AttemptSubmitted, AttemptStarted, AccessGranted
        public int ExamId { get; set; }
        public int? AttemptId { get; set; }
        public DateTime EventAt { get; set; }
        public bool? IsPassed { get; set; }
        public decimal? Percentage { get; set; }
        public string? ExamTitleAr { get; set; }
        public string? ExamTitleEn { get; set; }
    }

    public class UpcomingExamDto
    {
        public int ExamId { get; set; }
        public string? ExamTitleAr { get; set; }
        public string? ExamTitleEn { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DurationMinutes { get; set; }
        public decimal? PassingMarks { get; set; }
        public decimal? TotalMarks { get; set; }
        public int? MaxAttempts { get; set; }
        public bool RequirePassword { get; set; }
        public bool IsPublic { get; set; }
        public int AttemptCountForUser { get; set; }
        public bool HasReachedMaxAttempts { get; set; }
        public bool CanAttemptNow { get; set; }
    }

    public class InProgressAttemptDto
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public string? ExamTitleAr { get; set; }
        public string? ExamTitleEn { get; set; }
        public DateTime? StartedAt { get; set; }
        public int? TimeSpentMinutes { get; set; }
        public int? DurationMinutes { get; set; }
        public int? TimeLeftMinutes { get; set; }
        public string? Status { get; set; }
    }

    public class PassedCertificateDto
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public string? ExamTitleAr { get; set; }
        public string? ExamTitleEn { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public decimal? TotalMarks { get; set; }
    }

    public sealed class ExamProgressItemDto
    {
        public int ExamId { get; set; }
        public string? ExamTitleAr { get; set; }
        public string? ExamTitleEn { get; set; }
        public int AttemptCount { get; set; }
        public int CompletedCount { get; set; }
        public int PassedCount { get; set; }
        public decimal? BestPercentage { get; set; }
        public DateTime? LastSubmittedAt { get; set; }
        public DateTime? LastStartedAt { get; set; }
        public string? LastStatus { get; set; }
        public decimal? LastPercentage { get; set; }
    }

    public class PaymentItemVm
    {
        public int PaymentId { get; set; }
        public DateTime PaidAt { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Method { get; set; } = "";   // مثلاً: Visa, Wallet, Cash
        public string Status { get; set; } = "";   // Paid / Pending / Failed
        public string Reference { get; set; } = ""; // رقم مرجعي/TxnId
    }

    public class StudentDashboardVm
    {
        public StudentProfileDto? Profile { get; set; }
        public DashboardCountersDto? Counters { get; set; }
        public List<RecentActivityItemDto> RecentActivity { get; set; } = new();
        public List<UpcomingExamDto> UpcomingExams { get; set; } = new();
        public List<InProgressAttemptDto> InProgress { get; set; } = new();
        public List<PassedCertificateDto> PassedCertificates { get; set; } = new();
        public List<ExamProgressItemDto> ExamProgress { get; set; } = new();
        public List<PaymentItemVm> Payments { get; set; } = new();

    }
}
