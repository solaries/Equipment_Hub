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
    public class role_Vendor 
    { 
        public string add_role_Vendor(Equipment_hub_role_Vendor new_role_Vendor, string selectedRights, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_role_Vendor>(new_role_Vendor);
                if(returnID){
                    result =x.ToString().Trim();
                }
                 List<Equipment_hub_role_right_Vendor> VendorRoleRightList = new List<Equipment_hub_role_right_Vendor>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                    Equipment_hub_role_right_Vendor VendorRoleRight = new Equipment_hub_role_right_Vendor();
                    VendorRoleRight.Role = long.Parse(x.ToString());
                    VendorRoleRight.Right = long.Parse(idList[i]);
                    VendorRoleRightList.Add(VendorRoleRight);
                 }
                 context.InsertBulk<Equipment_hub_role_right_Vendor>(VendorRoleRightList);
            } 
            catch (Exception dd) 
            { 
                 result = dd.Message;
             }
             return result;
         }
         public string update_role_Vendor(Equipment_hub_role_Vendor new_role_Vendor, string selectedRights)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_role_Vendor);
                 context.DeleteWhere<Equipment_hub_role_right_Vendor>(" role = " + new_role_Vendor.Id.ToString());
                 List<Equipment_hub_role_right_Vendor> VendorRoleRightList = new List<Equipment_hub_role_right_Vendor>();
                 string[] idList = selectedRights.Split(new string[] { "sphinxcol" }, StringSplitOptions.RemoveEmptyEntries);
                 for (int i = 0; i < idList.Length; i++)
                 {
                    Equipment_hub_role_right_Vendor VendorRoleRight = new Equipment_hub_role_right_Vendor();
                    VendorRoleRight.Role = long.Parse(  new_role_Vendor.Id.ToString());
                    VendorRoleRight.Right = long.Parse(idList[i]);
                    VendorRoleRightList.Add(VendorRoleRight);
                 }
                 context.InsertBulk<Equipment_hub_role_right_Vendor>(VendorRoleRightList);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_role_Vendor_data> get_role_Vendor_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_role_Vendor_data>( "select a.id , a.roleName , a.company , a1.Company_Name  company_data    from equipment_hub_role_vendor a  inner join  equipment_hub_vendor_company a1 on a.company = a1.id "  + sql);
             return actual;
         }  
         public List<Equipment_hub_role_Vendor> get_role_Vendor(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_role_Vendor>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_role_Vendor_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("roleName")]  
    public string Rolename  
    {  
        get { return _Rolename; }  
        set { _Rolename = value;  }  
    }  
    string _Rolename;
    [Column("company")]  
    public string Company  
    {  
        get { return _Company; }  
        set { _Company = value;  }  
    }  
    string _Company;
    [Column("company_data")]  
    public string Company_data  
    {  
        get { return _Company_data; }  
        set { _Company_data = value;  }  
    }  
    string _Company_data;
  }  
 
 }
