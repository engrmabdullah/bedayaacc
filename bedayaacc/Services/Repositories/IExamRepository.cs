using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using bedayaacc.Models;

namespace bedayaacc.Repositories
{
    public interface IExamRepository
    {
        // Exam Management
        Task<ExamOperationResult> CreateExamAsync(CreateExamModel model, int createdBy, int instructorId);
        Task<ExamOperationResult> UpdateExamAsync(UpdateExamModel model, int updatedBy);
        Task<bool> DeleteExamAsync(int examId, int updatedBy);
        Task<Exam?> GetExamByIdAsync(int examId, int? userId = null);
        Task<List<Exam>> GetAllExamsAsync(int? categoryId = null, int? courseId = null, int? instructorId = null, bool? isPublic = null, int pageNumber = 1, int pageSize = 10);
        Task<List<ExamCategory>> GetExamCategoriesAsync();
        Task<List<QuestionType>> GetQuestionTypesAsync();

        // Question Management
        Task<ExamOperationResult> AddQuestionAsync(CreateQuestionModel model);
        Task<List<Question>> GetExamQuestionsAsync(int examId, bool shuffle = false);
        Task<Question?> GetQuestionByIdAsync(int questionId);
        Task<bool> DeleteQuestionAsync(int questionId);

        // Question Options
        Task<ExamOperationResult> AddQuestionOptionAsync(int questionId, CreateQuestionOptionModel model);
        Task<List<QuestionOption>> GetQuestionOptionsAsync(int questionId, bool shuffle = false);

        // Exam Attempts
        Task<ExamOperationResult> StartExamAttemptAsync(int examId, int userId, string? ipAddress = null, string? userAgent = null);
        Task<ExamAttempt?> GetAttemptByIdAsync(int attemptId);
        Task<List<ExamAttempt>> GetUserExamAttemptsAsync(int userId, int? examId = null);

        // Answer Submission
        Task<bool> SubmitAnswerAsync(SubmitAnswerModel model);
        Task<ExamOperationResult> SubmitExamAsync(int attemptId);

        // Results
        Task<ExamResultsModel?> GetExamResultsAsync(int attemptId);

        // Access Control
        Task<ExamAccessResult> CheckExamAccessAsync(int examId, int userId, string? password = null);

        Task<QuestionRevealDto?> RevealAnswerAsync(int attemptId, int questionId); // جديد


        Task<bool> HasUserPaidExamAsync(int examId, int userId);


        Task<PagedResult<ExamListItem>> GetExamsAsync(
          string? search, int? categoryId, bool? isActive, bool? isPublic,
          int page, int pageSize);

        Task<ExamListItem?> GetByIdAsync(int examId);


    }

    public class ExamRepository : IExamRepository
    {
        private readonly string _connectionString;

        public ExamRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BedayaDB")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        private IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // ==================== Exam Management ====================

        // ===========================
        // Create (no transaction)
        // ===========================
        public async Task<ExamOperationResult> CreateExamAsync(CreateExamModel model, int createdBy, int instructorId)
        {
            if (model.Price == 0)
            {
                model.IsPublic = true;
                model.RequirePassword = false;
                model.ExamPassword = null;
            }

            const string sql = @"
INSERT INTO dbo.Exams
(
    ExamTitleAr, ExamTitleEn, DescriptionAr, DescriptionEn,
    CategoryId, CourseId, InstructorId,
    DurationMinutes, TotalMarks, PassingMarks, MaxAttempts,
    StartDate, EndDate, IsPublic, RequirePassword, ExamPassword,
    ShowResultsImmediately, ShowCorrectAnswers, ShuffleQuestions, ShuffleOptions,
    IsActive, IsDeleted, CreatedAt, UpdatedAt, CreatedBy,
    ShowAnswerDuringExam, ShowExplanationDuringExam, Price
)
VALUES
(
    @ExamTitleAr, @ExamTitleEn, @DescriptionAr, @DescriptionEn,
    @CategoryId, @CourseId, @InstructorId,
    @DurationMinutes, @TotalMarks, @PassingMarks, @MaxAttempts,
    @StartDate, @EndDate, @IsPublic, @RequirePassword, @ExamPassword,
    @ShowResultsImmediately, @ShowCorrectAnswers, @ShuffleQuestions, @ShuffleOptions,
    1, 0, SYSUTCDATETIME(), SYSUTCDATETIME(), @CreatedBy,
    @ShowAnswerDuringExam, @ShowExplanationDuringExam, @Price
);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using var conn = GetConnection();
                // فتح الاتصال اختياري لأن Dapper بيفتح لوحده عند الحاجة
                // await conn.OpenAsync();

                var newId = await conn.ExecuteScalarAsync<int>(
                    sql,
                    new
                    {
                        model.ExamTitleAr,
                        model.ExamTitleEn,
                        model.DescriptionAr,
                        model.DescriptionEn,
                        model.CategoryId,
                        model.CourseId,
                        InstructorId = instructorId,
                        model.DurationMinutes,
                        model.TotalMarks,
                        model.PassingMarks,
                        model.MaxAttempts,
                        model.StartDate,
                        model.EndDate,
                        model.IsPublic,
                        model.RequirePassword,
                        model.ExamPassword,
                        model.ShowResultsImmediately,
                        model.ShowCorrectAnswers,
                        model.ShuffleQuestions,
                        model.ShuffleOptions,
                        CreatedBy = createdBy,
                        model.ShowAnswerDuringExam,
                        model.ShowExplanationDuringExam,
                        model.Price
                    }
                );

                return new ExamOperationResult
                {
                    Success = true,
                    Message = "Exam created successfully.",
                    ExamId = newId
                };
            }
            catch (SqlException ex)
            {
                var (msg, handled) = MapSqlConstraintError(ex);
                return new ExamOperationResult
                {
                    Success = false,
                    Message = handled ? msg : $"Failed to create exam: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = $"Failed to create exam: {ex.Message}"
                };
            }
        }
        // ===========================
        // Update (no transaction)
        // ===========================
        public async Task<ExamOperationResult> UpdateExamAsync(UpdateExamModel model, int updatedBy)
        {
            if (model.Price == 0)
            {
                model.IsPublic = true;
                model.RequirePassword = false;
                model.ExamPassword = null;
            }

            const string sql = @"
UPDATE dbo.Exams
SET
    ExamTitleAr = @ExamTitleAr,
    ExamTitleEn = @ExamTitleEn,
    DescriptionAr = @DescriptionAr,
    DescriptionEn = @DescriptionEn,
    CategoryId = @CategoryId,
    CourseId = @CourseId,
    DurationMinutes = @DurationMinutes,
    TotalMarks = @TotalMarks,
    PassingMarks = @PassingMarks,
    MaxAttempts = @MaxAttempts,
    StartDate = @StartDate,
    EndDate = @EndDate,
    IsPublic = @IsPublic,
    RequirePassword = @RequirePassword,
    ExamPassword = @ExamPassword,
    ShowResultsImmediately = @ShowResultsImmediately,
    ShowCorrectAnswers = @ShowCorrectAnswers,
    ShuffleQuestions = @ShuffleQuestions,
    ShuffleOptions = @ShuffleOptions,
    ShowAnswerDuringExam = @ShowAnswerDuringExam,
    ShowExplanationDuringExam = @ShowExplanationDuringExam,
    Price = @Price,
    UpdatedAt = SYSUTCDATETIME()
WHERE ExamId = @ExamId AND IsDeleted = 0; 
-- ولو عايز تربط الصلاحية بالمستخدم علّق السطر التالي بدل السابق:
-- AND (CreatedBy = @updatedBy OR InstructorId = @updatedBy);";

            try
            {
                using var conn = GetConnection();

                var rows = await conn.ExecuteAsync(
                    sql,
                    new
                    {
                        model.ExamTitleAr,
                        model.ExamTitleEn,
                        model.DescriptionAr,
                        model.DescriptionEn,
                        model.CategoryId,
                        model.CourseId,
                        model.DurationMinutes,
                        model.TotalMarks,
                        model.PassingMarks,
                        model.MaxAttempts,
                        model.StartDate,
                        model.EndDate,
                        model.IsPublic,
                        model.RequirePassword,
                        model.ExamPassword,
                        model.ShowResultsImmediately,
                        model.ShowCorrectAnswers,
                        model.ShuffleQuestions,
                        model.ShuffleOptions,
                        model.ShowAnswerDuringExam,
                        model.ShowExplanationDuringExam,
                        model.Price,
                        model.ExamId,
                        updatedBy
                    }
                );

                if (rows == 0)
                {
                    return new ExamOperationResult
                    {
                        Success = false,
                        Message = "Exam not found or already deleted."
                    };
                }

                return new ExamOperationResult
                {
                    Success = true,
                    Message = "Exam updated successfully.",
                    ExamId = model.ExamId
                };
            }
            catch (SqlException ex)
            {
                var (msg, handled) = MapSqlConstraintError(ex);
                return new ExamOperationResult
                {
                    Success = false,
                    Message = handled ? msg : $"Failed to update exam: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = $"Failed to update exam: {ex.Message}"
                };
            }
        }

        // ===========================
        // Delete (Soft Delete) (no transaction)
        // ===========================
        public async Task<bool> DeleteExamAsync(int examId, int updatedBy)
        {
            const string sql = @"
UPDATE dbo.Exams
SET IsDeleted = 1,
    IsActive = 0,
    UpdatedAt = SYSUTCDATETIME()
WHERE ExamId = @ExamId AND IsDeleted = 0;
-- ولو عايز تقيد بالصلاحية:
-- AND (CreatedBy = @updatedBy OR InstructorId = @updatedBy);";

            try
            {
                using var conn = GetConnection();
                var rows = await conn.ExecuteAsync(sql, new { ExamId = examId, updatedBy });
                return rows > 0;
            }
            catch
            {
                return false;
            }
        }

        // ===========================
        // Helpers
        // ===========================
        private static (string Message, bool Handled) MapSqlConstraintError(SqlException ex)
        {
            // رقم 547 = FK/CK violation غالباً
            if (ex.Number == 547)
            {
                var msg = ex.Message;

                if (msg.Contains("CK_Exams_Price_NonNegative", StringComparison.OrdinalIgnoreCase))
                    return ("السعر لا يمكن أن يكون سالباً.", true);

                if (msg.Contains("CK_Exams_Public_When_Free", StringComparison.OrdinalIgnoreCase))
                    return ("الاختبار المجاني يجب أن يكون عاماً.", true);

                if (msg.Contains("CK_Exams_NoPassword_When_Free", StringComparison.OrdinalIgnoreCase))
                    return ("الاختبار المجاني لا يجب أن يطلب كلمة مرور.", true);

                if (msg.Contains("FK_Exams_Categories", StringComparison.OrdinalIgnoreCase))
                    return ("الفئة المحددة غير موجودة.", true);

                if (msg.Contains("FK_Exams_Instructors", StringComparison.OrdinalIgnoreCase))
                    return ("المُدرّس المحدد غير موجود.", true);

                if (msg.Contains("FK_Exams_CreatedBy", StringComparison.OrdinalIgnoreCase))
                    return ("المستخدم (CreatedBy) غير موجود.", true);

                return ("قيود قاعدة البيانات تمنع العملية (تحقق من القيم المرتبطة).", true);
            }

            // أرقام شائعة أخرى: 2627/2601 = unique index
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                return ("قيمة مكررة تُخالف قيد فريد.", true);
            }

            return (string.Empty, false);
        }

        public async Task<Exam?> GetExamByIdAsync(int examId, int? userId = null)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var exam = await connection.QueryFirstOrDefaultAsync<Exam>(
                        "sp_GetExamById",
                        new { ExamId = examId, UserId = userId },
                        commandType: CommandType.StoredProcedure
                    );

                    return exam;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Exam>> GetAllExamsAsync(int? categoryId = null, int? courseId = null, int? instructorId = null, bool? isPublic = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var exams = await connection.QueryAsync<Exam>(
                        "sp_GetAllExams",
                        new
                        {
                            CategoryId = categoryId,
                            CourseId = courseId,
                            InstructorId = instructorId,
                            IsPublic = isPublic,
                            PageNumber = pageNumber,
                            PageSize = pageSize
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return exams.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<Exam>();
            }
        }

        public async Task<List<ExamCategory>> GetExamCategoriesAsync()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var sql = "SELECT * FROM ExamCategories WHERE IsActive = 1 ORDER BY DisplayOrder";
                    var categories = await connection.QueryAsync<ExamCategory>(sql);
                    return categories.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<ExamCategory>();
            }
        }

        public async Task<List<QuestionType>> GetQuestionTypesAsync()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var sql = "SELECT * FROM QuestionTypes WHERE IsActive = 1";
                    var types = await connection.QueryAsync<QuestionType>(sql);
                    return types.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<QuestionType>();
            }
        }

        // ==================== Question Management ====================

        public async Task<ExamOperationResult> AddQuestionAsync(CreateQuestionModel model)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ExamId", model.ExamId);
                    parameters.Add("@QuestionTypeId", model.QuestionTypeId);
                    parameters.Add("@QuestionTextAr", model.QuestionTextAr);
                    parameters.Add("@QuestionTextEn", model.QuestionTextEn);
                    parameters.Add("@QuestionImageUrl", model.QuestionImageUrl);
                    parameters.Add("@Marks", model.Marks);
                    parameters.Add("@NegativeMarks", model.NegativeMarks);
                    parameters.Add("@CorrectAnswer", model.CorrectAnswer);
                    parameters.Add("@DisplayOrder", model.DisplayOrder);
                    parameters.Add("@ExplanationAr", model.ExplanationAr);
                    parameters.Add("@ExplanationEn", model.ExplanationEn);
                    parameters.Add("@QuestionId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("sp_AddQuestion", parameters, commandType: CommandType.StoredProcedure);

                    int questionId = parameters.Get<int>("@QuestionId");

                    // Add options if provided
                    foreach (var option in model.Options)
                    {
                        await AddQuestionOptionAsync(questionId, option);
                    }

                    return new ExamOperationResult
                    {
                        Success = true,
                        QuestionId = questionId,
                        Message = "تم إضافة السؤال بنجاح"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "حدث خطأ أثناء إضافة السؤال"
                };
            }
        }

        public async Task<List<Question>> GetExamQuestionsAsync(int examId, bool shuffle = false)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var questions = await connection.QueryAsync<Question>(
                        "sp_GetExamQuestions",
                        new { ExamId = examId, ShuffleQuestions = shuffle },
                        commandType: CommandType.StoredProcedure
                    );

                    var questionList = questions.ToList();

                    // Load options for each question
                    foreach (var question in questionList)
                    {
                        question.Options = await GetQuestionOptionsAsync(question.QuestionId, shuffle);
                    }

                    return questionList;
                }
            }
            catch (Exception ex)
            {
                return new List<Question>();
            }
        }

        public async Task<Question?> GetQuestionByIdAsync(int questionId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var sql = @"
                        SELECT q.*, qt.TypeNameAr, qt.TypeNameEn, qt.TypeCode, 
                               qt.AllowMultipleAnswers, qt.RequiresOptions
                        FROM Questions q
                        INNER JOIN QuestionTypes qt ON q.QuestionTypeId = qt.QuestionTypeId
                        WHERE q.QuestionId = @QuestionId AND q.IsDeleted = 0";

                    var question = await connection.QueryFirstOrDefaultAsync<Question>(sql, new { QuestionId = questionId });

                    if (question != null)
                    {
                        question.Options = await GetQuestionOptionsAsync(questionId);
                    }

                    return question;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var sql = "UPDATE Questions SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE QuestionId = @QuestionId";
                    await connection.ExecuteAsync(sql, new { QuestionId = questionId });
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // ==================== Question Options ====================

        public async Task<ExamOperationResult> AddQuestionOptionAsync(int questionId, CreateQuestionOptionModel model)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@QuestionId", questionId);
                    parameters.Add("@OptionTextAr", model.OptionTextAr);
                    parameters.Add("@OptionTextEn", model.OptionTextEn);
                    parameters.Add("@OptionImageUrl", model.OptionImageUrl);
                    parameters.Add("@IsCorrect", model.IsCorrect);
                    parameters.Add("@DisplayOrder", model.DisplayOrder);
                    parameters.Add("@OptionId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("sp_AddQuestionOption", parameters, commandType: CommandType.StoredProcedure);

                    int optionId = parameters.Get<int>("@OptionId");

                    return new ExamOperationResult
                    {
                        Success = true,
                        OptionId = optionId,
                        Message = "تم إضافة الخيار بنجاح"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "حدث خطأ أثناء إضافة الخيار"
                };
            }
        }

        public async Task<List<QuestionOption>> GetQuestionOptionsAsync(int questionId, bool shuffle = false)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var options = await connection.QueryAsync<QuestionOption>(
                        "sp_GetQuestionOptions",
                        new { QuestionId = questionId, ShuffleOptions = shuffle },
                        commandType: CommandType.StoredProcedure
                    );

                    return options.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<QuestionOption>();
            }
        }

        // ==================== Exam Attempts ====================

        public async Task<ExamOperationResult> StartExamAttemptAsync(int examId, int userId, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ExamId", examId);
                    parameters.Add("@UserId", userId);
                    parameters.Add("@IpAddress", ipAddress);
                    parameters.Add("@UserAgent", userAgent);
                    parameters.Add("@AttemptId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("sp_StartExamAttempt", parameters, commandType: CommandType.StoredProcedure);

                    int attemptId = parameters.Get<int>("@AttemptId");

                    return new ExamOperationResult
                    {
                        Success = true,
                        AttemptId = attemptId,
                        Message = "تم بدء الاختبار بنجاح"
                    };
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Maximum attempts reached"))
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "لقد وصلت للحد الأقصى من المحاولات"
                };
            }
            catch (Exception ex)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "حدث خطأ أثناء بدء الاختبار"
                };
            }
        }

        public async Task<ExamAttempt?> GetAttemptByIdAsync(int attemptId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var sql = "SELECT * FROM ExamAttempts WHERE AttemptId = @AttemptId";
                    var attempt = await connection.QueryFirstOrDefaultAsync<ExamAttempt>(sql, new { AttemptId = attemptId });
                    return attempt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ExamAttempt>> GetUserExamAttemptsAsync(int userId, int? examId = null)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var attempts = await connection.QueryAsync<ExamAttempt>(
                        "sp_GetUserExamAttempts",
                        new { UserId = userId, ExamId = examId },
                        commandType: CommandType.StoredProcedure
                    );

                    return attempts.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<ExamAttempt>();
            }
        }

        // ==================== Answer Submission ====================

        public async Task<bool> SubmitAnswerAsync(SubmitAnswerModel model)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.ExecuteAsync(
                        "sp_SubmitAnswer",
                        new
                        {
                            AttemptId = model.AttemptId,
                            QuestionId = model.QuestionId,
                            SelectedOptionId = model.SelectedOptionId,
                            AnswerText = model.AnswerText,
                            TimeSpentSeconds = model.TimeSpentSeconds
                        },
                        commandType: CommandType.StoredProcedure
                    );

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ExamOperationResult> SubmitExamAsync(int attemptId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.ExecuteAsync(
                        "sp_SubmitExam",
                        new { AttemptId = attemptId },
                        commandType: CommandType.StoredProcedure
                    );

                    return new ExamOperationResult
                    {
                        Success = true,
                        AttemptId = attemptId,
                        Message = "تم تسليم الاختبار بنجاح"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ExamOperationResult
                {
                    Success = false,
                    Message = "حدث خطأ أثناء تسليم الاختبار"
                };
            }
        }

        // ==================== Results ====================

        public async Task<ExamResultsModel?> GetExamResultsAsync22(int attemptId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(
                        "sp_GetExamResults",
                        new { AttemptId = attemptId },
                        commandType: CommandType.StoredProcedure))
                    {
                        var attempt = await multi.ReadFirstOrDefaultAsync<ExamAttempt>();
                        var answers = (await multi.ReadAsync<ExamAnswer>()).ToList();

                        if (attempt == null)
                            return null;

                        // Get questions with options
                        var questions = await GetExamQuestionsAsync(attempt.ExamId);

                        var results = new ExamResultsModel
                        {
                            Attempt = attempt,
                            Questions = questions.Select(q => new QuestionResultModel
                            {
                                Question = q,
                                UserAnswer = answers.FirstOrDefault(a => a.QuestionId == q.QuestionId),
                                Options = q.Options
                            }).ToList()
                        };

                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ExamResultsModel?> GetExamResultsAsync(int attemptId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(
                        "sp_GetExamResults",
                        new { AttemptId = attemptId },
                        commandType: CommandType.StoredProcedure))
                    {
                        var attempt = await multi.ReadFirstOrDefaultAsync<ExamAttempt>();
                        var answers = (await multi.ReadAsync<ExamAnswer>()).ToList();

                        if (attempt == null)
                            return null;

                        // جلب الأسئلة مع الخيارات
                        var questions = await GetExamQuestionsAsync(attempt.ExamId);

                        var results = new ExamResultsModel
                        {
                            Attempt = attempt,
                            Questions = questions.Select(q => new QuestionResultModel
                            {
                                Question = q,
                                UserAnswer = answers.FirstOrDefault(a => a.QuestionId == q.QuestionId),
                                Options = q.Options
                            }).ToList()
                        };

                        // ======= حساب النتيجة المبدئية باستبعاد SHORT/ESSAY =======
                        // نعتبر القابلة للتصحيح الآلي: كل الأنواع عدا SHORT, ESSAY
                        var autoQuestions = questions.Where(q =>
                            !string.Equals(q.TypeCode, "SHORT", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(q.TypeCode, "ESSAY", StringComparison.OrdinalIgnoreCase)
                        ).ToList();

                        results.AutoMaxMarks = autoQuestions.Sum(q => q.Marks);

                        decimal autoScore = 0m;

                        foreach (var q in autoQuestions)
                        {
                            var ans = answers.FirstOrDefault(a => a.QuestionId == q.QuestionId);

                            // مفيش إجابة
                            if (ans == null)
                            {
                                // خصم درجات سالبة إن وجدت (اختياري)
                                if (q.NegativeMarks > 0)
                                    autoScore -= q.NegativeMarks;
                                continue;
                            }

                            bool isCorrect = false;

                            // MCQ/TF: اختيار واحد
                            if (string.Equals(q.TypeCode, "MCQ", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(q.TypeCode, "TF", StringComparison.OrdinalIgnoreCase))
                            {
                                if (ans.SelectedOptionId.HasValue)
                                {
                                    isCorrect = q.Options.Any(o => o.OptionId == ans.SelectedOptionId.Value && o.IsCorrect);
                                }
                            }
                            // FILLBLANK: مقارنة نصية بسيطة (Trim + case-insensitive)
                            else if (string.Equals(q.TypeCode, "FILLBLANK", StringComparison.OrdinalIgnoreCase))
                            {
                                var userText = (ans.AnswerText ?? string.Empty).Trim();
                                var correct = (q.CorrectAnswer ?? string.Empty).Trim();
                                if (!string.IsNullOrEmpty(correct))
                                {
                                    isCorrect = string.Equals(userText, correct, StringComparison.OrdinalIgnoreCase);
                                }
                            }
                            // MSQ أو أنواع أخرى (لو موجودة): ممكن تتجاهل أو تعالج حسب تصميمك الحالي
                            // هنا هنسيبها غير محسوبة لو مش عندك تخزين متعدد للاختيارات
                            // else if (q.TypeCode == "MSQ") { ... }

                            if (isCorrect)
                                autoScore += q.Marks;
                            else if (q.NegativeMarks > 0)
                                autoScore -= q.NegativeMarks;
                        }

                        results.AutoObtainedMarks = autoScore;
                        results.AutoProvisionalPercentage = (results.AutoMaxMarks > 0)
                            ? Math.Round((results.AutoObtainedMarks / results.AutoMaxMarks) * 100m, 2)
                            : 0m;

                        // الأسئلة المقالية المعلّقة
                        var manualQuestions = questions.Where(q =>
                            string.Equals(q.TypeCode, "SHORT", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(q.TypeCode, "ESSAY", StringComparison.OrdinalIgnoreCase)
                        );

                        results.PendingManualCount = manualQuestions.Count();
                        results.PendingManualMarks = manualQuestions.Sum(q => q.Marks);

                        return results;
                    }
                }
            }
            catch
            {
                return null;
            }
        }


        // ==================== Access Control ====================

        public async Task<ExamAccessResult> CheckExamAccessAsync(int examId, int userId, string? password = null)
        {
            try
            {
                var exam = await GetExamByIdAsync(examId, userId);

                if (exam == null)
                {
                    return new ExamAccessResult
                    {
                        HasAccess = false,
                        Message = "الاختبار غير موجود"
                    };
                }

                // Check if scheduled
                var now = DateTime.Now;
                var isScheduled = exam.StartDate.HasValue || exam.EndDate.HasValue;
                var hasStarted = !exam.StartDate.HasValue || exam.StartDate <= now;
                var hasEnded = exam.EndDate.HasValue && exam.EndDate < now;

                if (isScheduled && !hasStarted)
                {
                    return new ExamAccessResult
                    {
                        HasAccess = false,
                        IsScheduled = true,
                        HasStarted = false,
                        Message = $"الاختبار سيبدأ في {exam.StartDate:yyyy-MM-dd HH:mm}"
                    };
                }

                if (hasEnded)
                {
                    return new ExamAccessResult
                    {
                        HasAccess = false,
                        IsScheduled = true,
                        HasEnded = true,
                        Message = "انتهى وقت الاختبار"
                    };
                }

                // Check attempts
                if (exam.MaxAttempts > 0 && exam.UserAttempts >= exam.MaxAttempts)
                {
                    return new ExamAccessResult
                    {
                        HasAccess = false,
                        MaxAttemptsReached = true,
                        Message = "لقد وصلت للحد الأقصى من المحاولات"
                    };
                }

                // Check password
                if (exam.RequirePassword)
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        return new ExamAccessResult
                        {
                            HasAccess = false,
                            RequiresPassword = true,
                            Message = "الاختبار يتطلب كلمة مرور"
                        };
                    }

                    if (password != exam.ExamPassword)
                    {
                        return new ExamAccessResult
                        {
                            HasAccess = false,
                            RequiresPassword = true,
                            Message = "كلمة المرور غير صحيحة"
                        };
                    }
                }

                return new ExamAccessResult
                {
                    HasAccess = true,
                    Message = "يمكنك بدء الاختبار"
                };
            }
            catch (Exception ex)
            {
                return new ExamAccessResult
                {
                    HasAccess = false,
                    Message = "حدث خطأ أثناء التحقق من الصلاحيات"
                };
            }
        }

        public async Task<QuestionRevealDto?> RevealAnswerAsync(int attemptId, int questionId)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "sp_GetQuestionReveal",
                        new { AttemptId = attemptId, QuestionId = questionId },
                        commandType: CommandType.StoredProcedure);

                    if (row == null) return null;

                    var dto = new QuestionRevealDto
                    {
                        QuestionId = row.QuestionId,
                        ExplanationAr = row.ExplanationAr,
                        ExplanationEn = row.ExplanationEn,
                        ShowAnswer = (bool)row.ShowAnswer,
                        CorrectOptionIds = string.IsNullOrWhiteSpace((string?)row.CorrectOptionIds)
                            ? new List<int>()
                            : ((string)row.CorrectOptionIds).Split(',').Select(int.Parse).ToList()
                    };
                    return dto;
                }
            }
            catch
            {
                return null;
            }
        }


        public async Task<bool> HasUserPaidExamAsync(int examId, int userId)
        {
            const string sql = @"
SELECT CASE WHEN EXISTS(
   SELECT 1 FROM dbo.ExamOrders
   WHERE ExamId = @ExamId AND UserId = @UserId
     AND Status = N'PAID' AND IsDeleted = 0
) THEN 1 ELSE 0 END;";
            using var conn = GetConnection();
            var v = await conn.ExecuteScalarAsync<int>(sql, new { ExamId = examId, UserId = userId });
            return v == 1;
        }

        public async Task<PagedResult<ExamListItem>> GetExamsAsync(
           string? search, int? categoryId, bool? isActive, bool? isPublic,
           int page, int pageSize)
        {
            var p = new
            {
                search = string.IsNullOrWhiteSpace(search) ? null : search!.Trim(),
                categoryId,
                isActive,
                isPublic,
                offset = (page - 1) * pageSize,
                pageSize
            };

            const string sql = @"
-- صفحة البيانات
WITH base AS (
    SELECT e.*,
           c.CategoryNameAr, c.CategoryNameEn
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamCategories c ON c.CategoryId = e.CategoryId
    WHERE ISNULL(e.IsDeleted,0) = 0
      AND (@categoryId IS NULL OR e.CategoryId = @categoryId)
      AND (@isActive  IS NULL OR e.IsActive  = @isActive)
      AND (@isPublic  IS NULL OR e.IsPublic  = @isPublic)
      AND (
            @search IS NULL OR
            e.ExamTitleAr   LIKE '%' + @search + '%' OR
            e.ExamTitleEn   LIKE '%' + @search + '%' OR
            e.DescriptionAr LIKE '%' + @search + '%' OR
            e.DescriptionEn LIKE '%' + @search + '%' OR
            c.CategoryNameAr LIKE '%' + @search + '%' OR
            c.CategoryNameEn LIKE '%' + @search + '%'
          )
),
agg AS (
    SELECT b.ExamId,
           COUNT(DISTINCT a.AttemptId)  AS AttemptsCount,
           COUNT(DISTINCT a.UserId)     AS UniqueStudents
    FROM base b
    LEFT JOIN dbo.ExamAttempts a ON a.ExamId = b.ExamId
    GROUP BY b.ExamId
)
SELECT b.ExamId, b.ExamTitleAr, b.ExamTitleEn, b.DescriptionAr, b.DescriptionEn,
       b.CategoryId, b.CategoryNameAr, b.CategoryNameEn,
       b.DurationMinutes, b.TotalMarks, b.TotalQuestions, b.MaxAttempts,
       b.ShowResultsImmediately, b.ShowCorrectAnswers, b.ShuffleQuestions, b.ShuffleOptions,
       b.StartDate, b.EndDate, b.IsPublic, b.RequirePassword, b.ExamPassword,
       b.IsActive, b.IsDeleted, b.CreatedAt, b.UpdatedAt, b.Price, b.IsFree,
       ISNULL(g.AttemptsCount,0)   AS AttemptsCount,
       ISNULL(g.UniqueStudents,0)  AS UniqueStudents
FROM base b
LEFT JOIN agg g ON g.ExamId = b.ExamId
ORDER BY ISNULL(b.UpdatedAt, b.CreatedAt) DESC, b.ExamId DESC
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

-- الإجمالي
WITH base AS (
    SELECT e.ExamId
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamCategories c ON c.CategoryId = e.CategoryId
    WHERE ISNULL(e.IsDeleted,0) = 0
      AND (@categoryId IS NULL OR e.CategoryId = @categoryId)
      AND (@isActive  IS NULL OR e.IsActive  = @isActive)
      AND (@isPublic  IS NULL OR e.IsPublic  = @isPublic)
      AND (
            @search IS NULL OR
            e.ExamTitleAr   LIKE '%' + @search + '%' OR
            e.ExamTitleEn   LIKE '%' + @search + '%' OR
            e.DescriptionAr LIKE '%' + @search + '%' OR
            e.DescriptionEn LIKE '%' + @search + '%' OR
            c.CategoryNameAr LIKE '%' + @search + '%' OR
            c.CategoryNameEn LIKE '%' + @search + '%'
          )
)
SELECT COUNT(*) FROM base;";

            using var c = GetConnection();
            using var multi = await c.QueryMultipleAsync(sql, p);
            var items = (await multi.ReadAsync<ExamListItem>()).ToList();
            var total = await multi.ReadSingleAsync<int>();

            return new PagedResult<ExamListItem>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ExamListItem?> GetByIdAsync(int examId)
        {
            const string sql = @"
SELECT TOP 1 e.*, c.CategoryNameAr, c.CategoryNameEn
FROM dbo.Exams e
LEFT JOIN dbo.ExamCategories c ON c.CategoryId = e.CategoryId
WHERE e.ExamId = @examId AND ISNULL(e.IsDeleted,0) = 0;

-- stats
SELECT
    (SELECT COUNT(DISTINCT AttemptId) FROM dbo.ExamAttempts WHERE ExamId = @examId)  AS AttemptsCount,
    (SELECT COUNT(DISTINCT UserId)    FROM dbo.ExamAttempts WHERE ExamId = @examId)  AS UniqueStudents;
";
            using var c = GetConnection();
            using var multi = await c.QueryMultipleAsync(sql, new { examId });
            var exam = await multi.ReadFirstOrDefaultAsync<ExamListItem>();
            if (exam == null) return null;
            var stats = await multi.ReadFirstAsync<(int AttemptsCount, int UniqueStudents)>();
            exam.AttemptsCount = stats.AttemptsCount;
            exam.UniqueStudents = stats.UniqueStudents;
            return exam;
        }
    }



    public class ExamListItem
    {
        public int ExamId { get; set; }
        public string? ExamTitleAr { get; set; }
        public string? ExamTitleEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryNameAr { get; set; }
        public string? CategoryNameEn { get; set; }
        public int? DurationMinutes { get; set; }
        public int? TotalMarks { get; set; }
        public int? TotalQuestions { get; set; }
        public int? MaxAttempts { get; set; }
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
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal? Price { get; set; }
        public bool IsFree { get; set; }

        // إحصائيات
        public int AttemptsCount { get; set; }
        public int UniqueStudents { get; set; }

        // للعرض السريع
        public string Title => string.IsNullOrWhiteSpace(ExamTitleAr) ? (ExamTitleEn ?? "") : ExamTitleAr!;
    }
}