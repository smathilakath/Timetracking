using Intuit.Ipp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeTracking.Models.DTO
{
    public class TimeActivitydto
    {
        public List<Employee> EmployeeList { get; set; }
        public List<Customer> CustomerList { get; set; }
        public List<Item> ItemList { get; set; }
        public Syncdto Syncdto { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }
        public string QboId { get; set; }
        [Required]
        public IEnumerable<SelectListItem> Employee { get; set; }
        public string EmployeeSelected { get; set; }
        public IEnumerable<SelectListItem> Customer { get; set; }
        public string CustomerSelected { get; set; }
        public IEnumerable<SelectListItem> Item { get; set; }
        public string ItemSelected { get; set; }
        public OAuthorizationdto oAuthTokens { get; set; }

        public DateTime Date { get; set; }
        public int CompanyId { get; set; }

        public long Id { get; set; }

        public string AlertMessage { get; set; }

        public int Hours { get; set; }

        public DateTime TxnDate { get; set; }

        public List<TimeActivityFill> FillTime { get; set; }
    }

    public class TimeActivityFill
    {
        public string Employee { get; set; }
        public string Customer { get; set; }
        public string Item { get; set; }
        public string Date { get; set; }
        public string Hours { get; set; }
        public string QboId { get; set; }
    }
}