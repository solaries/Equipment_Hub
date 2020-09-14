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
    public class Selected_Equipement 
    { 
        public string add_Selected_Equipement(Equipment_hub_Selected_Equipement new_Selected_Equipement, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_Selected_Equipement>(new_Selected_Equipement);
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
         public string update_Selected_Equipement(Equipment_hub_Selected_Equipement new_Selected_Equipement)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_Selected_Equipement);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_Selected_Equipement_data> get_Selected_Equipement_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Selected_Equipement_data>( "select a.id , a.equipment_activity , a1.Number_Of_Units  equipment_activity_data  , a.equipment , a2.Equipment_Code  equipment_data    from Equipment_hub_Selected_Equipement a  inner join  Equipment_hub_Equipment_Activity a1 on a.equipment_activity = a1.id  inner join  Equipment_hub_Equipment a2 on a.equipment = a2.id "  + sql);
             return actual;
         }  
         public List<Equipment_hub_Selected_Equipement> get_Selected_Equipement(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Selected_Equipement>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_Selected_Equipement_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("equipment_activity")]  
    public string Equipment_activity  
    {  
        get { return _Equipment_activity; }  
        set { _Equipment_activity = value;  }  
    }  
    string _Equipment_activity;
    [Column("equipment_activity_data")]  
    public string Equipment_activity_data  
    {  
        get { return _Equipment_activity_data; }  
        set { _Equipment_activity_data = value;  }  
    }  
    string _Equipment_activity_data;
    [Column("equipment")]  
    public string Equipment  
    {  
        get { return _Equipment; }  
        set { _Equipment = value;  }  
    }  
    string _Equipment;
    [Column("equipment_data")]  
    public string Equipment_data  
    {  
        get { return _Equipment_data; }  
        set { _Equipment_data = value;  }  
    }  
    string _Equipment_data;
  }  
 
 }
