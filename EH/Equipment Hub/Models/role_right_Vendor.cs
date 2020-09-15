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
    public class role_right_Vendor 
    { 
        public string add_role_right_Vendor(Equipment_hub_role_right_Vendor new_role_right_Vendor, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_role_right_Vendor>(new_role_right_Vendor);
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
         public string update_role_right_Vendor(Equipment_hub_role_right_Vendor new_role_right_Vendor)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_role_right_Vendor);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_role_right_Vendor_data> get_role_right_Vendor_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_role_right_Vendor_data>( "select a.id , a.role , a1.roleName  role_data  , a.right , a2.rightName  right_data    from equipment_hub_role_right_vendor a  inner join  equipment_hub_role_vendor a1 on a.role = a1.id  inner join  equipment_hub_right_vendor a2 on a.right = a2.id "  + sql);
             return actual;
         }  
         public List<Equipment_hub_role_right_Vendor> get_role_right_Vendor(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_role_right_Vendor>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_role_right_Vendor_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("role")]  
    public string Role  
    {  
        get { return _Role; }  
        set { _Role = value;  }  
    }  
    string _Role;
    [Column("role_data")]  
    public string Role_data  
    {  
        get { return _Role_data; }  
        set { _Role_data = value;  }  
    }  
    string _Role_data;
    [Column("right")]  
    public string Right  
    {  
        get { return _Right; }  
        set { _Right = value;  }  
    }  
    string _Right;
    [Column("right_data")]  
    public string Right_data  
    {  
        get { return _Right_data; }  
        set { _Right_data = value;  }  
    }  
    string _Right_data;
  }  
 
 }
