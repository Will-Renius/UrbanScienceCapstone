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
    /*
 * This class demonstrates how to get a valid O-auth token.
 */
    public class Authentication
    {
        public static readonly string FetchTokenUri = "https://api.cognitive.microsoft.com/sts/v1.0";
        private string subscriptionKey;
        private string token;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public Authentication(string subscriptionKey)
        {
            this.subscriptionKey = subscriptionKey;
            this.token = FetchToken(FetchTokenUri, subscriptionKey).Result;

            // renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken()
        {
            return this.token;
        }

        private void RenewAccessToken()
        {
            this.token = FetchToken(FetchTokenUri, this.subscriptionKey).Result;
            Console.WriteLine("Renewed token.");
        }

        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        private async Task<string> FetchToken(string fetchUri, string subscriptionKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                UriBuilder uriBuilder = new UriBuilder(fetchUri);
                uriBuilder.Path += "/issueToken";

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
                return await result.Content.ReadAsStringAsync();
            }
        }
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
            /*
             * //this is where we want to hit our API to identify keywords.

            //for some reason if I call our client from this function it says: "Search not found"
            // My assumption is that this funtion is the one that takes it to the new page
            //      so leaving for now
            return RedirectToAction("Keywords", "Home", new { search = model.search });
             * /


        }

        public async Task<ActionResult> Keywords(string search)
        {
            List<Document> documents = new List<Document>();
            documents.Add(new Document() { language = "en", id = "1", text = search });

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

       
        public ActionResult SpeechToText(string file_url)
        {
            //args[0] = "https://speech.platform.bing.com/recognize";
            string args_0 = "https://speech.platform.bing.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US";
            string args_1 = file_url;


            // Note: Sign up at http://www.projectoxford.ai to get a subscription key.  Search for Speech APIs from Azure Marketplace.  
            // Use the subscription key as Client secret below.
            Authentication auth = new Authentication("12db281210d349b8beb823e44e49543b");

            string requestUri = args_0;/*.Trim(new char[] { '/', '?' });*/

            string host = @"speech.platform.bing.com";
            string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";

            /*
             * Input your own audio file or use read from a microphone stream directly.
             */
            string audioFile = args_1;
            string responseString;
            FileStream fs = null;

            try
            {
                var token = auth.GetAccessToken();
                Console.WriteLine("Token: {0}\n", token);
                Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);

                HttpWebRequest request = null;
                request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.SendChunked = true;
                request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version11;
                request.Host = host;
                request.ContentType = contentType;
                request.Headers["Authorization"] = "Bearer " + token;

                using (fs = new FileStream(audioFile, FileMode.Open, FileAccess.Read))
                {

                    /*
                     * Open a request stream and write 1024 byte chunks in the stream one at a time.
                     */
                    byte[] buffer = null;
                    int bytesRead = 0;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        /*
                         * Read 1024 raw bytes from the input audio file.
                         */
                        buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }

                        // Flush
                        requestStream.Flush();
                    }

                    /*
                     * Get the response from the service.
                     */
                    Console.WriteLine("Response:");
                    using (WebResponse response = request.GetResponse())
                    {
                        Console.WriteLine(((HttpWebResponse)response).StatusCode);

                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            responseString = sr.ReadToEnd();
                        }

                        Console.WriteLine(responseString);
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            ViewBag.text = "Hello world";
            return View();
        }
    }
}
