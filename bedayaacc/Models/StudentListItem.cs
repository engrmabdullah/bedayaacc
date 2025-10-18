namespace bedayaacc.Models
{


    public class StudentListItem
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int ExamsCount { get; set; }
        public int AttemptsCount { get; set; }
        public DateTime? LastActivity { get; set; }
        public string FullName => string.Join(' ', new[] { FirstName, LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    public class StudentDetailsDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public List<StudentOrderDto> Orders { get; set; } = new();
        public List<StudentAttemptDto> Attempts { get; set; } = new();

        public int TotalExams => Orders.Select(o => o.ExamId).Distinct().Count();
        public int TotalAttempts => Attempts.Count;
        public DateTime? LastActivity =>
            (new DateTime?[] { Attempts.MaxOrDefault(a => a.SubmittedAt ?? a.StartedAt), Orders.MaxOrDefault(o => o.PaidAt ?? o.CreatedAt) })
            .Where(d => d.HasValue).DefaultIfEmpty(null).Max();
    }

    public class StudentOrderDto
    {
        public int OrderId { get; set; }
        public int ExamId { get; set; }
        public string? ExamTitle { get; set; }
        public string? Currency { get; set; }
        public decimal PaidAmount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class StudentAttemptDto
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public string? ExamTitle { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal? Percentage { get; set; }
        public bool IsPassed { get; set; }
        public string? Status { get; set; }
    }

    internal static class LinqDateExt
    {
        public static DateTime? MaxOrDefault<T>(this IEnumerable<T> src, Func<T, DateTime?> selector)
        {
            var vals = src.Select(selector).Where(x => x.HasValue).Select(x => x!.Value);
            return vals.Any() ? vals.Max() : (DateTime?)null;
        }
    }
}
