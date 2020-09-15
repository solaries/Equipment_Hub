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
    public class Vendor_Payments 
    { 
        public string add_Vendor_Payments(Equipment_hub_Vendor_Payments new_Vendor_Payments, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_Vendor_Payments>(new_Vendor_Payments);
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
         public string update_Vendor_Payments(Equipment_hub_Vendor_Payments new_Vendor_Payments)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_Vendor_Payments);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_Vendor_Payments_data> get_Vendor_Payments_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Vendor_Payments_data>( "select a.id , a.Equipement_Activity , a1.Number_Of_Units  Equipement_Activity_data  , a.amount , a.Date_Payment_Was_Due , a.Date_Payment_Was_Made   from equipment_hub_vendor_payments a  inner join  equipment_hub_equipment_activity a1 on a.Equipement_Activity = a1.id "  + sql);
             return actual;
         }  
         public List<Equipment_hub_Vendor_Payments> get_Vendor_Payments(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Vendor_Payments>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_Vendor_Payments_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("Equipement_Activity")]  
    public string Equipement_activity  
    {  
        get { return _Equipement_activity; }  
        set { _Equipement_activity = value;  }  
    }  
    string _Equipement_activity;
    [Column("Equipement_Activity_data")]  
    public string Equipement_activity_data  
    {  
        get { return _Equipement_activity_data; }  
        set { _Equipement_activity_data = value;  }  
    }  
    string _Equipement_activity_data;
    [Column("amount")]  
    public string Amount  
    {  
        get { return _Amount; }  
        set { _Amount = value;  }  
    }  
    string _Amount;
    [Column("Date_Payment_Was_Due")]  
    public string Date_payment_was_due  
    {  
        get { return _Date_payment_was_due; }  
        set { _Date_payment_was_due = value;  }  
    }  
    string _Date_payment_was_due;
    [Column("Date_Payment_Was_Made")]  
    public string Date_payment_was_made  
    {  
        get { return _Date_payment_was_made; }  
        set { _Date_payment_was_made = value;  }  
    }  
    string _Date_payment_was_made;
  }  
 
 }
