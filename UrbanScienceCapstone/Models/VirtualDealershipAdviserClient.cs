using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace UrbanScienceCapstone.Models
{
    static class VirtualDealershipAdviserClient
    {
        public struct UserData
        {
            long Id;
            string Query;
        }

        //currently just using localhost url -- once i publish the api ill change it.
        const string uriBase = "http://localhost:65007/api/keywords";

        


        static public async Task<List<string>> Keywords(string search)
        {
            try
            {
                string url = uriBase + "?query=" + Uri.EscapeDataString(search);

                var client = new HttpClient();
                
                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(HttpUtility.UrlEncode(url));
                string json_string = await response.Content.ReadAsStringAsync();
                List<string> keywords = JsonConvert.DeserializeObject<List<string>>(json_string);

                if(keywords.Count == 0)
                {
                    keywords.Add("No keywords identified");
                }
                return keywords;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
