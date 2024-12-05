namespace LitConnect.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        switch (statusCode)
        {
            case 404:
                return View("NotFound");
            case 401:
                return RedirectToPage("/Identity/Account/Login");
            default:
                return View("Error");
        }
    }
}