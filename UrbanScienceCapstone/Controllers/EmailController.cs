using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UrbanScienceCapstone.Controllers
{
    public class EmailController : Controller
    {
        // GET: /<controller>/
        public IActionResult ShareAction(string text, string kpi)
        {
            return View();
        }
    }
}
