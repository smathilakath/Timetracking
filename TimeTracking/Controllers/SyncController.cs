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
using TimeTracking.Models.Service;

namespace TimeTracking.Controllers
{
    public class SyncController : BaseController
    {
        // GET: Sync
        SyncService syncService = null;
        Syncdto syncObjects = null;
        /// <summary>
        /// Sync the employee from backend to the QBO server
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Employee(Int64 id)
        {
            OAuthorizationdto oAuthDetails = new OAuthService(new OAuthorizationdto()).GetAccessToken(this);
            syncService = new SyncService(oAuthDetails);
            syncObjects = id>0?syncService.GetSyncObjects(this,id):new  Syncdto();
            syncObjects.OauthToken = oAuthDetails;
            syncObjects.CompanyId = oAuthDetails.Realmid;
            
            if (!syncService.IsEmpSync(syncObjects, syncService).IsEmployeeSync)
            {
                syncObjects = syncService.GetDatafromDBEmployee(syncObjects);
                if (syncObjects.EmployeeList.Count>0)
                {
                    syncObjects = syncService.SyncEmployees(this, syncObjects);
                }
            }
            return RedirectToAction("Sync", "Home", new { id = syncObjects.Id, isConnected = oAuthDetails.IsConnected });
        }
        /// <summary>
        /// Sync the Customer from backend to the QBO server
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Customer(Int64 id)
        {
            OAuthorizationdto oAuthDetails = new OAuthService(new OAuthorizationdto()).GetAccessToken(this);
            syncService = new SyncService(oAuthDetails);
            syncObjects = id > 0 ? syncService.GetSyncObjects(this, id) : new Syncdto();
            syncObjects.OauthToken = oAuthDetails;
            syncObjects.CompanyId = oAuthDetails.Realmid;
         
            if (!syncService.IsCustSync(syncObjects, syncService).IsCustomerSync)
            {
                syncObjects = syncService.GetDatafromDBCustomer(syncObjects);
                if (syncObjects.CustomerList.Count>0)
                {
                    syncObjects = syncService.SyncCustomer(this, syncObjects);
                }
            }
            return RedirectToAction("Sync", "Home", new { id = syncObjects.Id, isConnected = oAuthDetails.IsConnected });
        }
        /// <summary>
        /// Sync the item from backend to the QBO server
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ServiceItem(Int64 id)
        {
            OAuthorizationdto oAuthDetails = new OAuthService(new OAuthorizationdto()).GetAccessToken(this);
            syncService = new SyncService(oAuthDetails);
            syncObjects = id > 0 ? syncService.GetSyncObjects(this, id) : new Syncdto();
            syncObjects.OauthToken = oAuthDetails;
            syncObjects.CompanyId = oAuthDetails.Realmid;
          
            if (!syncService.IsServiceItemSync(syncObjects, syncService).IsServiceItemSync)
            {
                syncObjects = syncService.GetDatafromDBItem(syncObjects);
                if (syncObjects.ItemList.Count>0)
                {
                    syncObjects = syncService.SyncServiceItems(this, syncObjects);
                }
            }
            return RedirectToAction("Sync", "Home", new { id = syncObjects.Id, isConnected = oAuthDetails.IsConnected });
        }
    }
}