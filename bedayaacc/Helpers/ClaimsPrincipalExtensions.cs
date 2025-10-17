using System.Security.Claims;

namespace bedayaacc.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? TryGetUserId(this ClaimsPrincipal? user)
        {
            var id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(id, out var uid) ? uid : null;
        }
    }
}
