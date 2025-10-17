using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bedayaacc.Models;
using bedayaacc.Repositories;

namespace bedayaacc.Services
{
    public interface IExamService
    {
        // Exam Management
        Task<ExamOperationResult> CreateExamAsync(CreateExamModel model, int instructorId);
        Task<ExamOperationResult> UpdateExamAsync(UpdateExamModel model, int userId);
        Task<bool> DeleteExamAsync(int examId, int userId);

        Task<Exam?> GetExamAsync(int examId, int? userId = null);
        Task<List<Exam>> GetAllExamsAsync(int? categoryId = null, int? courseId = null, int? instructorId = null, bool? isPublic = null, int page = 1, int pageSize = 10);
        Task<List<ExamCategory>> GetCategoriesAsync();
        Task<List<QuestionType>> GetQuestionTypesAsync();

        // Question Management
        Task<ExamOperationResult> AddQuestionAsync(CreateQuestionModel model);
        Task<List<Question>> GetExamQuestionsForTakingAsync(int examId, int attemptId);
        Task<Question?> GetQuestionAsync(int questionId);
        Task<bool> DeleteQuestionAsync(int questionId);

        // Exam Taking
        Task<ExamAccessResult> CheckAccessAsync(int examId, int userId, string? password = null);
        Task<ExamOperationResult> StartExamAsync(int examId, int userId, string? password = null, string? ipAddress = null, string? userAgent = null);
        Task<bool> SaveAnswerAsync(SubmitAnswerModel model);
        Task<ExamOperationResult> SubmitExamAsync(int attemptId, int userId);

        // Results & History
        Task<ExamResultsModel?> GetResultsAsync(int attemptId, int userId);
        Task<List<ExamAttempt>> GetUserAttemptsAsync(int userId, int? examId = null);
        Task<ExamAttempt?> GetAttemptAsync(int attemptId);

        // Statistics
        Task<ExamStatistics> GetExamStatisticsAsync(int examId);
        Task<UserExamStatistics> GetUserStatisticsAsync(int userId);


        Task<QuestionRevealDto?> RevealAnswerAsync(int attemptId, int questionId); // جديد

    }

    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IPaymentRepository _paymentRepository;

        public ExamService(IExamRepository examRepository, IPaymentRepository paymentRepository)
        {
            _examRepository = examRepository;
            _paymentRepository = paymentRepository;
        }

        // ==================== Exam Management ====================

        public async Task<ExamOperationResult> CreateExamAsync(CreateExamModel model, int instructorId)
        {
            if (string.IsNullOrWhiteSpace(model.ExamTitleAr))
                return new ExamOperationResult { Success = false, Message = "عنوان الاختبار مطلوب" };

            if (model.DurationMinutes <= 0)
                return new ExamOperationResult { Success = false, Message = "مدة الاختبار يجب أن تكون أكبر من صفر" };

            if (model.PassingMarks > model.TotalMarks)
                return new ExamOperationResult { Success = false, Message = "درجة النجاح يجب أن تكون أقل من أو تساوي الدرجة الكلية" };

            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate < model.StartDate)
                return new ExamOperationResult { Success = false, Message = "تاريخ الانتهاء يجب أن يكون بعد تاريخ البداية" };

            if (model.Price < 0)
                return new ExamOperationResult { Success = false, Message = "السعر لا يمكن أن يكون سالباً" };

            // لو مجاني: اجعله عامًا وألغ كلمة المرور
            if (model.Price == 0)
            {
                model.IsPublic = true;
                model.RequirePassword = false;
                model.ExamPassword = null;
            }

            return await _examRepository.CreateExamAsync(model, createdBy: instructorId, instructorId: instructorId);
        }

        public async Task<ExamOperationResult> UpdateExamAsync(UpdateExamModel model, int userId)
        {
            if (string.IsNullOrWhiteSpace(model.ExamTitleAr))
                return new ExamOperationResult { Success = false, Message = "عنوان الاختبار مطلوب" };

            if (model.DurationMinutes <= 0)
                return new ExamOperationResult { Success = false, Message = "مدة الاختبار يجب أن تكون أكبر من صفر" };

            if (model.PassingMarks > model.TotalMarks)
                return new ExamOperationResult { Success = false, Message = "درجة النجاح يجب أن تكون أقل من أو تساوي الدرجة الكلية" };

            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate < model.StartDate)
                return new ExamOperationResult { Success = false, Message = "تاريخ الانتهاء يجب أن يكون بعد تاريخ البداية" };

            if (model.Price < 0)
                return new ExamOperationResult { Success = false, Message = "السعر لا يمكن أن يكون سالباً" };

            if (model.Price == 0)
            {
                model.IsPublic = true;
                model.RequirePassword = false;
                model.ExamPassword = null;
            }

            return await _examRepository.UpdateExamAsync(model, userId);
        }

        public async Task<bool> DeleteExamAsync(int examId, int userId)
        {
            return await _examRepository.DeleteExamAsync(examId, userId);
        }


        public async Task<Exam?> GetExamAsync(int examId, int? userId = null)
        {
            return await _examRepository.GetExamByIdAsync(examId, userId);
        }

        public async Task<List<Exam>> GetAllExamsAsync(int? categoryId = null, int? courseId = null, int? instructorId = null, bool? isPublic = null, int page = 1, int pageSize = 10)
        {
            return await _examRepository.GetAllExamsAsync(categoryId, courseId, instructorId, isPublic, page, pageSize);
        }

        public async Task<List<ExamCategory>> GetCategoriesAsync()
        {
            return await _examRepository.GetExamCategoriesAsync();
        }

        public async Task<List<QuestionType>> GetQuestionTypesAsync()
        {
            return await _examRepository.GetQuestionTypesAsync();
        }

        // ==================== Question Management ====================

        public async Task<ExamOperationResult> AddQuestionAsync(CreateQuestionModel model)
        {
            // Validate question
            if (string.IsNullOrWhiteSpace(model.QuestionTextAr))
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "نص السؤال مطلوب"
                };
            }

            if (model.Marks <= 0)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "درجة السؤال يجب أن تكون أكبر من صفر"
                };
            }

            // Validate options for MCQ/TF questions
            var questionTypes = await GetQuestionTypesAsync();
            var questionType = questionTypes.Find(qt => qt.QuestionTypeId == model.QuestionTypeId);

            if (questionType != null && questionType.RequiresOptions && model.Options.Count == 0)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "هذا النوع من الأسئلة يتطلب خيارات"
                };
            }

            if (questionType != null && questionType.RequiresOptions)
            {
                var hasCorrectAnswer = model.Options.Exists(o => o.IsCorrect);
                if (!hasCorrectAnswer)
                {
                    return new ExamOperationResult
                    {
                        Success = false,
                        Message = "يجب تحديد الإجابة الصحيحة"
                    };
                }
            }

            return await _examRepository.AddQuestionAsync(model);
        }

        public async Task<List<Question>> GetExamQuestionsForTakingAsync(int examId, int attemptId)
        {
            var exam = await GetExamAsync(examId);
            if (exam == null)
                return new List<Question>();

            var questions = await _examRepository.GetExamQuestionsAsync(examId, exam.ShuffleQuestions);

            // Load options with shuffle setting
            foreach (var question in questions)
            {
                question.Options = await _examRepository.GetQuestionOptionsAsync(question.QuestionId, exam.ShuffleOptions);
            }

            return questions;
        }

        public async Task<Question?> GetQuestionAsync(int questionId)
        {
            return await _examRepository.GetQuestionByIdAsync(questionId);
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            return await _examRepository.DeleteQuestionAsync(questionId);
        }

        // ==================== Exam Taking ====================

        public async Task<ExamAccessResult> CheckAccessAsync2(int examId, int userId, string? password = null)
        {
            return await _examRepository.CheckExamAccessAsync(examId, userId, password);
        }

        public async Task<ExamAccessResult> CheckAccessAsync(int examId, int userId, string? password = null)
        {
            var exam = await _examRepository.GetExamByIdAsync(examId, userId);
            if (exam == null)
                return new ExamAccessResult { HasAccess = false, Message = "الاختبار غير موجود" };

            if (exam.Price == 0)
                return new ExamAccessResult { HasAccess = true };

            if (userId <= 0)
                return new ExamAccessResult { HasAccess = false, Message = "يلزم تسجيل الدخول لشراء هذا الاختبار" };

            var paidOrUnverified = await _paymentRepository.HasUserPaidExamAsync(examId, userId); // يشمل PAID_UNVERIFIED
            if (!paidOrUnverified)
                return new ExamAccessResult { HasAccess = false, Message = "يلزم شراء/رفع إيصال التحويل قبل البدء" };

            return await _examRepository.CheckExamAccessAsync(examId, userId, password);
        }


        public async Task<ExamOperationResult> StartExamAsync(int examId, int userId, string? password = null, string? ipAddress = null, string? userAgent = null)
        {
            // Check access first
            var accessResult = await CheckAccessAsync(examId, userId, password);

            if (!accessResult.HasAccess)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = accessResult.Message
                };
            }

            // Check if there's an ongoing attempt
            var attempts = await _examRepository.GetUserExamAttemptsAsync(userId, examId);
            var ongoingAttempt = attempts.Find(a => a.Status == "IN_PROGRESS" && !a.IsCompleted);

            if (ongoingAttempt != null)
            {
                return new ExamOperationResult
                {
                    Success = true,
                    AttemptId = ongoingAttempt.AttemptId,
                    Message = "لديك محاولة جارية، سيتم استكمالها"
                };
            }

            // Start new attempt
            return await _examRepository.StartExamAttemptAsync(examId, userId, ipAddress, userAgent);
        }

        public async Task<bool> SaveAnswerAsync(SubmitAnswerModel model)
        {
            // Validate attempt is still active
            var attempt = await _examRepository.GetAttemptByIdAsync(model.AttemptId);

            if (attempt == null || attempt.IsCompleted)
            {
                return false;
            }

            return await _examRepository.SubmitAnswerAsync(model);
        }

        public async Task<ExamOperationResult> SubmitExamAsync(int attemptId, int userId)
        {
            // Verify ownership
            var attempt = await _examRepository.GetAttemptByIdAsync(attemptId);

            if (attempt == null)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "المحاولة غير موجودة"
                };
            }

            if (attempt.UserId != userId)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "غير مصرح لك بتسليم هذا الاختبار"
                };
            }

            if (attempt.IsCompleted)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "تم تسليم هذا الاختبار مسبقاً"
                };
            }

            return await _examRepository.SubmitExamAsync(attemptId);
        }

        // ==================== Results & History ====================

        public async Task<ExamResultsModel?> GetResultsAsync(int attemptId, int userId)
        {
            var attempt = await _examRepository.GetAttemptByIdAsync(attemptId);

            if (attempt == null || attempt.UserId != userId)
            {
                return null;
            }

            return await _examRepository.GetExamResultsAsync(attemptId);
        }

        public async Task<List<ExamAttempt>> GetUserAttemptsAsync(int userId, int? examId = null)
        {
            return await _examRepository.GetUserExamAttemptsAsync(userId, examId);
        }

        public async Task<ExamAttempt?> GetAttemptAsync(int attemptId)
        {
            return await _examRepository.GetAttemptByIdAsync(attemptId);
        }

        // ==================== Statistics ====================

        public async Task<ExamStatistics> GetExamStatisticsAsync(int examId)
        {
            var exam = await GetExamAsync(examId);
            if (exam == null)
            {
                return new ExamStatistics();
            }

            // You can expand this to calculate more detailed statistics
            return new ExamStatistics
            {
                ExamId = examId,
                TotalAttempts = exam.TotalAttempts,
                UniqueStudents = exam.UniqueStudents,
                AverageScore = 0, // Calculate from database
                HighestScore = 0, // Calculate from database
                LowestScore = 0, // Calculate from database
                PassRate = 0 // Calculate from database
            };
        }

        public async Task<UserExamStatistics> GetUserStatisticsAsync(int userId)
        {
            var attempts = await GetUserAttemptsAsync(userId);

            var completedAttempts = attempts.FindAll(a => a.IsCompleted);
            var passedAttempts = completedAttempts.FindAll(a => a.IsPassed);

            return new UserExamStatistics
            {
                UserId = userId,
                TotalExamsTaken = attempts.Count,
                TotalExamsPassed = passedAttempts.Count,
                AverageScore = completedAttempts.Count > 0
                    ? completedAttempts.Average(a => (double)a.Percentage)
                    : 0,
                TotalTimeSpent = completedAttempts.Sum(a => a.TimeSpentMinutes ?? 0)
            };
        }

        public async Task<QuestionRevealDto?> RevealAnswerAsync(int attemptId, int questionId)
        {
            // ممكن تضيف شروط إضافية هنا (مثلاً: لازم يكون جاوب الأول)
            return await _examRepository.RevealAnswerAsync(attemptId, questionId);
        }

    }

    // ==================== Statistics Models ====================

    public class ExamStatistics
    {
        public int ExamId { get; set; }
        public int TotalAttempts { get; set; }
        public int UniqueStudents { get; set; }
        public decimal AverageScore { get; set; }
        public decimal HighestScore { get; set; }
        public decimal LowestScore { get; set; }
        public decimal PassRate { get; set; }
    }

    public class UserExamStatistics
    {
        public int UserId { get; set; }
        public int TotalExamsTaken { get; set; }
        public int TotalExamsPassed { get; set; }
        public double AverageScore { get; set; }
        public int TotalTimeSpent { get; set; }
    }
}