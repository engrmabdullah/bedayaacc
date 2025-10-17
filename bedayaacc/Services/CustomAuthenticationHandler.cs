using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace bedayaacc.Services
{
    public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthOptions>
    {
        public CustomAuthenticationHandler(
            IOptionsMonitor<CustomAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Always return success - actual authentication is handled by CustomAuthenticationStateProvider
            var identity = new ClaimsIdentity("CustomAuthenticationScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "CustomAuthenticationScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
    }
}
