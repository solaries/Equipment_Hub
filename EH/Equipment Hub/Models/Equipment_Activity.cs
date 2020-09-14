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
    public class Equipment_Activity 
    {
        public string add_Equipment_Activity(Equipment_hub_Equipment_Activity new_Equipment_Activity, string Activity_status, bool returnID = false) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_Equipment_Activity>(new_Equipment_Activity);

                 Equipment_hub_Equipment_Activity_Movement em1 = new Equipment_hub_Equipment_Activity_Movement();
                 em1.Activity_date = System.DateTime.Now ;
                 em1.Activity_status = long.Parse(Activity_status);
                 em1.Equipment_activity = long.Parse(x.ToString());
                 context.Insert<Equipment_hub_Equipment_Activity_Movement>(em1);
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
         public string update_Equipment_Activity(Equipment_hub_Equipment_Activity new_Equipment_Activity)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();

                 //new_Equipment_Activity = get_Equipment_Activity(" where id = " + new_Equipment_Activity.Id.ToString())[0];
                 //var x = context.Update(new_Equipment_Activity);

                 Equipment_hub_Equipment_Activity_Movement em1  = context.Fetch<Equipment_hub_Equipment_Activity_Movement>( " where Equipment_activity = " +  new_Equipment_Activity.Id.ToString() + " order by id desc " )[0] ;
                 em1.Activity_date = System.DateTime.Now;
                 em1.Activity_status +=1;
                 if (em1.Activity_status == 3)
                 {
                     em1.Activity_status += 1;
                 }

                 context.Insert<Equipment_hub_Equipment_Activity_Movement>(em1);

             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_Equipment_Activity_data> get_Equipment_Activity_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Equipment_Activity_data>("select a.id , a.equipment_type , a1.Name_Of_Equipment  equipment_type_data  ,  a.Number_Of_Units , a.Rent_Rate ,    CONCAT( a2.Qunatity * a.Rent_Rate_Qunatity ,' ', b1.Duration_Name) Rent_Rate_data ,  b1.Duration_Name ,  	  a2.Price * a.Number_Of_Units * a.Rent_Rate_Qunatity Rent_Rate_Qunatity,a2.Price , a.Rent_Rate_Qunatity q,  	    a.Customer_Company ,  a3.Company_Name  Customer_Company_data  , a.Vendor_Company , a4.Company_Name  Vendor_Company_data  ,  		     c1.Activity_Status , a5.Activity_Status_Name  Activity_Status_data  , c1.activity_date    	  from Equipment_hub_Equipment_Activity a   inner JOIN  Equipment_hub_Equipment_Type a1 on a.equipment_type = a1.id  	   inner JOIN  Equipment_hub_Rent_Rate a2 on a.Rent_Rate = a2.id   	   inner JOIN  equipment_hub_duration_type b1 ON  a2.Duration_Type = b1.id  	 INNER JOIN  (SELECT * FROM equipment_hub_equipment_activity_movement  WHERE id IN ( SELECT id FROM ( SELECT  Equipment_Activity ,  max(id)  id FROM equipment_hub_equipment_activity_movement   GROUP BY Equipment_Activity) a )) c1 ON c1.Equipment_Activity = a.id INNER JOIN  Equipment_hub_Customer_Company a3 on a.Customer_Company = a3.id   	   inner JOIN   Equipment_hub_Vendor_Company a4 on a.Vendor_Company = a4.id   	   inner JOIN   Equipment_hub_Activity_Status a5 ON c1.Activity_Status = a5.id " + sql);
             return actual;
         }  
         public List<Equipment_hub_Equipment_Activity> get_Equipment_Activity(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Equipment_Activity>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_Equipment_Activity_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("equipment_type")]  
    public string Equipment_type  
    {  
        get { return _Equipment_type; }  
        set { _Equipment_type = value;  }  
    }  
    string _Equipment_type;
    [Column("equipment_type_data")]  
    public string Equipment_type_data  
    {  
        get { return _Equipment_type_data; }  
        set { _Equipment_type_data = value;  }  
    }  
    string _Equipment_type_data;
    [Column("Number_Of_Units")]  
    public string Number_of_units  
    {  
        get { return _Number_of_units; }  
        set { _Number_of_units = value;  }  
    }  
    string _Number_of_units;
    [Column("Rent_Rate")]  
    public string Rent_rate  
    {  
        get { return _Rent_rate; }  
        set { _Rent_rate = value;  }  
    }  
    string _Rent_rate;
    [Column("Rent_Rate_data")]  
    public string Rent_rate_data  
    {  
        get { return _Rent_rate_data; }  
        set { _Rent_rate_data = value;  }  
    }  
    string _Rent_rate_data;
    [Column("Rent_Rate_Qunatity")]  
    public string Rent_rate_qunatity  
    {  
        get { return _Rent_rate_qunatity; }  
        set { _Rent_rate_qunatity = value;  }  
    }  
    string _Rent_rate_qunatity;
    [Column("Customer_Company")]  
    public string Customer_company  
    {  
        get { return _Customer_company; }  
        set { _Customer_company = value;  }  
    }  
    string _Customer_company;
    [Column("Customer_Company_data")]  
    public string Customer_company_data  
    {  
        get { return _Customer_company_data; }  
        set { _Customer_company_data = value;  }  
    }  
    string _Customer_company_data;
    [Column("Vendor_Company")]  
    public string Vendor_company  
    {  
        get { return _Vendor_company; }  
        set { _Vendor_company = value;  }  
    }  
    string _Vendor_company;
    [Column("Vendor_Company_data")]  
    public string Vendor_company_data  
    {  
        get { return _Vendor_company_data; }  
        set { _Vendor_company_data = value;  }  
    }  
    string _Vendor_company_data;
    [Column("Activity_Status")]  
    public string Activity_status  
    {  
        get { return _Activity_status; }  
        set { _Activity_status = value;  }  
    }  
    string _Activity_status;
    [Column("Activity_Status_data")]  
    public string Activity_status_data  
    {  
        get { return _Activity_status_data; }  
        set { _Activity_status_data = value;  }  
    }  
    string _Activity_status_data;
    [Column("activity_date")]  
    public string Activity_date  
    {  
        get { return _Activity_date; }  
        set { _Activity_date = value;  }  
    }  
    string _Activity_date;
  }  
 
 }
