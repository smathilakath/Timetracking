/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracking.Models;
using TimeTracking.Models.DTO;
using TimeTracking.Models.Repository;
using TimeTracking.Models.Service;

namespace TimeTracking.Controllers
{
    public class InvoiceController : BaseController
    {
        InvoiceService invoiceService = null;
        Invoicedto invoicedto = new Invoicedto();
        InvoiceRepository invoiceRepository = null;
        Multiplemodels multiplemodels = null;
        
        // GET: Invoice
        /// <summary>
        /// Load the Invoice with Pending/Created state.
        /// </summary>
        /// <param name="id">repo identifier</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Load(Int64 id)
        {
            TimeActivitydto timeActivitydto = id > 0 ? new TimeActivityRepository().Get(this, id) : new TimeActivitydto();
            invoicedto.oAuthTokens = timeActivitydto.oAuthTokens;
            invoicedto.CustomerList = timeActivitydto.CustomerList;
            invoicedto.CompanyId = invoicedto.oAuthTokens.Realmid;
            invoicedto.ConnectionString = timeActivitydto.Syncdto.ConnectionString;
            invoicedto.TimeActivityDto = timeActivitydto;
            invoiceService = new InvoiceService(invoicedto);
            invoicedto = invoiceService.LoadPending(invoicedto);
            invoicedto = invoiceService.LoadInvoiced(invoicedto);
            invoiceRepository = new InvoiceRepository();
            invoicedto = invoiceRepository.Save(this, invoicedto);
            multiplemodels = new Multiplemodels();
            multiplemodels.TimeActivityModel = timeActivitydto;
            multiplemodels.InvoiceModel = invoicedto;
            multiplemodels.SyncObjectsModel = invoicedto.TimeActivityDto.Syncdto;
            multiplemodels.IsConnected = timeActivitydto.oAuthTokens.IsConnected;
            multiplemodels.IsReadyTimeentry = true;
            multiplemodels.IsReadytoInvoice = true;
            multiplemodels.IsReadySync = false;
            return View("Invoices", multiplemodels);
        }
        /// <summary>
        /// Save the invoice to the qbo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="qboId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(Int64 id, Int64 qboId)
        {
            Invoicedto invoicedto = id > 0 ? new InvoiceRepository().Get(this, id) : new Invoicedto();
            invoicedto.timeQboId = qboId;
            invoiceService = new InvoiceService(invoicedto);
            invoicedto = invoiceService.GenerateInvoice(invoicedto);
            invoicedto = invoiceService.UpdateDatabase(invoicedto);
            //invoicedto = invoiceService.FillCreatedInvoice(invoicedto);
            invoiceRepository = new InvoiceRepository();
            invoicedto = invoiceRepository.Save(this, invoicedto);
            object data = new
            {
                Id = invoicedto.Id
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// render the view post invoice generation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult View(Int64 id)
        {
            Invoicedto invoicedto = id > 0 ? new InvoiceRepository().Get(this, id) : new Invoicedto();
            invoiceService = new InvoiceService(invoicedto);
            invoicedto = invoiceService.LoadPending(invoicedto);
            invoicedto = invoiceService.LoadInvoiced(invoicedto);
            multiplemodels = new Multiplemodels();
            multiplemodels.TimeActivityModel = invoicedto.TimeActivityDto;
            multiplemodels.InvoiceModel = invoicedto;
            multiplemodels.SyncObjectsModel = invoicedto.TimeActivityDto.Syncdto;
            multiplemodels.IsReadySync = multiplemodels.SyncObjectsModel.IsEmployeeSync | multiplemodels.SyncObjectsModel.IsCustomerSync | multiplemodels.SyncObjectsModel.IsServiceItemSync;
            multiplemodels.IsReadyTimeentry = true;
            multiplemodels.IsReadytoInvoice = false;
            return View("Invoices", multiplemodels);
        }
    }
}