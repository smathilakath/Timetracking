/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Intuit.Ipp.Data;
using Intuit.Ipp.QueryFilter;
namespace TimeTracking.Models
{
    /// <summary>
    /// Data transferable object for Sync operation
    /// </summary>
    public class Syncdto
    {
        public bool IsEmployeeSync { get; set; }
        public bool IsCustomerSync { get; set; }
        public bool IsServiceItemSync { get; set; }
        public OAuthorizationdto OauthToken { get; set; }
        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DBContext"].ToString(); }

        }
        public List<Employee> EmployeeList { get; set; }
        public List<Customer> CustomerList { get; set; }
        public List<Item> ItemList { get; set; }
        public Int64 Id { get; set; }
        public string QboId { get; set; }
        public int CompanyId { get; set; }
        private string viewInQbo = string.Empty;
        public string ViewInQbo { get; set; }
        public string DeepLink
        {
            get
            {
                return ConfigurationManager.AppSettings["DeepLink"];
            }

        }
       
    }
}