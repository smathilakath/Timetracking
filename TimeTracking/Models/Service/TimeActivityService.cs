/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracking.Models.DTO;
using TimeTracking.Models.Repository;

namespace TimeTracking.Models.Service
{
    /// <summary>
    /// This class is responsible for creating time activity
    /// </summary>
    public class TimeActivityService
    {
        
        DataserviceFactory dataserviceFactory = null;
        DataService dataService = null;
        private TimeActivityRepository timeActivityRepository = null;
        /// <summary>
        /// Fire up service context and repository
        /// </summary>
        /// <param name="timeActivity"></param>
        public TimeActivityService(TimeActivitydto timeActivity)
        {
            dataserviceFactory = new DataserviceFactory(timeActivity.oAuthTokens);
            dataService = dataserviceFactory.getDataService();
            timeActivityRepository = new TimeActivityRepository();
        }
        /// <summary>
        /// Load the drop down list
        /// </summary>
        /// <param name="timeActivitydto"></param>
        /// <returns></returns>
        internal TimeActivitydto LoaddropdownList(TimeActivitydto timeActivitydto)
        {
            timeActivitydto.Employee = timeActivitydto.EmployeeList
               .Select(x =>
                       new SelectListItem
                       {
                           Value = x.Id,
                           Text = x.GivenName
                       });

            timeActivitydto.Employee =  new SelectList(timeActivitydto.Employee, "Value", "Text");
            timeActivitydto.Customer = timeActivitydto.CustomerList
              .Select(x =>
                      new SelectListItem
                      {
                          Value = x.Id,
                          Text = x.GivenName
                      });

            timeActivitydto.Customer = new SelectList(timeActivitydto.Customer, "Value", "Text");
            timeActivitydto.Item = timeActivitydto.ItemList
              .Select(x =>
                      new SelectListItem
                      {
                          Value = x.Id,
                          Text = x.Name
                      });

            timeActivitydto.Item = new SelectList(timeActivitydto.Item, "Value", "Text");
            timeActivitydto.FillTime = GetTimeActivityFromDb(timeActivitydto);
            return timeActivitydto;
        }
        /// <summary>
        /// return the time activity from sql
        /// </summary>
        /// <param name="timeActivitydto"></param>
        /// <returns></returns>
        private List<TimeActivityFill> GetTimeActivityFromDb(TimeActivitydto timeActivitydto)
        {
            List<TimeActivityFill> fillList = new List<TimeActivityFill>();
            using (SqlConnection sqlConnection = new SqlConnection(timeActivitydto.Syncdto.ConnectionString))
            {
                string oString = "Select * from TimeActivity";
                SqlCommand oCmd = new SqlCommand(oString, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        TimeActivityFill timeActivityFill = new TimeActivityFill();
                        timeActivityFill.Employee = oReader["Employee"].ToString();
                        timeActivityFill.Customer = oReader["Customer"].ToString();
                        timeActivityFill.Item = oReader["Item"].ToString();
                        timeActivityFill.Date = oReader["Date"].ToString();
                        timeActivityFill.Hours = oReader["Hours"].ToString();
                        timeActivityFill.QboId = oReader["QboId"].ToString();
                        fillList.Add(timeActivityFill);
                    }
                    sqlConnection.Close();
                }
            }
            return fillList;
        }
        /// <summary>
        /// Push the time activity to QBO
        /// </summary>
        /// <param name="timeActivitydto"></param>
        /// <returns></returns>
        internal TimeActivitydto Save(TimeActivitydto timeActivitydto)
        {
            try
            {
                TimeActivity timeActivity = new TimeActivity();
            timeActivity.TxnDate = timeActivitydto.Date;
            SelectListItem selectedCustom = GetSelectedItem(timeActivitydto.Customer,timeActivitydto.CustomerSelected);
            timeActivity.CustomerRef = new ReferenceType
            {
               
                Value = selectedCustom.Value,
            };
            SelectListItem selectedItem = GetSelectedItem(timeActivitydto.Item, timeActivitydto.ItemSelected);
            timeActivity.ItemRef = new ReferenceType()
            {
               
                Value = selectedItem.Value
            };
            SelectListItem selectedEmp = GetSelectedItem(timeActivitydto.Employee, timeActivitydto.EmployeeSelected);
            timeActivity.NameOf = TimeActivityTypeEnum.Employee;
            timeActivity.NameOfSpecified = true;
            timeActivity.AnyIntuitObject = new ReferenceType()
            {
              
                Value = selectedEmp.Value
            };
            timeActivity.ItemElementName = ItemChoiceType5.EmployeeRef;
           
            timeActivity.BillableStatus = BillableStatusEnum.NotBillable;
            timeActivity.BillableStatusSpecified = true;
            timeActivity.Taxable = false;
            timeActivity.TaxableSpecified = true;
            //Time
            timeActivity.HourlyRate = new Decimal(0.00);
            timeActivity.HourlyRateSpecified = true;
            timeActivity.Hours = 10;

            timeActivity.HoursSpecified = true;
            timeActivity.Minutes = 0;
            timeActivity.Description = timeActivitydto.Description;
            timeActivity.MinutesSpecified = true;
            timeActivity = dataService.Add<TimeActivity>(timeActivity);
            timeActivitydto.QboId = timeActivity.Id;
            timeActivitydto.Hours = timeActivity.Hours;
            timeActivitydto.TxnDate = timeActivity.TxnDate;
            timeActivitydto.AlertMessage = string.Format("Time Activity successfully created and pushed to QBO (QBO ID = {0})", timeActivity.Id);
            return timeActivitydto;
            }
            catch (Intuit.Ipp.Exception.FaultException ex)
            {
                throw ex;
            }
            catch (Intuit.Ipp.Exception.InvalidTokenException ex)
            {
                throw ex;
            }
            catch (Intuit.Ipp.Exception.SdkException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// return the selected value
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private SelectListItem GetSelectedItem(IEnumerable<SelectListItem> enumerable, string p)
        {
            var selected = enumerable.Where(x => x.Value == p).First();
            selected.Selected = true;
            return selected;
        }
        /// <summary>
        /// this function is used to get rate.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private decimal getRate(List<Item> list,string id)
        {
            var result = from item in list
                         where item.Id == id
                         select item;
            return 0;
           
        }
    }
}