/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeTracking.Models.Repository
{
    /// <summary>
    /// Repository to save Sync specific objects
    /// </summary>
    public class SyncRepository
    {
        Dictionary<Int64, Syncdto> syncRepo = null;
        Controller syncController = null;
        public SyncRepository()
        {
            syncRepo = new Dictionary<Int64, Syncdto>();
        }
        /// <summary>
        /// Retrieve the object from dictionary
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Syncdto Get(object controller, Int64 id)
        {
            syncController = controller as System.Web.Mvc.Controller;
            Dictionary<Int64, Syncdto> syncRepo = syncController.TempData["Sync"] as Dictionary<Int64, Syncdto>;
            syncController.TempData.Keep();
            if (syncRepo.ContainsKey(id))
            {
                return syncRepo[id];
            }
            else
            {

                return syncRepo[syncRepo.First().Key];
            }
        }
        /// <summary>
        /// Save the object to dictionary
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="syncObjects"></param>
        /// <returns></returns>
        internal Syncdto Save(object controller, Syncdto syncObjects)
        {
            syncController = controller as Controller;
            Random random = new Random();
            syncObjects.Id = random.Next(1,100);
            syncRepo.Add(syncObjects.Id, syncObjects);
            syncController.TempData["Sync"] = syncRepo;
            syncController.TempData.Keep();
            return syncObjects;
        }
    }
}