/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * 
 * 
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
    /// <summary>
    /// Home controller is responsible for OAuth and Sync operation.
    /// </summary>
    public class HomeController : BaseController
    {
        Multiplemodels multiplemodels = null;
        SyncRepository syncRepo = new SyncRepository();
        /// <summary>
        /// Intitalise multiple models and kick start oauth/sync
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            multiplemodels = new Multiplemodels();
            multiplemodels.SyncObjectsModel = new Syncdto();
            multiplemodels.OAuthorizationModel = new OAuthorizationdto();
            multiplemodels.TimeActivityModel = new TimeActivitydto();
            multiplemodels.IsReadySync = false;
            var oAuthModel = new OAuthService(multiplemodels.OAuthorizationModel).IsTokenAvailable(this);
            if (oAuthModel.IsConnected)
            {
                multiplemodels.IsReadySync = true;
                multiplemodels.OAuthorizationModel = oAuthModel;
                multiplemodels.IsConnected = oAuthModel.IsConnected;
                var syncService = new SyncService(oAuthModel);
                multiplemodels.SyncObjectsModel.OauthToken = oAuthModel;
                multiplemodels.SyncObjectsModel = syncService.IsEmpSync(multiplemodels.SyncObjectsModel, syncService);
                multiplemodels.SyncObjectsModel = syncService.IsCustSync(multiplemodels.SyncObjectsModel, syncService);
                multiplemodels.SyncObjectsModel = syncService.IsServiceItemSync(multiplemodels.SyncObjectsModel, syncService);
                multiplemodels.SyncObjectsModel.CompanyId = oAuthModel.Realmid;
                multiplemodels.SyncObjectsModel = syncRepo.Save(this, multiplemodels.SyncObjectsModel);
                multiplemodels.IsReadyTimeentry = multiplemodels.SyncObjectsModel.IsEmployeeSync || multiplemodels.SyncObjectsModel.IsCustomerSync || multiplemodels.SyncObjectsModel.IsServiceItemSync;
                multiplemodels.IsReadytoInvoice = false;
                return View(multiplemodels);
            }
            else
            {
                return View(multiplemodels);
            }
        }
      
        /// <summary>
        /// Changes made
        /// </summary>
        /// <returns></returns>
        public ActionResult Close()
        {
            multiplemodels = new Multiplemodels();
            multiplemodels.OAuthorizationModel = new OAuthorizationdto();
            multiplemodels.SyncObjectsModel = new Syncdto();
            multiplemodels.TimeActivityModel = new TimeActivitydto();
            return View(multiplemodels);
        }
        /// <summary>
        /// Render the sync operations.
        /// </summary>
        /// <param name="id">identifier for repo</param>
        /// <param name="isConnected">oauth status</param>
        /// <returns></returns>
        public ActionResult Sync(int id, bool isConnected)
        {
            multiplemodels = new Multiplemodels();
            if (multiplemodels != null)
            {
                multiplemodels.SyncObjectsModel = new SyncRepository().Get(this, id);
                multiplemodels.OAuthorizationModel = new OAuthorizationdto();
                multiplemodels.OAuthorizationModel.IsConnected = isConnected;
                multiplemodels.TimeActivityModel = new TimeActivitydto();
                multiplemodels.IsReadyTimeentry = true;
                return View("Index", multiplemodels);
            }
            else
            {
                return View("Index");
            }
        }
    }
}