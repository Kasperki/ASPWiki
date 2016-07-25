using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ASPWiki
{
    static class ControllerHelper
    {
        #region FlashMessages
        public static void FlashMessageError(this Controller controller, string message)
        {
            controller.TempData["FlashMessage"] = message;
            controller.TempData["FlashMessageClass"] = "alert-danger";
        }

        public static void FlashMessageInfo(this Controller controller, string message)
        {
            controller.TempData["FlashMessage"] = message;
            controller.TempData["FlashMessageClass"] = "alert-info";
        }

        public static void FlashMessageSuccess(this Controller controller, string message)
        {
            controller.TempData["FlashMessage"] = message;
            controller.TempData["FlashMessageClass"] = "alert-success";
        }

        public static void FlashMessageWarning(this Controller controller, string message)
        {
            controller.TempData["FlashMessage"] = message;
            controller.TempData["FlashMessageClass"] = "alert-warning";
        }
        #endregion

        public static string GetModelStateErrors(this Controller controller)
        {
            string errorString = string.Empty;
            foreach (var modelState in controller.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errorString += error.ErrorMessage;
                }
            }

            return errorString;
        }

        public static string GetLastItemFromPath(this Controller controller, string path)
        {
            var paths = path?.Split('/');
            return paths?[paths.Length - 1];
        }
    }
}
