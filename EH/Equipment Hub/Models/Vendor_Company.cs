using NPoco;
using Equipment_hub.Data;
using Equipment_hub.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
 
namespace Equipment_hub.Models 
{ 
    public class Vendor_Company 
    { 
        public string add_Vendor_Company(Equipment_hub_Vendor_Company new_Vendor_Company, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_Vendor_Company>(new_Vendor_Company);
                if(returnID){
                    result =x.ToString().Trim();
                }
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_Vendor_Company(Equipment_hub_Vendor_Company new_Vendor_Company)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_Vendor_Company);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_Vendor_Company> get_Vendor_Company(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Vendor_Company>( sql);
             return actual;
         }  
     }
 
 }
