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
            string related_kpi_url = "http://localhost:65007/api/RelatedKpi";
            //string related_kpi_url = "http://virtualdealershipadvisorapi.azurewebsites.net/api/kpi";
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
            // get the most needed kpi_list
            // delete when dan is done
            KpiList most_needed_kpis = new KpiList();

            Kpi test_kpi1 = new Kpi();
            test_kpi1.name = "Sales Effectiveness";
            test_kpi1.value = 20;
            most_needed_kpis.kpi_list.Add(test_kpi1);

            Kpi test_kpi2 = new Kpi();
            test_kpi2.name = "Pump In";
            test_kpi2.value = 30;
            most_needed_kpis.kpi_list.Add(test_kpi2);

            Kpi test_kpi3 = new Kpi();
            test_kpi2.name = "Dealer Share";
            test_kpi2.value = 35;
            most_needed_kpis.kpi_list.Add(test_kpi3);

            ViewBag.needed_kpi_list = most_needed_kpis.kpi_list;
            return View();


        }
        public ActionResult ActionResponse(string kpi_name, int kpi_value)
        {
            

            //KpiList kpi_list_object = JsonConvert.DeserializeObject<KpiList>(TempData["kpi_list"].ToString());

            // call web api to get action sending it kpi information
            // might want to add more values in action so assuming json
            string action_sample = "{'text' : 'increase inventory'}";
            KpiAction action_to_take = JsonConvert.DeserializeObject<KpiAction>(action_sample);
            //
            List<KpiAction> action_list = new List<KpiAction>();
            KpiAction action1 = new KpiAction();
            action1.text = "Your sales[for this segment /in this area / etc] are good.Use Premium MMO to take a look at your sales effectiveness to determine the level of additional opportunity.";
            action_list.Add(action1);

            KpiAction action2 = new KpiAction();
            action2.text = "Evaluate Dealer Business Intelligence to determine how your profit compares to other dealers.";
            action_list.Add(action2);

            KpiAction action3 = new KpiAction();
            action3.text = "Leverage Sales Leads to find additional interest in your area.";
            action_list.Add(action3);

            KpiAction action4 = new KpiAction();
            action4.text = "Optimize your inventory mix to meet consumer demand in your area.";
            action_list.Add(action4);

            ViewBag.action_list = action_list;
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
