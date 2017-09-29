// Hello1.cs
using System; //Console
using System.Collections.Generic; //Lists, Dictionaries
using System.Linq;
using System.Text.RegularExpressions;


namespace UrbanScienceCapstone
{
    public class VDA
    {
      private string sentence;
      private List<string> KPI_list = new List<string>();

      private Dictionary<string,string> The_D;
      //Ideally the database, dummy dictionary for now

      private List<string> SEGMENTS = new List<string>();
      //List of Segments
      private List<string> BRANDS = new List<string>();
      //List of Brands

      private string segment = "";
      private string brand = "";

      public VDA(string statement){
        sentence = statement;
      }

      public void extract_keywords(){

        string[] sentence_list = sentence.Trim().ToLower().Split(' ');
        int n = sentence_list.Length;

        for (int i = 0; i <= n; ++i){
          string ele = sentence_list[i];

          //Console.WriteLine(ele); uncomment to print lines

          if (ele == "pump"){
            //or "sell"

            if (sentence_list[i+1] == "in" || sentence_list[i+1] == "out"){
              ele = ele + " " + sentence_list[i+1];
            }
          }


          if (The_D.ContainsKey(ele)){
            //Check if element is contained inside database
            KPI_list.Add(The_D[ele]);
          }

          if (SEGMENTS.Contains(ele)){
            //Segments
            segment = ele;
          }

          if (BRANDS.Contains(ele)){
            //Brands
            brand = ele;
          }
        }

      }
        //public static void Main(string[] args){}

    }

}

//VDA vda = new VDA("Hello my name is");
//vda.extract_keywords;
