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
            //string uriBase2 = "http://localhost:65007/api/kpi";
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
                //Kpi testKpi = JsonConvert.DeserializeObject<Kpi>(json_string);


                //ViewBag.testKpi = testKpi;
                KpiList kpi_list_object = JsonConvert.DeserializeObject<KpiList>(json_string);
                ViewBag.kpi_list = kpi_list_object.kpi_list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

            
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
