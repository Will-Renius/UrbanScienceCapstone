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
        //currently just using localhost url -- once i publish the api ill change it.
        const string uriBase = "http://localhost:65007/api/cognitiveservice";

        static public async Task<List<string>> Keywords(string search)
        {
            try
            {
                var client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();
                //add any default headers below this

                HttpResponseMessage response = await client.GetAsync(uriBase);
                string json_string = await response.Content.ReadAsStringAsync();
                List<string> keywords = JsonConvert.DeserializeObject<List<string>>(json_string);

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
