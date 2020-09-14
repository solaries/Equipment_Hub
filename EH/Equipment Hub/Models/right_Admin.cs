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
    public class right_Admin 
    { 
        public string add_right_Admin(Equipment_hub_right_Admin new_right_Admin, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_right_Admin>(new_right_Admin);
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
         public string update_right_Admin(Equipment_hub_right_Admin new_right_Admin)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_right_Admin);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_right_Admin> get_right_Admin(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_right_Admin>( sql);
             return actual;
         }  
     }
 
 }
