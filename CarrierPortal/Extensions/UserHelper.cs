using System.Security.Claims;

namespace CarrierPortal.Extensions
{
    public static class UserHelper
    {
        public static bool IsCurrentUserOrAdmin(string id, ClaimsPrincipal user)
        {
            var currentUserId = GetCurrentUserId(user);

            // Check if the provided id is equal to the current user's id or if the current user is an admin
            return id == currentUserId || IsCurrentUserAdmin(user);
        }

        private static string GetCurrentUserId(ClaimsPrincipal user)
        {
            // Assuming you are in an ASP.NET Core context (controller, Razor Page)
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private static bool IsCurrentUserAdmin(ClaimsPrincipal user)
        {
            // Assuming you are in an ASP.NET Core context (controller, Razor Page)
            return user.IsInRole("Admin");
        }
    }
}
