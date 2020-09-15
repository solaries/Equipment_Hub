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
    public class Equipment_Activity_Movement 
    { 
        public string add_Equipment_Activity_Movement(Equipment_hub_Equipment_Activity_Movement new_Equipment_Activity_Movement, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_Equipment_Activity_Movement>(new_Equipment_Activity_Movement);
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
         public string update_Equipment_Activity_Movement(Equipment_hub_Equipment_Activity_Movement new_Equipment_Activity_Movement)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_Equipment_Activity_Movement);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_Equipment_Activity_Movement_data> get_Equipment_Activity_Movement_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Equipment_Activity_Movement_data>( "select a.id , a.Activity_Status , a1.Activity_Status_Name  Activity_Status_data  , a.activity_date , a.Equipment_Activity , a2.Number_Of_Units  Equipment_Activity_data    from equipment_hub_equipment_activity_movement a  inner join  equipment_hub_activity_status a1 on a.Activity_Status = a1.id  inner join  equipment_hub_equipment_activity a2 on a.Equipment_Activity = a2.id "  + sql);
             return actual;
         }  
         public List<Equipment_hub_Equipment_Activity_Movement> get_Equipment_Activity_Movement(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Equipment_Activity_Movement>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_Equipment_Activity_Movement_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
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
    [Column("Equipment_Activity")]  
    public string Equipment_activity  
    {  
        get { return _Equipment_activity; }  
        set { _Equipment_activity = value;  }  
    }  
    string _Equipment_activity;
    [Column("Equipment_Activity_data")]  
    public string Equipment_activity_data  
    {  
        get { return _Equipment_activity_data; }  
        set { _Equipment_activity_data = value;  }  
    }  
    string _Equipment_activity_data;
  }  
 
 }
