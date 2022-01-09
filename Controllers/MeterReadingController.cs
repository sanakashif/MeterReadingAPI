using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MeterReadingAPI.Models;

namespace MeterReadingAPI.Controllers
{
    public class MeterReadingController : ApiController
    {
        EnergyCompanyEntities db = new EnergyCompanyEntities();
        // POST: Meter Readings
        [System.Web.Http.Route("api/meter-reading/meter-reading-uploads")]
        [System.Web.Http.HttpPost]        
        public string Post([FromBody] JObject value)
        {
            if (ModelState.IsValid)
            {
                var successfulReadings = 0;
                var failedReadings = value["records"].Count();

                try
                {
                    var records = value["records"];

                    // Iterating through all meter reading enteries
                    for (int i = 0; i < records.Count(); i++)
                    {
                        var currentRecord = records[i];
                        // Getting the account Id for the current record
                        var accountId = Convert.ToInt32(currentRecord["AccountId"]);
                        // Checking if account exists in the accounts table
                        var accountExist = db.Accounts.Where(a => a.ID == accountId).FirstOrDefault();
                        // Checking if reading already exist for this account 
                        var existingReading = db.MeterReadings.Where(m => m.AccountID == accountId).FirstOrDefault();

                        if (accountExist!=null && existingReading==null)
                        {
                            MeterReading meterReading = new MeterReading();
                            meterReading.ID = i + 1;
                            meterReading.AccountID = Convert.ToInt32(currentRecord["AccountId"]);
                            meterReading.MeterReadingDateTime = DateTime.Parse(currentRecord["MeterReadingDateTime"].ToString());
                            
                            // Only inserting a record to db if meter read value is not null
                            if (currentRecord["MeterReadValue"] != null)
                            {
                                int reading;

                                // Check if meter reading is a valid integer
                                if (int.TryParse(currentRecord["MeterReadValue"].ToString(), out reading))
                                {
                                    // Only save to database if reading is <=5 digits else move to next record
                                    if (reading.ToString().Length <= 5)
                                    {
                                        meterReading.MeterReadValue = reading.ToString("00000");
                                                                               
                                        db.MeterReadings.Add(meterReading);
                                        db.SaveChanges();                                        
                                        
                                        successfulReadings += 1;
                                        failedReadings -= 1;
                                    }
                                }

                            }

                        }
                     
                    }
                    return $"Successful reading enteries: {successfulReadings}. Failed reading enteries: {failedReadings}";
                }
                catch(Exception ex)
                {
                    return $"Successful reading enteries: {successfulReadings}. Failed reading enteries: {failedReadings}";
                }
                
            }
            else
            {
                return "Fail";
            }

        }

      

    }
}