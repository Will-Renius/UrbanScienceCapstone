using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//non default
using UrbanScienceCapstone.Models;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UrbanScienceCapstone.Controllers
{
    public class HomeController : Controller
    {
        const string urlBase = "http://localhost:65007/api/keywords";

        // GET: /<controller>/
        //swtichted iactionresult to ation result, may want to switch back
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(Search model)
        {
            if (ModelState.IsValid)
            {
                //TODO: SubscribeUser(model.Email);
            }

            //for some reason if I call our client from this function it says: "Search not found"
            // My assumption is that this funtion is the one that takes it to the new page
            //      so leaving for now
            return RedirectToAction("Keywords", "Home", new { search = model.search });

        }

        public async Task<ActionResult> Keywords(string search)
        {
            try
            {
                string url = urlBase + "?query=" + Uri.EscapeDataString(search);

                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                //Convert into keywords
                string json_string = await response.Content.ReadAsStringAsync();
                List<string> keywords = JsonConvert.DeserializeObject<List<string>>(json_string);

                if (keywords.Count == 0)
                {
                    keywords.Add("No keywords identified");
                }
                ViewBag.keyPhrases = keywords;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View();
        }
    }
}
