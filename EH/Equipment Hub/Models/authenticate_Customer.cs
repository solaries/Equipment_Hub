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
    public class authenticate_Customer 
    { 
        public string add_authenticate_Customer(Equipment_hub_authenticate_Customer new_authenticate_Customer, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_authenticate_Customer>(new_authenticate_Customer);
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
         public string update_authenticate_Customer(Equipment_hub_authenticate_Customer new_authenticate_Customer)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_authenticate_Customer);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_authenticate_Customer_data> get_authenticate_Customer_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_authenticate_Customer_data>( "select a.id , a.first_name , a.last_name , a.email , a.password , a.password2 , a.activated , a.Company_Name , a1.Company_Name  Company_Name_data    from Equipment_hub_authenticate_Customer a  inner join  Equipment_hub_Customer_Company a1 on a.Company_Name = a1.id "  + sql);
             return actual;
         }  
         public List<Equipment_hub_authenticate_Customer> get_authenticate_Customer(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_authenticate_Customer>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_authenticate_Customer_data  
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
    [Column("activated")]  
    public string Activated  
    {  
        get { return _Activated; }  
        set { _Activated = value;  }  
    }  
    string _Activated;
    [Column("Company_Name")]  
    public string Company_name  
    {  
        get { return _Company_name; }  
        set { _Company_name = value;  }  
    }  
    string _Company_name;
    [Column("Company_Name_data")]  
    public string Company_name_data  
    {  
        get { return _Company_name_data; }  
        set { _Company_name_data = value;  }  
    }  
    string _Company_name_data;
  }  
 
 }
