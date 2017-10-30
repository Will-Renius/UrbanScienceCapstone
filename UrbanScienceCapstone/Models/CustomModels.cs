using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class KpiList
    {
        public List<Kpi> kpi_list { get; set; }
        public KpiList()
        {
            kpi_list = new List<Kpi>();
        }
    }
    public class Kpi
    {
        public string name { get; set; }
        public int  value { get; set; }
        public double p_val { get; set; }
        public string segment { get; set; }
        public string brand { get; set; }
    }
    public class KpiAction
    {
        public string actionP { get; set; }
        public string kpi { get; set; }
        public string type { get; set; }
        public string actionLink { get; set; }
    }
    

    public class Search
    {
        public string search { get; set; }
    }

    public class LoginVerification
    {
        public bool isAdmin { get; set; }
        public bool validUser { get; set; }
    }
    public class LoginInfo
    {
        public string dealerid { get; set; }
        public string password { get; set; }
    }

    public class UpdateLuis
    {
        [Display(Name = "KPI")]
        public string intentName { get; set; }
        public string text { get; set; }
        public List<EntityLabel> entityLabels { get; set; }

    }
    public class EntityLabel
    {
        public string entityName { get; set; }
        public int startCharIndex { get; set; }
        public int endCharIndex { get; set; }
    }
    public class Intent
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "KPI")]
        public string Name { get; set; }
        public string Utterance { get; set; }
    }
}
