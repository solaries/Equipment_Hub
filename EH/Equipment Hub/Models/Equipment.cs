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
    public class Equipment 
    { 
        public string add_Equipment(Equipment_hub_Equipment new_Equipment, bool returnID = false ) 
         {
             string result = "";
             if(returnID){
                result = "0";
             }
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Insert<Equipment_hub_Equipment>(new_Equipment);
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
         public string update_Equipment(Equipment_hub_Equipment new_Equipment)
         {
             string result = "";
             try
             {
                 var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
                 var x = context.Update(new_Equipment);
             }
             catch (Exception dd)
             {
                 result = dd.Message;
             }
             return result;
         }
         public List<Equipment_hub_Equipment_data> get_Equipment_linked(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Equipment_data>( "select a.id , a.Equipment_Type , a1.Name_Of_Equipment  Equipment_Type_data  , a.Equipment_Code , a.Vendor , a2.first_name  Vendor_data    from Equipment_hub_Equipment a  inner join  Equipment_hub_Equipment_Type a1 on a.Equipment_Type = a1.id  inner join  Equipment_hub_authenticate_Vendor a2 on a.Vendor = a2.id "  + sql);
             return actual;
         }  
         public List<Equipment_hub_Equipment> get_Equipment(string sql)
         {
             var context = Equipment_hub.Data.Models.Equipment_hub.GetInstance();
             var actual = context.Fetch<Equipment_hub_Equipment>( sql);
             return actual;
         }  
     }
 public partial class Equipment_hub_Equipment_data  
  {  
    [Column("id")]  
    public long Id  
    {  
        get { return _Id; }  
        set { _Id = value;  }  
    }  
    long _Id;
    [Column("Equipment_Type")]  
    public string Equipment_type  
    {  
        get { return _Equipment_type; }  
        set { _Equipment_type = value;  }  
    }  
    string _Equipment_type;
    [Column("Equipment_Type_data")]  
    public string Equipment_type_data  
    {  
        get { return _Equipment_type_data; }  
        set { _Equipment_type_data = value;  }  
    }  
    string _Equipment_type_data;
    [Column("Equipment_Code")]  
    public string Equipment_code  
    {  
        get { return _Equipment_code; }  
        set { _Equipment_code = value;  }  
    }  
    string _Equipment_code;
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
