using Microsoft.AspNetCore.Mvc;

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
    }
}
