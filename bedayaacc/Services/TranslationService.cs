using Microsoft.Extensions.FileProviders;
using Microsoft.JSInterop;
using System.Text.Json;

namespace bedayaacc.Services
{
    public class TranslationService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IJSRuntime _js;
        private Dictionary<string, string> _translations = new();
        private string _currentLanguage = "ar";
        private bool _initialized = false;

        public event Action? OnLanguageChanged;
        public bool IsLoaded { get; private set; } = false;

        public TranslationService(IWebHostEnvironment env, IJSRuntime js)
        {
            _env = env;
            _js = js;

            // تحميل عربي بشكل متزامن لتفادي فلاش المحتوى
            TryLoadSync(_currentLanguage);
            IsLoaded = _translations.Count > 0;
        }

        private void TryLoadSync(string language)
        {
            try
            {
                var path = Path.Combine(_env.ContentRootPath, "Localization", $"{language}.json");
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                                   ?? new Dictionary<string, string>();
                }
            }
            catch { /* تجاهل أخطاء البداية */ }
        }

        private async Task LoadAsync(string language)
        {
            var path = Path.Combine(_env.ContentRootPath, "Localization", $"{language}.json");
            if (File.Exists(path))
            {
                var json = await File.ReadAllTextAsync(path);
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                               ?? new Dictionary<string, string>();
            }
            else
            {
                _translations = new Dictionary<string, string>();
            }
        }

        public async Task SetLanguageAsync(string language)
        {
            _currentLanguage = language;
            await LoadAsync(language);

            IsLoaded = true;
            await _js.InvokeVoidAsync("localStorage.setItem", "preferredLang", language);
            await _js.InvokeVoidAsync("setHtmlLanguageAndDir", language);

            OnLanguageChanged?.Invoke();
        }

        public async Task InitAsync()
        {
            var savedLang = await _js.InvokeAsync<string>("localStorage.getItem", "preferredLang");
            if (!string.IsNullOrEmpty(savedLang))
                await SetLanguageAsync(savedLang);
            else
                await SetLanguageAsync(_currentLanguage);

            IsLoaded = true;
        }

        public async Task EnsureInitAfterRenderAsync()
        {
            if (_initialized) return;
            await InitAsync();
            _initialized = true;
        }

        public string Translate(string key)
            => _translations.TryGetValue(key, out var value) ? value : key;

        public string CurrentLanguage => _currentLanguage;
    }
}
