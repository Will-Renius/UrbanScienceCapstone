using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrbanScienceCapstone.Models
{
    public class Keywords
    {
        public int Id { get; set; }
        public List<string> KeyPhrases {get; set;}
    }

    public class Document
    {
        public List<string> keyPhrases { get; set; }
        public string id { get; set; }
    }

    public class RootObject
    {
        public List<Document> documents { get; set; }
        public List<object> errors { get; set; }
    }
    public class Search
    {
        public string search { get; set; }
    }
}
