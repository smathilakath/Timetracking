﻿using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TimeTracking.Models.Repository;

namespace TimeTracking.Models
{
    public class SyncService
    {
        DataserviceFactory dataserviceFactory = null;
        DataService dataService = null;
        Syncdto syncObjects = null;
        private SyncRepository syncRepository = null;

        public SyncService(OAuthorizationdto oAuthorization)
        {
            dataserviceFactory = new DataserviceFactory(oAuthorization);
            dataService = dataserviceFactory.getDataService();
            syncObjects = new Syncdto();
            syncRepository = new SyncRepository();
        }
        #region <<Prop>>
        public ServiceContext ServiceContext { get { return dataserviceFactory.getServiceContext; } }
        #endregion
        //
        #region <<DB>>

        internal Syncdto GetSyncObjects(object controller, Int64 id)
        {
            return syncRepository.Get(controller, id);
        }
        internal Syncdto GetDatafromDBEmployee(Syncdto syncObjects)
        {

            List<Employee> empList = new List<Employee>();

            using (SqlConnection sqlConnection = new SqlConnection(syncObjects.ConnectionString))
            {
                string oString = "Select * from Employee";
                SqlCommand oCmd = new SqlCommand(oString, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        Employee qboEmp = new Employee();
                        qboEmp.GivenName = oReader["GivenName"].ToString();
                        qboEmp.FamilyName = oReader["FamilyName"].ToString();
                        qboEmp.PrimaryPhone = new TelephoneNumber { FreeFormNumber = oReader["PrimaryPhone"].ToString() };
                        qboEmp.PrimaryEmailAddr = new EmailAddress { Address = oReader["PrimaryEmailAddr"].ToString() };
                        empList.Add(qboEmp);

                    }
                    sqlConnection.Close();
                }
            }
            syncObjects.EmployeeList = empList;
            return syncObjects;
        }


        internal Syncdto GetDatafromDBCustomer(Syncdto syncObjects)
        {

            List<Customer> custList = new List<Customer>();
            using (SqlConnection sqlConnection = new SqlConnection(syncObjects.ConnectionString))
            {
                string oString = "Select * from Customer";
                SqlCommand oCmd = new SqlCommand(oString, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        Customer qboEmp = new Customer();
                        qboEmp.GivenName = oReader["GivenName"].ToString();
                        qboEmp.FamilyName = oReader["FamilyName"].ToString();
                        qboEmp.PrimaryPhone = new TelephoneNumber { FreeFormNumber = oReader["PrimaryPhone"].ToString() };
                        qboEmp.PrimaryEmailAddr = new EmailAddress { Address = oReader["PrimaryEmailAddr"].ToString() };
                        custList.Add(qboEmp);
                    }
                    sqlConnection.Close();
                }
            }
            syncObjects.CustomerList = custList;
            return syncObjects;
        }
        internal Syncdto GetDatafromDBItem(Syncdto syncObjects)
        {

            List<Item> itemList = new List<Item>();
            using (SqlConnection sqlConnection = new SqlConnection(syncObjects.ConnectionString))
            {
                string oString = "Select * from Item";
                SqlCommand oCmd = new SqlCommand(oString, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        Item qboItem = new Item();
                        qboItem.Name = oReader["Name"].ToString();
                        qboItem.UnitPrice = Convert.ToDecimal(oReader["UnitPrice"]);
                        itemList.Add(qboItem);
                    }
                    sqlConnection.Close();
                }
            }
            syncObjects.ItemList = itemList;
            return syncObjects;
        }
        #endregion
        //
        #region <<Sync>>
        public Syncdto SyncEmployees(object controller, Syncdto syncObjects)
        {

            foreach (Employee emp in syncObjects.EmployeeList)
            {
                string EXISTING_EMPLOYEE_QUERY = string.Format("select * from employee where active = true and givenName = '{0}' and familyName = '{1}'", emp.GivenName.Trim(), emp.FamilyName.Trim());
                QueryService<Employee> queryService = new QueryService<Employee>(dataserviceFactory.getServiceContext);
                Employee resultFound = queryService.ExecuteIdsQuery(EXISTING_EMPLOYEE_QUERY).FirstOrDefault<Employee>();
                if (resultFound == null)
                {
                    Employee entity = dataService.Add<Employee>(emp);
                    syncObjects.QboId = entity.Id;
                    syncObjects.IsEmployeeSync = true;
                }
            }
            syncObjects = syncRepository.Save(controller, syncObjects);
            return syncObjects;
        }
        internal Syncdto SyncCustomer(object controller, Syncdto syncObjects)
        {
            foreach (Customer customerItem in syncObjects.CustomerList)
            {
                string EXISTING_CUSTOMER_QUERY = string.Format("select * from customer where active = true and givenName = '{0}' and familyName = '{1}'", customerItem.GivenName.Trim(), customerItem.FamilyName.Trim());
                QueryService<Customer> queryService = new QueryService<Customer>(dataserviceFactory.getServiceContext);
                Customer resultFound = queryService.ExecuteIdsQuery(EXISTING_CUSTOMER_QUERY).FirstOrDefault<Customer>();
                if (resultFound == null)
                {
                    Customer entity = dataService.Add<Customer>(customerItem);
                    syncObjects.QboId = entity.Id;
                    syncObjects.IsCustomerSync = true;
                }
            }
            syncObjects = syncRepository.Save(controller, syncObjects);
            return syncObjects;
        }

        internal Syncdto SyncServiceItems(object controller, Syncdto syncObjects)
        {
            foreach (Item ItemItem in syncObjects.ItemList)
            {
                string EXISTING_ITEM_QUERY = string.Format("select * from Item where active = true and name = '{0}'", ItemItem.Name.Trim());
                QueryService<Item> queryService = new QueryService<Item>(dataserviceFactory.getServiceContext);
                Item resultFound = queryService.ExecuteIdsQuery(EXISTING_ITEM_QUERY).FirstOrDefault<Item>();
                if (resultFound == null)
                {
                    Item entity = dataService.Add<Item>(ItemItem);
                    syncObjects.QboId = entity.Id;
                    syncObjects.IsServiceItemSync = true;
                }
            }
            syncObjects = syncRepository.Save(controller, syncObjects);
            return syncObjects;
        }

        public Syncdto IsServiceItemSync(Syncdto syncObjects, SyncService service)
        {
            Dictionary<string, bool> isSync = new Dictionary<string, bool>();
            var itemDataInDb = service.GetDatafromDBItem(syncObjects);
            
            for (int i = 0; i < itemDataInDb.ItemList.Count; i++)
            {
                string EXISTING_ITEM_QUERY = string.Format("select * from Item where active = true and name = '{0}'", itemDataInDb.ItemList[i].Name.Trim());
                QueryService<Item> queryService = new QueryService<Item>(service.ServiceContext);
                Item resultFound = queryService.ExecuteIdsQuery(EXISTING_ITEM_QUERY).FirstOrDefault<Item>();
                if (resultFound != null)
                {
                    itemDataInDb.ItemList[i].Id = resultFound.Id;
                    isSync.Add(itemDataInDb.ItemList[i].Name, true);
                }
                else
                {
                    isSync.Add(itemDataInDb.ItemList[i].Name, false);
                }
            }
            if (isSync.Where(x => x.Value == false).Any())
            {
                itemDataInDb.IsServiceItemSync = false;

            }
            else
            {
                itemDataInDb.IsServiceItemSync = true;
            }
            return itemDataInDb;
        }
        public Syncdto IsCustSync(Syncdto syncObjects, SyncService service)
        {
            Dictionary<string, bool> isSync = new Dictionary<string, bool>();
            var custDataInDb = service.GetDatafromDBCustomer(syncObjects);
           
            for (int i = 0; i < custDataInDb.CustomerList.Count; i++)
            {
                string EXISTING_CUSTOMER_QUERY = string.Format("select * from customer where active = true and givenName = '{0}' and familyName = '{1}'", custDataInDb.CustomerList[i].GivenName.Trim(), custDataInDb.CustomerList[i].FamilyName.Trim());
                QueryService<Customer> queryService = new QueryService<Customer>(service.ServiceContext);
                Customer resultFound = queryService.ExecuteIdsQuery(EXISTING_CUSTOMER_QUERY).FirstOrDefault<Customer>();
                if (resultFound != null)
                {
                    custDataInDb.CustomerList[i].Id = resultFound.Id;
                    isSync.Add(custDataInDb.CustomerList[i].GivenName, true);
                }
                else
                {
                    isSync.Add(custDataInDb.CustomerList[i].GivenName, false);
                }
            }
            if (isSync.Where(x => x.Value == false).Any())
            {
                custDataInDb.IsCustomerSync = false;
            }
            else
            {
                custDataInDb.IsCustomerSync = true;
            }
            return custDataInDb;
        }

        public Syncdto IsEmpSync(Syncdto syncObjects, SyncService service)
        {
            Dictionary<string, bool> isSync = new Dictionary<string, bool>();
            var empDataInDb = service.GetDatafromDBEmployee(syncObjects);
            for (int i = 0; i < empDataInDb.EmployeeList.Count; i++)
            {
                string EXISTING_EMPLOYEE_QUERY = string.Format("select * from employee where active = true and givenName='{0}' and familyName= '{1}'", empDataInDb.EmployeeList[i].GivenName.Trim(), empDataInDb.EmployeeList[i].FamilyName.Trim());

                QueryService<Employee> queryService = new QueryService<Employee>(service.ServiceContext);
                Employee resultFound = queryService.ExecuteIdsQuery(EXISTING_EMPLOYEE_QUERY).FirstOrDefault<Employee>();
                if (resultFound != null)
                {
                    empDataInDb.EmployeeList[i].Id = resultFound.Id;
                    //indexIterator = i+1;
                    isSync.Add(empDataInDb.EmployeeList[i].GivenName, true);
                }
                else
                {
                    isSync.Add(empDataInDb.EmployeeList[i].GivenName, false);
                }
            }
            if (isSync.Where(x => x.Value == false).Any())
            {
                empDataInDb.IsEmployeeSync = false;
            }
            else
            {
                empDataInDb.IsEmployeeSync = true;
            }

            return empDataInDb;
        }
        #endregion
    }
}