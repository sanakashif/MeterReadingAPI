using MeterReadingAPI.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MeterReadingAPI.Controllers
{
    public class AccountsController : ApiController
    {
        EnergyCompanyEntities db = new EnergyCompanyEntities();

        // POST: Accounts
        [System.Web.Http.Route("api/accounts/insert-accounts")]
        public string Post([FromBody] JObject value)
        {
            if (ModelState.IsValid)
            {
                var successfulAccounts = 0;
                var failedAccounts = 0;
                try
                {
                    var records = value["records"];

                    for (int i = 0; i < records.Count(); i++)
                    {
                        var currentRecord = records[i];

                        var id = Convert.ToInt32(currentRecord["AccountId"]);

                        var existringAccount = db.Accounts.Where(a => a.ID == id).FirstOrDefault();

                        if (existringAccount == null)
                        {
                            Account account = new Account();
                            account.ID = Convert.ToInt32(currentRecord["AccountId"]);
                            account.FirstName = currentRecord["FirstName"].ToString();
                            account.LastName = currentRecord["LastName"].ToString();

                            db.Accounts.Add(account);
                            db.SaveChanges();
                            successfulAccounts += 1;
                        }
                        else
                        {
                            failedAccounts += 1;
                        }


                    }
                    return $"Successful account enteries: {successfulAccounts}. Failed account enteries: {failedAccounts}";
                }
                catch (Exception )
                {
                    return $"Successful account enteries: {successfulAccounts}. Failed account enteries: {failedAccounts}";
                    //return "Fail";
                }
              
            }
            else
            {
                return "Fail";
            }

        }
    }
}