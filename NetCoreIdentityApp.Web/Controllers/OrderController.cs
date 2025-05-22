using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreIdentityApp.Web.Controllers
{
    public class OrderController : Controller
    {
        [Authorize(Policy = "Permission.Order.Read")]
        public ActionResult Index()
        {
            return View();
        }

    }
}
