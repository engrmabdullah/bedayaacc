using bedayaacc.Components;
using bedayaacc.Repositories;
using bedayaacc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Security.Cryptography;
using AppAuthService = bedayaacc.Repositories.IAuthenticationService;
using AppAuthServiceImpl = bedayaacc.Repositories.AuthenticationService;


var builder = WebApplication.CreateBuilder(args);

// ===== Services =====
builder.Services.AddMemoryCache(); // <— مهم
builder.Services.AddSingleton<LoginTicketStore>(); // <— جديد

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TranslationService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<AppAuthService, AppAuthServiceImpl>();
builder.Services.AddScoped<bedayaacc.Services.Repositories.ICourseRepository, bedayaacc.Services.Repositories.InMemoryCourseRepository>();
builder.Services.AddScoped<IStudentDashboardRepository, StudentDashboardRepository>();
builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IFileStorage, FileStorage>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();


builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = "bedayaacc.auth";
        o.LoginPath = "/login";
        o.LogoutPath = "/logout";
        o.AccessDeniedPath = "/denied";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromDays(14);
        o.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = 401; return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri); return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = 403; return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri); return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
    options.AddPolicy("Instructor", p => p.RequireRole("Instructor")); // جديد

});

builder.Services.AddAntiforgery(o =>
{
    o.HeaderName = "X-CSRF-TOKEN";
    o.Cookie.Name = "bedayaacc.csrf";
    o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

//
// ===== Callback Endpoint لتوليد الكوكيز على GET عادي =====
//

app.MapGet("/auth/callback", async (HttpContext ctx, string token, string? target, LoginTicketStore tickets) =>
{
    if (!tickets.TryConsume(token, out var t) || t.ExpiresUtc < DateTimeOffset.UtcNow)
    {
        ctx.Response.Redirect("/login?err=expired");
        return;
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, t.UserId.ToString()),
        new(ClaimTypes.Name, $"{t.FirstName} {t.LastName}".Trim())
    };
    if (!string.IsNullOrWhiteSpace(t.Email))
        claims.Add(new Claim(ClaimTypes.Email, t.Email!));
    foreach (var r in t.Roles)
        claims.Add(new Claim(ClaimTypes.Role, r));

    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    var props = new AuthenticationProperties
    {
        IsPersistent = t.Remember,
        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(t.Remember ? 30 : 1)
    };

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

    var safeTarget = string.IsNullOrWhiteSpace(target) ? "/" : target;
    ctx.Response.Redirect(safeTarget);
});
app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    ctx.Response.Redirect("/");
});
app.Run();

//
// ===== LoginTicketStore (تخزين مؤقت للتذكرة) =====
//
public record LoginTicket(
    int UserId,
    string FirstName,
    string LastName,
    string? Email,
    List<string> Roles,
    bool Remember,
    DateTimeOffset ExpiresUtc
);

public sealed class LoginTicketStore
{
    private readonly IMemoryCache _cache;
    public LoginTicketStore(IMemoryCache cache) => _cache = cache;

    public string Create(LoginTicket ticket)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        _cache.Set(token, ticket, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = ticket.ExpiresUtc
        });
        return token;
    }

    public bool TryConsume(string token, out LoginTicket ticket)
    {
        if (_cache.TryGetValue(token, out ticket!))
        {
            _cache.Remove(token); // one-time
            return true;
        }
        ticket = default!;
        return false;
    }
}

