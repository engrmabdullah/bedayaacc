using Microsoft.AspNetCore.Components.Forms;

namespace bedayaacc.Services
{
    public interface IFileStorage
    {
        Task<(string FileName, string RelativeUrl)> SaveReceiptAsync(
      IBrowserFile file,
      IProgress<double>? progress = null,
      CancellationToken ct = default);
    }
}
