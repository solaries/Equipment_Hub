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
    public class Rent_Rate 
    { 
        public string add_Rent_Rate(Equipment_hub_Rent_Rate new_Rent_Rate, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_Rent_Rate>(new_Rent_Rate);
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
         public string update_Rent_Rate(Equipment_hub_Rent_Rate new_Rent_Rate)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_Rent_Rate);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_Rent_Rate_data> get_Rent_Rate_linked(string sql, bool addExtra = false)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Rent_Rate_data>(

                 " SELECT * FROM (select a.id , a.Equipement_Type , a1.Name_Of_Equipment  Equipement_Type_data  , a.Duration_Type , a2.Duration_Name  Duration_Type_data  , a.Qunatity , a.Price , a.Entry_Date , a.Vendor , a3.Company_Name  Vendor_data    from Equipment_hub_Rent_Rate a  inner join  Equipment_hub_Equipment_Type a1 on a.Equipement_Type = a1.id  inner join  Equipment_hub_Duration_Type a2 on a.Duration_Type = a2.id  inner join  equipment_hub_vendor_company a3 on a.Vendor = a3.id    WHERE a.id IN ( SELECT id FROM ( select max(id) id, Equipement_Type, vendor from equipment_hub_rent_rate GROUP BY Equipement_Type, vendor ) a)  " +
                 (addExtra ? "UNION ALL  SELECT  0 id , a1.id  Equipement_Type , a1.Name_Of_Equipment  Equipement_Type_data  ,0  Duration_Type ,  ''  Duration_Type_data  , 0 Qunatity , 0 Price , '' Entry_Date ,a3.id   Vendor , a3.Company_Name  Vendor_data     FROM (select distinct Equipment_Type, vendor from equipment_hub_equipment) x INNER JOIN    Equipment_hub_Equipment_Type a1 ON x.Equipment_Type = a1.id   INNER JOIN    equipment_hub_vendor_company a3 ON x.Vendor  = a3.id   WHERE CONCAT(a1.id , '-', a3.id) NOT IN    ( SELECT DISTINCT CONCAT(  Equipement_Type , '-',  vendor) FROM  equipment_hub_rent_rate )" : "") + 
                 "    ) a   " + sql);
             return actual;
         }  
         public List<Equipment_hub_Rent_Rate> get_Rent_Rate(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Rent_Rate>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_Rent_Rate_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("Equipement_Type")]  
    public string Equipement_type  
    {  
        get { return _Equipement_type; }  
        set { _Equipement_type = value;  }  
    }  
    string _Equipement_type;
    [Column("Equipement_Type_data")]  
    public string Equipement_type_data  
    {  
        get { return _Equipement_type_data; }  
        set { _Equipement_type_data = value;  }  
    }  
    string _Equipement_type_data;
    [Column("Duration_Type")]  
    public string Duration_type  
    {  
        get { return _Duration_type; }  
        set { _Duration_type = value;  }  
    }  
    string _Duration_type;
    [Column("Duration_Type_data")]  
    public string Duration_type_data  
    {  
        get { return _Duration_type_data; }  
        set { _Duration_type_data = value;  }  
    }  
    string _Duration_type_data;
    [Column("Qunatity")]  
    public string Qunatity  
    {  
        get { return _Qunatity; }  
        set { _Qunatity = value;  }  
    }  
    string _Qunatity;
    [Column("Price")]  
    public string Price  
    {  
        get { return _Price; }  
        set { _Price = value;  }  
    }  
    string _Price;
    [Column("Entry_Date")]  
    public string Entry_date  
    {  
        get { return _Entry_date; }  
        set { _Entry_date = value;  }  
    }  
    string _Entry_date;
    [Column("Vendor")]  
    public string Vendor  
    {  
        get { return _Vendor; }  
        set { _Vendor = value;  }  
    }  
    string _Vendor;
    [Column("Vendor_data")]  
    public string Vendor_data  
    {  
        get { return _Vendor_data; }  
        set { _Vendor_data = value;  }  
    }  
    string _Vendor_data;
  }  
 
 }
