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
            // here is where we'll call our kpi relevence from the web api
            // we'll send it our model.search and it will return JSON
            // might have trouble with asynch since we have to wait for api call, not sure if we can return a redirect
            string test_json = "{'kpi_list': [{'name': 'sales effectiveness', 'value': 10, 'priority': 0.87}, {'name': 'pump in', 'value':20, 'priority': 0.67}]}";
            JObject return_json = JObject.Parse(test_json);
            KpiList kpi_list_object = new KpiList();
            kpi_list_object = return_json.ToObject<KpiList>();

            TempData["kpi_list"] = JsonConvert.SerializeObject(kpi_list_object);
            

            return RedirectToAction("KPI", "Home", new { test = "hello world" });

        }

        public ActionResult KPI(string test)
        {
            //if (TempData["kpi_list"] == null)
            //{

            //   throw some error
            //}
            // re-assign tempdata
            TempData["kpi_list"] = TempData["kpi_list"].ToString();

            KpiList kpi_list_object = JsonConvert.DeserializeObject<KpiList>(TempData["kpi_list"].ToString());
            ViewBag.kpi_list = kpi_list_object.kpi_list;
            return View();


        }
        public ActionResult ActionResponse(string kpi_name, int kpi_value)
        {
            // we should still have kpi_list stored in session
            if (TempData["kpi_list"] != null)
            {
                KpiList kpi_list_object = JsonConvert.DeserializeObject<KpiList>(TempData["kpi_list"].ToString());
                ViewBag.kpi_list = kpi_list_object.kpi_list;
            }
            else
            {
                ViewBag.kpi_list = new List<Kpi>();
            }

            //KpiList kpi_list_object = JsonConvert.DeserializeObject<KpiList>(TempData["kpi_list"].ToString());

            // call web api to get action sending it kpi information
            // might want to add more values in action so assuming json
            string action_sample = "{'text' : 'increase inventory'}";
            KpiAction action_to_take = JsonConvert.DeserializeObject<KpiAction>(action_sample);
            //
            ViewBag.action = action_to_take;
            ViewBag.kpi_name = kpi_name;
            ViewBag.kpi_value = kpi_value;
            return View();


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
