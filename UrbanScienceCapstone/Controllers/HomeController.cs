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
//speech to text
using System.Net;
using System.IO;
using System.Threading;



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
        const string uriBase = "http://localhost:65007/api/keywords";

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

        public async Task<ActionResult> KPI(string search)
        {
            //get the most related kpi
            //string related_kpi_url = "http://localhost:65007/api/RelatedKpi";
            string related_kpi_url = "http://virtualdealershipadvisorapi.azurewebsites.net/api/RelatedKpi";
            try
            {
                string url = related_kpi_url + "?query=" + Uri.EscapeDataString(search);
                //string url = uriBase2;
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                string json_string = await response.Content.ReadAsStringAsync();
                //Kpi testKpi = JsonConvert.DeserializeObject<Kpi>(json_string);


                //ViewBag.testKpi = testKpi;

                Kpi most_related_kpi = JsonConvert.DeserializeObject<Kpi>(json_string);
                ViewBag.most_related_kpi = most_related_kpi;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
  

            // call web api to get action sending it kpi information
            string needed_kpi_api_url = "http://localhost:65007/api/NeededKpi";
            //string related_kpi_url = "http://virtualdealershipadvisorapi.azurewebsites.net/api/NeededKpi"";
            List<Kpi> most_needed_kpis = new List<Kpi>();
            try
            {
                string url = $"{needed_kpi_api_url}?dealer_name={Uri.EscapeDataString("omega")}";
                //string url = uriBase2;
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                string json_string = await response.Content.ReadAsStringAsync();
                //Kpi testKpi = JsonConvert.DeserializeObject<Kpi>(json_string);


                //ViewBag.testKpi = testKpi;

                most_needed_kpis = JsonConvert.DeserializeObject<List<Kpi>>(json_string);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

            ViewBag.needed_kpi_list = most_needed_kpis;
            return View();


        }
        public async Task<ActionResult> ActionResponse(string kpi_name, int kpi_value)
        {


            //KpiList kpi_list_object = JsonConvert.DeserializeObject<KpiList>(TempData["kpi_list"].ToString());

            // call web api to get action sending it kpi information
            string action_api_url = "http://localhost:65007/api/Actions";
            //string related_kpi_url = "http://virtualdealershipadvisorapi.azurewebsites.net/api/Actions";
            List<KpiAction> actions_to_take = new List<KpiAction>();
            try
            {
                string url = $"{action_api_url}?name={Uri.EscapeDataString(kpi_name)}&value={kpi_value}";
                //string url = uriBase2;
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                string json_string = await response.Content.ReadAsStringAsync();
                //Kpi testKpi = JsonConvert.DeserializeObject<Kpi>(json_string);


                //ViewBag.testKpi = testKpi;

                actions_to_take = JsonConvert.DeserializeObject<List<KpiAction>>(json_string);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            //
            

            ViewBag.action_list = actions_to_take;
            ViewBag.kpi_name = kpi_name;
            ViewBag.kpi_value = kpi_value;
            return View();
            /*
             * //this is where we want to hit our API to identify keywords.

            //for some reason if I call our client from this function it says: "Search not found"
            // My assumption is that this funtion is the one that takes it to the new page
            //      so leaving for now
            return RedirectToAction("Keywords", "Home", new { search = model.search });
             */


        }


        public async Task<ActionResult> Keywords(string search)
        {
            try
            {
                string url = uriBase + "?query=" + Uri.EscapeDataString(search);

                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                string json_string = await response.Content.ReadAsStringAsync();
                List<string> keywords = JsonConvert.DeserializeObject<List<string>>(json_string);

                if (keywords.Count == 0)
                {
                    keywords.Add("No keywords identified");
                }
                ViewBag.keywords = keywords;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return View();
        }
        public async Task<ActionResult> TestKPI(string search)
        {
            //string uriBase2 = "http://localhost:65007/api/Kpi";
            string uriBase2 = "http://virtualdealershipadvisorapi.azurewebsites.net/api/kpi";
            try
            {
                string url = uriBase2 + "?query=" + Uri.EscapeDataString(search);
                //string url = uriBase2;
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                string json_string = await response.Content.ReadAsStringAsync();
                Kpi testKpi= JsonConvert.DeserializeObject<Kpi>(json_string);

                
                ViewBag.testKpi = testKpi;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
            return View();
        }



        public ActionResult speechRecognition()
        {
            return View();
        }
    }
        
}
