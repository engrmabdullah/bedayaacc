using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using bedayaacc.Models;
using bedayaacc.Repositories;

namespace bedayaacc.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly IUserRepository _userRepository;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private bool _isInitialized = false;

        public CustomAuthenticationStateProvider(
            ProtectedSessionStorage sessionStorage,
            IUserRepository userRepository)
        {
            _sessionStorage = sessionStorage;
            _userRepository = userRepository;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // During prerendering, return anonymous user
            if (!_isInitialized)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }

            try
            {
                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

                if (userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                }

                var roles = await _userRepository.GetUserRolesAsync(userSession.UserId);

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.UserId.ToString()),
                    new Claim(ClaimTypes.Name, userSession.FullName),
                    new Claim(ClaimTypes.Email, userSession.Email)
                }.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role))), "CustomAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAuthenticationStateAsync Error: {ex.Message}");
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task<string> UpdateAuthenticationState(UserSession? userSession)
        {
            // Mark as initialized on first use
            _isInitialized = true;

            Console.WriteLine($"UpdateAuthenticationState called with user: {userSession?.Email ?? "null"}");

            ClaimsPrincipal claimsPrincipal;
            string redirectUrl = "/";

            if (userSession != null)
            {
                try
                {
                    await _sessionStorage.SetAsync("UserSession", userSession);
                    Console.WriteLine("Session storage set successfully");

                    var roles = await _userRepository.GetUserRolesAsync(userSession.UserId);
                    Console.WriteLine($"User roles: {string.Join(", ", roles)}");

                    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userSession.UserId.ToString()),
                        new Claim(ClaimTypes.Name, userSession.FullName),
                        new Claim(ClaimTypes.Email, userSession.Email)
                    }.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role))), "CustomAuth"));

                    redirectUrl = GetRedirectUrlByRole(roles);
                    Console.WriteLine($"Redirect URL: {redirectUrl}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in UpdateAuthenticationState: {ex.Message}");
                    claimsPrincipal = _anonymous;
                }
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
                Console.WriteLine("User logged out");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            return redirectUrl;
        }

        private string GetRedirectUrlByRole(List<string> roles)
        {
            if (roles.Contains("Admin"))
                return "/admin/dashboard";
            else if (roles.Contains("Instructor"))
                return "/instructor/dashboard";
            else if (roles.Contains("Support"))
                return "/support/dashboard";
            else if (roles.Contains("Student"))
                return "/student/dashboard";
            else
                return "/";
        }
    }
    public class UserSession
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // REQUIRED: Empty constructor for serialization
        public UserSession() { }

        public UserSession(int userId, string fullName, string email)
        {
            UserId = userId;
            FullName = fullName;
            Email = email;
        }
    }
}
