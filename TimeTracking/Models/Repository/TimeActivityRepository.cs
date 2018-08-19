/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using Intuit.Ipp.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracking.Models.DTO;

namespace TimeTracking.Models.Repository
{
    /// <summary>
    ///  Repository to save Sync specific objects
    /// </summary>
    public class TimeActivityRepository
    {
        Dictionary<Int64, TimeActivitydto> timeActivityRepository = null;
        Controller timeController = null;
        public TimeActivityRepository()
        {
            timeActivityRepository = new Dictionary<Int64, TimeActivitydto>();
        }
        /// <summary>
        /// Retrieve the object from dictionary
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal TimeActivitydto Get(object controller, Int64 id)
        {
            timeController = controller as System.Web.Mvc.Controller;
            Dictionary<Int64, TimeActivitydto> timeRepo = timeController.TempData["TimeActivity"] as Dictionary<Int64, TimeActivitydto>;
            timeController.TempData.Keep();
            return timeRepo[id];

        }
        /// <summary>
        /// Save the object to dictionary
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="timeActivity"></param>
        /// <returns></returns>
        internal TimeActivitydto Save(object controller, TimeActivitydto timeActivity)
        {
            timeController = controller as Controller;
            Random random = new Random();
            timeActivity.Id = random.Next(1,100);
            timeActivityRepository.Add(timeActivity.Id, timeActivity);
            timeController.TempData["TimeActivity"] = timeActivityRepository;
            timeController.TempData.Keep();
            return timeActivity;
        }
        /// <summary>
        /// Retrieve based on entity type.
        /// </summary>
        /// <param name="timeActivitydto"></param>
        /// <param name="caseString"></param>
        /// <returns></returns>
        private object ReturnListItem(TimeActivitydto timeActivitydto, string caseString)
        {
            switch (caseString)
            {
                case "emp":
                    return timeActivitydto.EmployeeList.Where(x => x.Id == timeActivitydto.EmployeeSelected).First();
                case "cust":
                    return timeActivitydto.CustomerList.Where(x => x.Id == timeActivitydto.CustomerSelected).First();
                case "item":
                    return timeActivitydto.ItemList.Where(x => x.Id == timeActivitydto.ItemSelected).First();
            }
            return null;
        }
        /// <summary>
        /// Save the objects to SQL
        /// </summary>
        /// <param name="conString"></param>
        /// <param name="timeActivity"></param>
        internal void SavetoDb(string conString, TimeActivitydto timeActivity)
        {
            var employeeObj = ReturnListItem(timeActivity, "emp") as Employee;
            var CustomerObj = ReturnListItem(timeActivity, "cust") as Customer;
            var ItemObj = ReturnListItem(timeActivity, "item") as Item;
            var dateObj = timeActivity.TxnDate;
            var hoursObj = timeActivity.Hours;
            var qboId = timeActivity.QboId;
            string query = "INSERT INTO [TimeActivity] ([RealmId], [Employee], [Customer], [Item], [Date],  [Hours], [QboId])";
            query += " VALUES (@RealmId, @Employee, @Customer, @Item, @Date, @Hours, @QboId)";
            using (SqlCommand myCommand = new SqlCommand(query, new SqlConnection(conString)))
            {
                myCommand.Connection.Open();
                myCommand.Parameters.AddWithValue("@RealmId", timeActivity.CompanyId);
                myCommand.Parameters.AddWithValue("@Employee", string.Format("{0} {1}", employeeObj.GivenName, employeeObj.FamilyName));
                myCommand.Parameters.AddWithValue("@Customer", string.Format("{0} {1}", CustomerObj.GivenName, CustomerObj.FamilyName));
                myCommand.Parameters.AddWithValue("@Item", ItemObj.Name);
                myCommand.Parameters.AddWithValue("@Date", dateObj);
                myCommand.Parameters.AddWithValue("@Hours", hoursObj);
                myCommand.Parameters.AddWithValue("@QboId", qboId);
                myCommand.ExecuteNonQuery();
                myCommand.Connection.Close();
            }
           
        }
    }
}