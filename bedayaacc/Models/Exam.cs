using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bedayaacc.Models
{
    // ==================== Exam Models ====================

    public class Exam
    {
        public int ExamId { get; set; }
        public string ExamTitleAr { get; set; } = default!;
        public string? ExamTitleEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public int? CategoryId { get; set; }
        public int? CourseId { get; set; }
        public int InstructorId { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassingMarks { get; set; }
        public int? MaxAttempts { get; set; }
        public int? TotalQuestions { get; set; }
        public bool ShowResultsImmediately { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleOptions { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublic { get; set; }
        public bool RequirePassword { get; set; }
        public string? ExamPassword { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public bool ShowAnswerDuringExam { get; set; }
        public bool ShowExplanationDuringExam { get; set; }

        // جديد
        public decimal Price { get; set; }
        public bool IsFree => Price == 0;

        // احصائيات العرض الحالية
        public int TotalAttempts { get; set; }
        public int UniqueStudents { get; set; }
        public int UserAttempts { get; set; }
        // أسماء الفئات للعرض
        public string? CategoryNameAr { get; set; }


        // ==== جديد لعرض اسم المدرّس ====
        public string? InstructorName { get; set; }              // يُملأ بالـ SQL alias (مقترح)
        public string? InstructorFirstName { get; set; }         // احتياطي لو محتاجهم
        public string? InstructorLastName { get; set; }          // احتياطي لو محتاجهم

        // عرض افتراضي لو InstructorName فاضي
        public string DisplayInstructorName =>
            !string.IsNullOrWhiteSpace(InstructorName)
                ? InstructorName!
                : $"{InstructorFirstName} {InstructorLastName}".Trim();
    }


    public class ExamCategory
    {
        public int CategoryId { get; set; }
        public string CategoryNameAr { get; set; } = string.Empty;
        public string CategoryNameEn { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public int ExamId { get; set; }
        public int QuestionTypeId { get; set; }

        // Question Content
        public string QuestionTextAr { get; set; } = string.Empty;
        public string? QuestionTextEn { get; set; }
        public string? QuestionImageUrl { get; set; }

        // Grading
        public decimal Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        public string? CorrectAnswer { get; set; }

        // Display
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }

        // Status
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public string? TypeNameAr { get; set; }
        public string? TypeNameEn { get; set; }
        public string? TypeCode { get; set; }
        public bool AllowMultipleAnswers { get; set; }
        public bool RequiresOptions { get; set; }

        public string? ExplanationAr { get; set; }   // جديد
        public string? ExplanationEn { get; set; }   // جديد

        public List<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }

    public class QuestionType
    {
        public int QuestionTypeId { get; set; }
        public string TypeNameAr { get; set; } = string.Empty;
        public string TypeNameEn { get; set; } = string.Empty;
        public string TypeCode { get; set; } = string.Empty;
        public bool AllowMultipleAnswers { get; set; }
        public bool RequiresOptions { get; set; }
        public bool IsActive { get; set; }
    }

    public class QuestionOption
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public string OptionTextAr { get; set; } = string.Empty;
        public string? OptionTextEn { get; set; }
        public string? OptionImageUrl { get; set; }
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExamAttempt
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }

        // Attempt Info
        public int AttemptNumber { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? TimeSpentMinutes { get; set; }

        // Scoring
        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal Percentage { get; set; }
        public bool IsPassed { get; set; }

        // Status
        public string Status { get; set; } = "IN_PROGRESS";
        public bool IsCompleted { get; set; }

        // Metadata
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        // Navigation Properties
        public string? ExamTitleAr { get; set; }
        public string? ExamTitleEn { get; set; }
        public string? StudentName { get; set; }
        public bool ShowCorrectAnswers { get; set; }

        public List<ExamAnswer> Answers { get; set; } = new List<ExamAnswer>();
    }

    public class ExamAnswer
    {
        public int AnswerId { get; set; }
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }

        // Answer Content
        public int? SelectedOptionId { get; set; }
        public string? AnswerText { get; set; }

        // Grading
        public bool? IsCorrect { get; set; }
        public decimal MarksObtained { get; set; }

        // Feedback
        public string? InstructorFeedback { get; set; }
        public int? GradedBy { get; set; }
        public DateTime? GradedAt { get; set; }

        // Metadata
        public DateTime AnsweredAt { get; set; }
        public int TimeSpentSeconds { get; set; }

        // Navigation Properties
        public string? QuestionTextAr { get; set; }
        public string? QuestionTextEn { get; set; }
        public decimal QuestionMarks { get; set; }
        public string? TypeCode { get; set; }
        public string? SelectedOptionTextAr { get; set; }
        public string? SelectedOptionTextEn { get; set; }
        public bool SelectedOptionIsCorrect { get; set; }
    }

    // ==================== DTOs and View Models ====================

    public class CreateExamModel
    {
        [Required] public string ExamTitleAr { get; set; } = default!;
        public string? ExamTitleEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public int? CategoryId { get; set; }
        public int? CourseId { get; set; }
        [Range(1, int.MaxValue)] public int DurationMinutes { get; set; } = 60;
        [Range(1, 99999)] public decimal TotalMarks { get; set; } = 100;
        [Range(0, 99999)] public decimal PassingMarks { get; set; } = 50;
        public int? MaxAttempts { get; set; } = 1;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublic { get; set; } = true;
        public bool RequirePassword { get; set; }
        public string? ExamPassword { get; set; }
        public bool ShowResultsImmediately { get; set; } = true;
        public bool ShowCorrectAnswers { get; set; } = true;
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleOptions { get; set; }
        public bool ShowAnswerDuringExam { get; set; }
        public bool ShowExplanationDuringExam { get; set; }

        // جديد
        [Range(0, 1000000, ErrorMessage = "Price must be non-negative")]
        public decimal Price { get; set; } = 0;
    }

    public class UpdateExamModel : CreateExamModel
    {
        [Required] public int ExamId { get; set; }
    }


    public class QuestionRevealDto
    {
        public int QuestionId { get; set; }
        public string? ExplanationAr { get; set; }
        public string? ExplanationEn { get; set; }
        public bool ShowAnswer { get; set; }
        public List<int> CorrectOptionIds { get; set; } = new();
    }

    public class CreateQuestionModel
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        public int QuestionTypeId { get; set; }

        [Required(ErrorMessage = "نص السؤال بالعربي مطلوب")]

        public string QuestionTextAr { get; set; } = string.Empty;


        public string? QuestionTextEn { get; set; }


        public string? QuestionImageUrl { get; set; }

        [Required]
        [Range(0.1, 100)]
        public decimal Marks { get; set; } = 1;

        [Range(0, 100)]
        public decimal NegativeMarks { get; set; } = 0;

        public string? CorrectAnswer { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public List<CreateQuestionOptionModel> Options { get; set; } = new List<CreateQuestionOptionModel>();

        public string? ExplanationAr { get; set; }   // جديد
        public string? ExplanationEn { get; set; }   // جديد
    }

    public class CreateQuestionOptionModel
    {
        [Required(ErrorMessage = "نص الخيار بالعربي مطلوب")]
        [StringLength(1000)]
        public string OptionTextAr { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? OptionTextEn { get; set; }

        [StringLength(500)]
        public string? OptionImageUrl { get; set; }

        public bool IsCorrect { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class StartExamModel
    {
        [Required]
        public int ExamId { get; set; }

        public string? ExamPassword { get; set; }
    }

    public class SubmitAnswerModel
    {
        [Required]
        public int AttemptId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public int? SelectedOptionId { get; set; }

        public string? AnswerText { get; set; }

        public int TimeSpentSeconds { get; set; }
    }

    public class ExamResultsModel
    {
        public ExamAttempt Attempt { get; set; } = new ExamAttempt();
        public List<QuestionResultModel> Questions { get; set; } = new List<QuestionResultModel>();

        // ===== جديد: نتائج مبدئية بدون المقالية =====
        public decimal AutoMaxMarks { get; set; }              // مجموع درجات الأسئلة القابلة للتصحيح الآلي
        public decimal AutoObtainedMarks { get; set; }         // الدرجات المجمعة تلقائياً
        public decimal AutoProvisionalPercentage { get; set; } // نسبة مبدئية = AutoObtained / AutoMax
        public int PendingManualCount { get; set; }            // عدد الأسئلة المقالية (قيد المراجعة)
        public decimal PendingManualMarks { get; set; }        // مجموع درجاتها
    }

    public class QuestionResultModel
    {
        public Question Question { get; set; } = new Question();
        public ExamAnswer? UserAnswer { get; set; }
        public List<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }

    // ==================== Result Models ====================

    public class ExamOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        public int AttemptId { get; set; }
        public object? Data { get; set; }
    }

    public class ExamAccessResult
    {
        public bool HasAccess { get; set; }
        public bool IsScheduled { get; set; }
        public bool HasStarted { get; set; }
        public bool HasEnded { get; set; }
        public bool MaxAttemptsReached { get; set; }
        public bool RequiresPassword { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}