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
    public class role_Admin 
    { 
        public string add_role_Admin(Equipment_hub_role_Admin new_role_Admin, string selectedRights, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_role_Admin>(new_role_Admin);
                if(returnID){
                    result =x.ToString().Trim();
                }
                 List<Equipment_hub_role_right_Admin> AdminRoleRightList = new List<Equipment_hub_role_right_Admin>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                    Equipment_hub_role_right_Admin AdminRoleRight = new Equipment_hub_role_right_Admin();
                    AdminRoleRight.Role = long.Parse(x.ToString());
                    AdminRoleRight.Right = long.Parse(idList[i]);
                    AdminRoleRightList.Add(AdminRoleRight);
                 }
                 context.InsertBulk<Equipment_hub_role_right_Admin>(AdminRoleRightList);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_role_Admin(Equipment_hub_role_Admin new_role_Admin, string selectedRights)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_role_Admin);
                 context.DeleteWhere<Equipment_hub_role_right_Admin>(" role = " + new_role_Admin.Id.ToString());
                 List<Equipment_hub_role_right_Admin> AdminRoleRightList = new List<Equipment_hub_role_right_Admin>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                    Equipment_hub_role_right_Admin AdminRoleRight = new Equipment_hub_role_right_Admin();
                    AdminRoleRight.Role = long.Parse(  new_role_Admin.Id.ToString());
                    AdminRoleRight.Right = long.Parse(idList[i]);
                    AdminRoleRightList.Add(AdminRoleRight);
                 }
                 context.InsertBulk<Equipment_hub_role_right_Admin>(AdminRoleRightList);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_role_Admin> get_role_Admin(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_role_Admin>( sql);
             return actual;
         }  
     }
 
 }
