/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeTracking.Models.DTO;

namespace TimeTracking.Models
{
    /// <summary>
    /// Need to optimize the algo. This is not a efficient approach.
    /// </summary>
    public class Multiplemodels
    {
        public OAuthorizationdto OAuthorizationModel { get; set; }
        public Syncdto SyncObjectsModel { get; set; }
        public TimeActivitydto TimeActivityModel { get; set; }
        public bool IsReadyTimeentry { get; set; }
        public bool IsReadytoInvoice { get; set; }
        public bool IsReadySync { get; set; }
        public bool IsConnected { get; set; }
        public Invoicedto InvoiceModel { get; set; }
    }
}