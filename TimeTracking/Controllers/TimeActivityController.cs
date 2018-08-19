/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using Intuit.Ipp.Data;
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
    /// <summary>
    /// 
    /// </summary>
    public class TimeActivityController : BaseController
    {
        // GET: TimeActivity
        TimeActivityService timeActivityService = null;
        TimeActivitydto timeActivity = new TimeActivitydto();
        TimeActivityRepository timeActivityRepository = null;
        Multiplemodels multiplemodels = null;
        /// <summary>
        /// Load the timeentry from the backend.
        /// </summary>
        /// <param name="id">repo identifier</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Load(Int64 id)
        {
            Syncdto syncDetails = id > 0 ? new SyncRepository().Get(this, id) : new Syncdto();
            timeActivity.oAuthTokens = syncDetails.OauthToken;
            timeActivity.Syncdto = syncDetails;
            timeActivity.EmployeeList = syncDetails.EmployeeList;
            timeActivity.CustomerList = syncDetails.CustomerList;
            timeActivity.ItemList = syncDetails.ItemList;
            timeActivityService = new TimeActivityService(timeActivity);
            timeActivity = timeActivityService.LoaddropdownList(timeActivity);
            timeActivity.CompanyId = timeActivity.oAuthTokens.Realmid;
            timeActivityRepository = new TimeActivityRepository();
            timeActivityRepository.Save(this, timeActivity);
            multiplemodels = new Multiplemodels();
            multiplemodels.TimeActivityModel = timeActivity;
            multiplemodels.SyncObjectsModel = syncDetails;
            multiplemodels.IsConnected = syncDetails.OauthToken.IsConnected;
            multiplemodels.IsReadytoInvoice = true;
            return View("TimeActivity", multiplemodels);
        }
        /// <summary>
        /// Save the data collected to QBO
        /// </summary>
        /// <param name="id">repo identifier</param>
        /// <param name="empSelect">employee selected</param>
        /// <param name="cusSelect">customer selected</param>
        /// <param name="itemSelect">Item selected</param>
        /// <param name="description">description selected</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(Int64 id, string empSelect, string cusSelect, string itemSelect, string description)
        {
            timeActivity = id > 0 ? new TimeActivityRepository().Get(this, id) : new TimeActivitydto();
            timeActivityService = new TimeActivityService(timeActivity);
            timeActivity.EmployeeSelected = empSelect;
            timeActivity.CustomerSelected = cusSelect;
            timeActivity.ItemSelected = itemSelect;
            timeActivity.Description = description;
            timeActivity = timeActivityService.Save(timeActivity);
            timeActivityRepository = new TimeActivityRepository();
            timeActivity = timeActivityRepository.Save(this, timeActivity);
            return GetJsonObject(timeActivity, timeActivityRepository); 
        }
        /// <summary>
        /// retueve the list item based on the entity type
        /// </summary>
        /// <param name="timeActivitydto"></param>
        /// <param name="caseString"></param>
        /// <returns></returns>
        private object ReturnListItem(TimeActivitydto timeActivitydto,string caseString)
        {
            switch (caseString)
            {
                case "emp":
                    return  timeActivitydto.EmployeeList.Where(x => x.Id == timeActivitydto.EmployeeSelected).First();
                case "cust":
                    return timeActivitydto.CustomerList.Where(x => x.Id == timeActivitydto.CustomerSelected).First();
                case "item":
                    return timeActivitydto.ItemList.Where(x => x.Id == timeActivitydto.ItemSelected).First();
            }
            return null;
        }
        /// <summary>
        /// Retrieve the JSON object to be passed to client.
        /// </summary>
        /// <param name="timeActivity"></param>
        /// <param name="timeActivityRepository"></param>
        /// <returns></returns>
        private JsonResult GetJsonObject(TimeActivitydto timeActivity,TimeActivityRepository timeActivityRepository)
        {
            try
            {
                var employeeObj = ReturnListItem(timeActivity, "emp") as Employee;
                var CustomerObj = ReturnListItem(timeActivity, "cust") as Customer;
                var ItemObj = ReturnListItem(timeActivity, "item") as Item;
                var dateObj = timeActivity.TxnDate.ToShortDateString();
                var hoursObj = timeActivity.Hours;
                var qboId = timeActivity.QboId;
                object data = new {
                       ControlId = timeActivity.Id,
                       RealmId = timeActivity.oAuthTokens.Realmid,
                       AlertMessage = timeActivity.AlertMessage,
                       Employee = string.Format("{0} {1}", employeeObj.GivenName,employeeObj.FamilyName),
                       Customer = string.Format("{0} {1}",CustomerObj.GivenName,CustomerObj.FamilyName),
                       Item = ItemObj.Name ,
                       Date=dateObj,
                       Hours=hoursObj,
                       QboId = qboId};
                timeActivityRepository.SavetoDb(timeActivity.Syncdto.ConnectionString, timeActivity);
                return Json(data,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}