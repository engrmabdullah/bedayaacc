namespace bedayaacc.Services
{
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Hosting;

    public class FileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _env;
        public FileStorage(IWebHostEnvironment env) => _env = env;

        public async Task<(string FileName, string RelativeUrl)> SaveReceiptAsync(
            IBrowserFile file, IProgress<double>? progress = null, CancellationToken ct = default)
        {
            var root = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "receipts");
            Directory.CreateDirectory(root);

            var safe = string.Concat(Path.GetFileNameWithoutExtension(file.Name).Take(60));
            var ext = Path.GetExtension(file.Name);
            var final = $"{Guid.NewGuid():N}_{safe}{ext}";
            var abs = Path.Combine(root, final);

            await using var fs = new FileStream(abs, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await using var src = file.OpenReadStream(25 * 1024 * 1024); // 25MB

            var total = file.Size;
            var buf = new byte[81920];
            long readTotal = 0;
            int read;

            while ((read = await src.ReadAsync(buf.AsMemory(0, buf.Length), ct)) > 0)
            {
                await fs.WriteAsync(buf.AsMemory(0, read), ct);
                readTotal += read;
                if (progress != null && total > 0)
                {
                    var p = Math.Round(readTotal * 100.0 / total, 0);
                    progress.Report(p);
                }
            }

            var rel = $"/uploads/receipts/{final}";
            return (final, rel);
        }
    }

}
