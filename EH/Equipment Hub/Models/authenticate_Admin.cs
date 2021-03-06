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
    public class authenticate_Admin 
    { 
        public string add_authenticate_Admin(Equipment_hub_authenticate_Admin new_authenticate_Admin, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_authenticate_Admin>(new_authenticate_Admin);
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
         public string update_authenticate_Admin(Equipment_hub_authenticate_Admin new_authenticate_Admin)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_authenticate_Admin);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_authenticate_Admin_data> get_authenticate_Admin_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_authenticate_Admin_data>( "select a.id , a.first_name , a.last_name , a.email , a.role , a1.roleName  role_data  , a.password , a.password2   from equipment_hub_authenticate_admin a  inner join  equipment_hub_role_admin a1 on a.role = a1.id "  + sql);
             return actual;
         }  
         public List<Equipment_hub_authenticate_Admin> get_authenticate_Admin(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_authenticate_Admin>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_authenticate_Admin_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("first_name")]  
    public string First_name  
    {  
        get { return _First_name; }  
        set { _First_name = value;  }  
    }  
    string _First_name;
    [Column("last_name")]  
    public string Last_name  
    {  
        get { return _Last_name; }  
        set { _Last_name = value;  }  
    }  
    string _Last_name;
    [Column("email")]  
    public string Email  
    {  
        get { return _Email; }  
        set { _Email = value;  }  
    }  
    string _Email;
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
    [Column("password")]  
    public byte[] Password  
    {  
        get { return _Password; }  
        set { _Password = value;  }  
    }  
    byte[] _Password;
    [Column("password2")]  
    public byte[] Password2  
    {  
        get { return _Password2; }  
        set { _Password2 = value;  }  
    }  
    byte[] _Password2;
  }  
 
 }
