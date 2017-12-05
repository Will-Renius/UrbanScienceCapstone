using System;
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
        //const string VDA_API_URL = "http://localhost:65007/api/";
        //url for API
        const string VDA_API_URL = "http://msufall2017virtualdealershipadviserapi.azurewebsites.net/api/";

        //variables for sessions
        const string SessionKeyDealerName = "_DealerName";
        const string SessionKeyUsername = "_Username";
        const string SessionKeyPassword = "_Password";
        const string SessionKeyFirstName = "_FirstName";
        const string SessionKeyLastName = "_LastName";

        //login page
        public ActionResult Login(bool error)
        {

            ViewBag.error = error;
            return View();
        }

        //verify valid information for login
        [HttpPost]
        public async Task<ActionResult> VerifyLogin(LoginInfo info)
        {
            //we will be receiving Content-Type = application/x-www-form-urlencoded

            //https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/sending-html-form-data-part-1
            //https://www.exceptionnotfound.net/asp-net-mvc-demystified-modelstate/

            if (ModelState.IsValid && !string.IsNullOrEmpty(info.dealerid) && !string.IsNullOrEmpty(info.password))
            {

                string url = $"{VDA_API_URL}/VerifyLogin?username={Uri.EscapeDataString(info.dealerid)}&password={Uri.EscapeDataString(info.password)}";
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);
                string json_string = await response.Content.ReadAsStringAsync();


                if (response.StatusCode == HttpStatusCode.OK)
                {

                    LoginVerification login_credentials = JsonConvert.DeserializeObject<LoginVerification>(json_string);
                    if (login_credentials.validUser)
                    {
                        HttpContext.Session.SetString(SessionKeyDealerName, login_credentials.dealer_name);
                        HttpContext.Session.SetString(SessionKeyUsername, login_credentials.username);
                        HttpContext.Session.SetString(SessionKeyFirstName, login_credentials.first_name);
                        HttpContext.Session.SetString(SessionKeyLastName, login_credentials.last_name);

                        HttpContext.Session.SetString(SessionKeyPassword, info.password);

                        
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        return RedirectToAction("Login", "Home", new { error = true });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Home", new { error = true });
                }

            }
            else
            {
                return RedirectToAction("Login", "Home", new { error = true });
            }
        }


        //index page, displays search
        public ActionResult Index()
        {
            string dealer_name = HttpContext.Session.GetString(SessionKeyDealerName);
            ViewBag.first_name = HttpContext.Session.GetString(SessionKeyFirstName);
            ViewBag.last_name = HttpContext.Session.GetString(SessionKeyLastName);

            ViewBag.dealer_name = dealer_name;
            
            return View();
        }


        //display KPI results page
        public async Task<ActionResult> KPI(string search)
        {
            List<Intent> intent_list = new List<Intent>
            {
                new Intent(){ Id = 1, Name = "Dealer Share", Utterance=search },
                new Intent(){ Id = 1, Name = "Dealer Effectiveness", Utterance=search },
                new Intent(){ Id = 1, Name = "Dealer Sales", Utterance=search },
                new Intent(){ Id = 1, Name = "Insell", Utterance=search },
                new Intent(){ Id = 1, Name = "None", Utterance=search }
            };
            ViewBag.SelectedValue = "None";
           
            ViewBag.ListOfIntents = intent_list;
            ViewBag.search = search;

            //get the most related kpi
            string dealer_name = HttpContext.Session.GetString(SessionKeyDealerName);
            string username = HttpContext.Session.GetString(SessionKeyUsername);
            string password = HttpContext.Session.GetString(SessionKeyPassword);

            ViewBag.dealer_name = dealer_name;

            //check if admin
            string login_url = $"{VDA_API_URL}/VerifyLogin?username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}";
            var credentials_client = new HttpClient();

            credentials_client.DefaultRequestHeaders.Accept.Clear();
            //add any default headers below this
            credentials_client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage credentials_response = await credentials_client.GetAsync(login_url);

            ViewBag.IsAdmin = false;

            if (credentials_response.StatusCode == HttpStatusCode.OK)
            {
                string json_string = await credentials_response.Content.ReadAsStringAsync();
                LoginVerification login = JsonConvert.DeserializeObject<LoginVerification>(json_string);
                ViewBag.IsAdmin = login.isAdmin;
            }

            // do related kpi

            string related_kpi_url = $"{VDA_API_URL}/RelatedKpi?query={search}&dealer_name={dealer_name}";
            var related_kpi_client = new HttpClient();

            related_kpi_client.DefaultRequestHeaders.Accept.Clear();
            //add any default headers below this
            related_kpi_client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage related_kpi_response = await related_kpi_client.GetAsync(related_kpi_url);

            if (related_kpi_response.StatusCode == HttpStatusCode.OK)
            {
                string json_string = await related_kpi_response.Content.ReadAsStringAsync();

                Kpi most_related_kpi = JsonConvert.DeserializeObject<Kpi>(json_string);
                ViewBag.most_related_kpi = most_related_kpi;
            }



            // call web api to get action sending it kpi information
            List<Kpi> most_needed_kpis = new List<Kpi>();

            
            string needed_kpi_url = $"{VDA_API_URL}/NeededKpi?dealer_name={Uri.EscapeDataString(dealer_name)}";
            var needed_kpi_client = new HttpClient();

            needed_kpi_client.DefaultRequestHeaders.Accept.Clear();
            //add any default headers below this
            needed_kpi_client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage needed_kpi_response = await needed_kpi_client.GetAsync(needed_kpi_url);
            
            if(needed_kpi_response.StatusCode == HttpStatusCode.OK)
            {
                string json_string = await needed_kpi_response.Content.ReadAsStringAsync();
                most_needed_kpis = JsonConvert.DeserializeObject<List<Kpi>>(json_string);
                ViewBag.needed_kpi_list = most_needed_kpis;
            }

            
            return View();


        }

        //Display action result page
        public async Task<ActionResult> ActionResponse(string kpi_name, int kpi_value, double kpi_p_val)
        {
            string dealer_name = HttpContext.Session.GetString(SessionKeyDealerName);

            ViewBag.dealer_name = dealer_name;

            


            List<KpiAction> actions_to_take = new List<KpiAction>();

            string url = $"{VDA_API_URL}/Actions?name={Uri.EscapeDataString(kpi_name)}&value={kpi_p_val}";
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            //add any default headers below this
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                string json_string = await response.Content.ReadAsStringAsync();

                actions_to_take = JsonConvert.DeserializeObject<List<KpiAction>>(json_string);
            }
           

         


            ViewBag.action_list = actions_to_take;
            ViewBag.kpi_name = kpi_name;
            ViewBag.kpi_value = kpi_value;
            ViewBag.kpi_p_val = kpi_p_val;
            return View();



        }
       
    }
}
