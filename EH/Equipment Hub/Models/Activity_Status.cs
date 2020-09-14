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
    public class Activity_Status 
    { 
        public string add_Activity_Status(Equipment_hub_Activity_Status new_Activity_Status, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_Activity_Status>(new_Activity_Status);
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
         public string update_Activity_Status(Equipment_hub_Activity_Status new_Activity_Status)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_Activity_Status);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_Activity_Status> get_Activity_Status(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Activity_Status>( sql);
             return actual;
         }  
     }
 
 }
