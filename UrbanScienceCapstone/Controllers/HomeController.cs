using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//non default
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using UrbanScienceCapstone.Models;
using Newtonsoft.Json.Linq;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UrbanScienceCapstone.Controllers
{
    struct Document
    {
        public string language;
        public string id;
        public string text;
    }

    public class HomeController : Controller
    {
        const string subscriptionKey = "fc2944a69ec44b03939f48cf7a7a0ad3";
        const string uriBase = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases";

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
            return RedirectToAction("Keywords", "Home", new { search = model.search });

        }

        public async Task<ActionResult> Keywords(string search)
        {
            List<Document> documents = new List<Document>();
            documents.Add(new Document() { language = "en", id = "1", text = search});

            List<Keywords> KeywordInfo = new List<Keywords>();
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            HttpResponseMessage response;

            // Compose request.
            string body = "";
            foreach (Document doc in documents)
            {
                if (!string.IsNullOrEmpty(body))
                {
                    body = body + ",";
                }

                body = body + "{ \"language\": \"" + doc.language + "\", \"id\":\"" + doc.id + "\",  \"text\": \"" + doc.text + "\"   }";
            }

            body = "{  \"documents\": [" + body + "] }";

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uriBase, content);
            }

            // Get the JSON response
            string result = await response.Content.ReadAsStringAsync();
            JObject result2 = JObject.Parse(result);
            //Deserializing the response recieved from web api and storing into the Employee list 
            RootObject testdes = new RootObject();

            //testdes = JsonConvert.DeserializeObject<RootObject>(result);
            testdes = result2.ToObject<RootObject>();
            ViewBag.keyPhrases = testdes.documents[0].keyPhrases;
            return View();
        }
    }
}
