﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//non default
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session; //session state
using Microsoft.Extensions.Caching.Distributed;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using UrbanScienceCapstone.Models;
using Newtonsoft.Json.Linq;
using System.Web;
//speech to text
using System.Net;
using System.IO;
using System.Threading;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UrbanScienceCapstone.Controllers
{
    public class HomeController : Controller
    {

        const string subscriptionKey = "dadc20b8bf47462bb82321e581b795c6";
        const string VDA_API_URL = "http://virtualdealershipadvisorapi.azurewebsites.net/api/";
        //const string VDA_API_URL = "http://localhost:65007/api/";
        const string SessionKeyDealerId = "_DealerId";


        public ActionResult Login()
        {
            string dealer_name = HttpContext.Session.GetString(SessionKeyDealerId);
            ViewBag.dealer_name = dealer_name;
            return View();
        }
        //If we make their dealerId persist through multiple sessions, might wanna have this as the default route
        [HttpPost]
        public async Task<ActionResult> VerifyLogin(LoginInfo info)
        {
            //we will be receiving Content-Type = application/x-www-form-urlencoded
            //https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/sending-html-form-data-part-1
            //https://www.exceptionnotfound.net/asp-net-mvc-demystified-modelstate/

            if(ModelState.IsValid && !string.IsNullOrEmpty(info.dealerid))
            {
                //just copied code below calling needed kpis, definitely refactor
                List<Kpi> most_needed_kpis = new List<Kpi>();
                try
                {
                    string url = $"{VDA_API_URL}/NeededKpi?dealer_name={Uri.EscapeDataString(info.dealerid)}";
                    var client = new HttpClient();

                    client.DefaultRequestHeaders.Accept.Clear();
                    //add any default headers below this
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(url);
                    

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContext.Session.SetString(SessionKeyDealerId, info.dealerid);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        string dealer = HttpContext.Session.GetString(SessionKeyDealerId);
                        return RedirectToAction("Login", "Home");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest();
                }

            }
            else
            {
                return BadRequest();
            }
        }


        // GET: /<controller>/
        //swtichted iactionresult to ation result, may want to switch back
        public ActionResult Index()
        {
            string dealer_name = HttpContext.Session.GetString(SessionKeyDealerId);
            ViewBag.dealer_name = dealer_name;
            return View();
        }



        public async Task<ActionResult> KPI(string search)
        {
            ViewBag.search = search;
            //get the most related kpi
            //string related_kpi_url = "http://localhost:65007/api/RelatedKpi";
            string related_kpi_url = "http://virtualdealershipadvisorapi.azurewebsites.net/api/RelatedKpi";
            string dealer_name = HttpContext.Session.GetString(SessionKeyDealerId);
            ViewBag.dealer_name = dealer_name;
            if (string.IsNullOrEmpty(dealer_name))
            {
                dealer_name = "omega";
            }
            try
            {
                string url = $"{related_kpi_url}?query={search}&dealer_name={dealer_name}";
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                string json_string = await response.Content.ReadAsStringAsync();

                Kpi most_related_kpi = JsonConvert.DeserializeObject<Kpi>(json_string);
                ViewBag.most_related_kpi = most_related_kpi;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }


            // call web api to get action sending it kpi information
            //string needed_kpi_api_url = "http://localhost:65007/api/NeededKpi";
            string needed_kpi_api_url = "http://virtualdealershipadvisorapi.azurewebsites.net/api/NeededKpi";
            List<Kpi> most_needed_kpis = new List<Kpi>();

            try
            {

                string url = $"{needed_kpi_api_url}?dealer_name={Uri.EscapeDataString(dealer_name)}";
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
        public async Task<ActionResult> ActionResponse(string kpi_name, int kpi_value, double kpi_p_val)
        {
            string dealer_name = HttpContext.Session.GetString(SessionKeyDealerId);
            ViewBag.dealer_name = dealer_name;


            //KpiList kpi_list_object = JsonConvert.DeserializeObject<KpiList>(TempData["kpi_list"].ToString());

            // call web api to get action sending it kpi information
            //string action_api_url = "http://localhost:65007/api/Actions";
            string action_api_url = "http://virtualdealershipadvisorapi.azurewebsites.net/api/Actions";
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
            ViewBag.kpi_p_val = kpi_p_val;
            return View();



        }


    }
        
}
