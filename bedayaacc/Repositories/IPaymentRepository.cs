using bedayaacc.Models;

namespace bedayaacc.Repositories
{
    public interface IPaymentRepository
    {
        Task<int> CreateOrderAsync(int userId, int examId, string currency = "EGP", decimal discountAmount = 0);

        // جديد: إنشاء طلب تحويل بنكي بشكل صريح (لو حابب تميّز)
        Task<int> CreateBankTransferOrderAsync(int userId, int examId, string currency = "EGP", decimal discountAmount = 0);

        // جديد: رفع الإيصال واعتبار الطلب مدفوع (غير مُراجع)
        Task<bool> UploadReceiptAndMarkPaidUnverifiedAsync(int orderId, decimal paidAmount, string receiptFileName, string receiptUrl, string? bankRef);

        Task<List<int>> GetPaidExamIdsAsync(int userId); // يجب أن تشمل PAID و PAID_UNVERIFIED
        Task<ExamOrder?> GetOrderAsync(int orderId, int userId);
        Task<ExamOrder?> GetLastPaidOrderAsync(int userId, int examId);

        // يجب أن تشمل PAID و PAID_UNVERIFIED
        Task<bool> HasUserPaidExamAsync(int examId, int userId);

        // مساعد: اجلب آخر أمر غير مكتمل/بانتظار إيصال لنفس الامتحان
        Task<ExamOrder?> GetPendingOrderAsync(int userId, int examId);


        Task<(List<OrderReviewItem> Items, int Total)> GetOrdersForInstructorAsync(
           int instructorId, string? status, int page, int pageSize, string? search);

        Task<bool> MarkOrderVerifiedAsync(int orderId, int verifiedBy);

        Task<bool> MarkOrderRejectedAsync(int orderId, int verifiedBy, string? reason);
    }

}
