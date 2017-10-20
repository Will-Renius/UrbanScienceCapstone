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
    }
    public class Search
    {
        public string search { get; set; }
    }

    public class LoginInfo
    {
        public string dealerid { get; set; }
        public string password { get; set; }
    }

    public class UpdateLuis
    {
        //public List<string> intent_list { get; set; }
        //public 
        //public UpdateLuis()
        //{
        //    intent_list = new List<string>(new string[] { "Dealer Effectiveness", "Insell", "Dealer Sales" }); ;
        //}
        private List<Intent> _flavors;
        
        [Display(Name = "Favorite Flavor")]
        public int SelectedFlavorId { get; set; }

        public IEnumerable<SelectListItem> FlavorItems
        {
            get { return new SelectList(_flavors, "Id", "Name"); }
        }
        public UpdateLuis()
        {
            Intent flavor1 = new Intent();
            Intent flavor2 = new Intent();
            flavor1.Id = 1;
            flavor1.Name = "chocolate";
            flavor2.Id = 2;
            flavor2.Name = "vanilla";
            _flavors.Add(flavor1);
            _flavors.Add(flavor2);
        }
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
