/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracking.Models.DTO;

namespace TimeTracking.Models.Repository
{
    /// <summary>
    /// Repository to save Invoice specific objects
    /// </summary>
    public class InvoiceRepository
    {
        Dictionary<Int64, Invoicedto> invoiceRepository = null;
        Controller invoiceController = null;
        public InvoiceRepository()
        {
            invoiceRepository = new Dictionary<Int64, Invoicedto>();
        }
        /// <summary>
        /// Save the object to dictionary
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="invoicedto"></param>
        /// <returns></returns>
        internal Invoicedto Save(object controller, Invoicedto invoicedto)
        {
            invoiceController = controller as Controller;
            Random random = new Random();
            invoicedto.Id = random.Next(1, 100);
            invoiceRepository.Add(invoicedto.Id, invoicedto);
            invoiceController.TempData["Invoice"] = invoiceRepository;
            invoiceController.TempData.Keep();
            return invoicedto;
        }
        /// <summary>
        /// Retrieve the object from dictionary
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Invoicedto Get(object controller, Int64 id)
        {
            invoiceController = controller as System.Web.Mvc.Controller;
            Dictionary<Int64, Invoicedto> invoiceRepo = invoiceController.TempData["Invoice"] as Dictionary<Int64, Invoicedto>;
            invoiceController.TempData.Keep();
            return invoiceRepo[id];
        }
       
    }
}