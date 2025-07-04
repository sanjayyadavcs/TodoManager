using Microsoft.AspNetCore.Mvc;

namespace TodoManager.MVC.Helpers
{
    public static class Extensions
    {
        public static string ActingUserName(this ControllerBase controller)
        {
            //return "admin";
            if (controller.User?.Identity?.IsAuthenticated == true)
            {
                return controller.User.Identity.Name ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
