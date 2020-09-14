using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Security.Cryptography; 
using System.Text;
using Equipment_hub.Data;
using Equipment_hub.Data.Models;
using Equipment_hub.Models;  
using System.Net;

namespace Equipment_hub.BusinessLogic
{ 
    public class centralCalls 
    {  
        private static long getVal( ) {
            if (HttpContext.Current.Session["userID"] == null)
            {
                return 0;
            }
            else {
                return long.Parse(HttpContext.Current.Session["userID"].ToString());
            }
        }
        public static string add_new_Activity_Status(string Activity_status_name, bool returnID = false )
        { 
            string response = ""; 
            Activity_Status c = new Activity_Status();  
            string data = "";
            try
            { 

                Equipment_hub_Activity_Status cust = new Equipment_hub_Activity_Status(); 
                cust.Activity_status_name =  Activity_status_name;
                data += ",Activity_status_name : " + Activity_status_name;
                response = c.add_Activity_Status(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ACTIVITY_STATUS_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ACTIVITY_STATUS_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ACTIVITY_STATUS_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Activity_Status";
                Audit.InsertAudit((int)eventzz.ERROR_ACTIVITY_STATUS_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Activity_Status> get_Activity_Status(string sql)
        { 
            List<Equipment_hub_Activity_Status> response = null;
            try
            { 
                Activity_Status c = new Activity_Status(); 
                response = c.get_Activity_Status(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ACTIVITY_STATUS_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Activity_Status( string id, string oActivity_status_name,string Activity_status_name, bool andPassword = true) 
        { 
            string response = ""; 
            Activity_Status c = new Activity_Status();   
            string data = "";
            try
            { 
                Equipment_hub_Activity_Status cust = c.get_Activity_Status(" where id = " + id  )[0]; 
                cust.Activity_status_name =  Activity_status_name;
                data += ",Activity_status_name : " + oActivity_status_name + " -> " + Activity_status_name;
                response = c.update_Activity_Status(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ACTIVITY_STATUS_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ACTIVITY_STATUS_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ACTIVITY_STATUS_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_authenticate_Admin(string First_name,string Last_name,string Email,string Role,string Password,string Password2, bool returnID = false )
        { 
            string response = ""; 
            authenticate_Admin c = new authenticate_Admin();  
            string data = "";
            try
            { 

                Equipment_hub_authenticate_Admin cust = new Equipment_hub_authenticate_Admin(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + Role;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_Admin(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_authenticate_Admin_data> get_authenticate_Admin(string sql)
        { 
            List<Equipment_hub_authenticate_Admin_data> response = null;
            try
            { 
                authenticate_Admin c = new authenticate_Admin(); 
                response = c.get_authenticate_Admin_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_authenticate_Admin( string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string First_name,string Last_name,string Email,string Role,string Password,string Password2, bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_Admin c = new authenticate_Admin();   
            string data = "";
            try
            { 
                Equipment_hub_authenticate_Admin cust = c.get_authenticate_Admin(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + oRole + " -> " + Role;
                 if(andPassword)
                 {
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 }
                response = c.update_authenticate_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_authenticate_Customer(string First_name,string Last_name,string Email,string Password,string Password2,string Activated,string Company_name, bool returnID = false )
        { 
            string response = ""; 
            authenticate_Customer c = new authenticate_Customer();  
            string data = "";
            try
            { 

                Equipment_hub_authenticate_Customer cust = new Equipment_hub_authenticate_Customer(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                cust.Activated =  Int16.Parse( Activated == null ? "1" : Activated)  ;
                 data += ",Activated : " + Activated;
                cust.Company_name =  long.Parse( Company_name == null ? "1" : Company_name)  ;
                 data += ",Company_name : " + Company_name;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_Customer(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_CUSTOMER_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_CUSTOMER_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_CUSTOMER_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_Customer";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_CUSTOMER_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_authenticate_Customer_data> get_authenticate_Customer(string sql)
        { 
            List<Equipment_hub_authenticate_Customer_data> response = null;
            try
            { 
                authenticate_Customer c = new authenticate_Customer(); 
                response = c.get_authenticate_Customer_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_CUSTOMER_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_authenticate_Customer( string id, string oFirst_name,string oLast_name,string oEmail,string oPassword,string oPassword2,string oActivated,string oCompany_name,string First_name,string Last_name,string Email,string Password,string Password2,string Activated,string Company_name, bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_Customer c = new authenticate_Customer();   
            string data = "";
            try
            { 
                Equipment_hub_authenticate_Customer cust = c.get_authenticate_Customer(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 cust.Activated =  Int16.Parse( Activated == null ? "1" : Activated)  ;
                 data += ",Activated : " + oActivated + " -> " + Activated;
                 cust.Company_name =  long.Parse( Company_name == null ? "1" : Company_name)  ;
                 data += ",Company_name : " + oCompany_name + " -> " + Company_name;
                 if(andPassword)
                 {
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 }
                response = c.update_authenticate_Customer(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_CUSTOMER_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_CUSTOMER_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_CUSTOMER_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_authenticate_Vendor(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Activated,string Company_name, bool returnID = false )
        { 
            string response = ""; 
            authenticate_Vendor c = new authenticate_Vendor();  
            string data = "";
            try
            { 

                Equipment_hub_authenticate_Vendor cust = new Equipment_hub_authenticate_Vendor(); 
                cust.First_name =  First_name;
                data += ",First_name : " + First_name;
                cust.Last_name =  Last_name;
                 data += ",Last_name : " + Last_name;
                cust.Email =  Email;
                 data += ",Email : " + Email;
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + Role;
                cust.Activated =  Int16.Parse( Activated == null ? "1" : Activated)  ;
                 data += ",Activated : " + Activated;
                cust.Company_name =  long.Parse( Company_name == null ? "1" : Company_name)  ;
                 data += ",Company_name : " + Company_name;
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                response = c.add_authenticate_Vendor(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_VENDOR_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_VENDOR_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_VENDOR_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding authenticate_Vendor";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_VENDOR_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_authenticate_Vendor_data> get_authenticate_Vendor(string sql)
        { 
            List<Equipment_hub_authenticate_Vendor_data> response = null;
            try
            { 
                authenticate_Vendor c = new authenticate_Vendor(); 
                response = c.get_authenticate_Vendor_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_VENDOR_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_authenticate_Vendor( string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string oActivated,string oCompany_name,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Activated,string Company_name, bool andPassword = true) 
        { 
            string response = ""; 
            authenticate_Vendor c = new authenticate_Vendor();   
            string data = "";
            try
            { 
                Equipment_hub_authenticate_Vendor cust = c.get_authenticate_Vendor(" where id = " + id  )[0]; 
                cust.First_name =  First_name;
                data += ",First_name : " + oFirst_name + " -> " + First_name;
                 cust.Last_name =  Last_name;
                 data += ",Last_name : " + oLast_name + " -> " + Last_name;
                 cust.Email =  Email;
                 data += ",Email : " + oEmail + " -> " + Email;
                 cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                 data += ",Role : " + oRole + " -> " + Role;
                 cust.Activated =  Int16.Parse( Activated == null ? "1" : Activated)  ;
                 data += ",Activated : " + oActivated + " -> " + Activated;
                 cust.Company_name =  long.Parse( Company_name == null ? "1" : Company_name)  ;
                 data += ",Company_name : " + oCompany_name + " -> " + Company_name;
                 if(andPassword)
                 {
                    cust.Password =  Encoding.ASCII.GetBytes( Password);
                    cust.Password2 =  Encoding.ASCII.GetBytes( Password2);
                 }
                response = c.update_authenticate_Vendor(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_AUTHENTICATE_VENDOR_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_AUTHENTICATE_VENDOR_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_AUTHENTICATE_VENDOR_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Customer_Company(string Company_name, bool returnID = false )
        { 
            string response = ""; 
            Customer_Company c = new Customer_Company();  
            string data = "";
            try
            { 

                Equipment_hub_Customer_Company cust = new Equipment_hub_Customer_Company(); 
                cust.Company_name =  Company_name;
                data += ",Company_name : " + Company_name;
                response = c.add_Customer_Company(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CUSTOMER_COMPANY_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_CUSTOMER_COMPANY_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CUSTOMER_COMPANY_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Customer_Company";
                Audit.InsertAudit((int)eventzz.ERROR_CUSTOMER_COMPANY_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Customer_Company> get_Customer_Company(string sql)
        { 
            List<Equipment_hub_Customer_Company> response = null;
            try
            { 
                Customer_Company c = new Customer_Company(); 
                response = c.get_Customer_Company(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_CUSTOMER_COMPANY_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Customer_Company( string id, string oCompany_name,string Company_name, bool andPassword = true) 
        { 
            string response = ""; 
            Customer_Company c = new Customer_Company();   
            string data = "";
            try
            { 
                Equipment_hub_Customer_Company cust = c.get_Customer_Company(" where id = " + id  )[0]; 
                cust.Company_name =  Company_name;
                data += ",Company_name : " + oCompany_name + " -> " + Company_name;
                response = c.update_Customer_Company(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_CUSTOMER_COMPANY_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_CUSTOMER_COMPANY_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_CUSTOMER_COMPANY_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Duration_Type(string Duration_name, bool returnID = false )
        { 
            string response = ""; 
            Duration_Type c = new Duration_Type();  
            string data = "";
            try
            { 

                Equipment_hub_Duration_Type cust = new Equipment_hub_Duration_Type(); 
                cust.Duration_name =  Duration_name;
                data += ",Duration_name : " + Duration_name;
                response = c.add_Duration_Type(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_DURATION_TYPE_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_DURATION_TYPE_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_DURATION_TYPE_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Duration_Type";
                Audit.InsertAudit((int)eventzz.ERROR_DURATION_TYPE_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Duration_Type> get_Duration_Type(string sql)
        { 
            List<Equipment_hub_Duration_Type> response = null;
            try
            { 
                Duration_Type c = new Duration_Type(); 
                response = c.get_Duration_Type(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_DURATION_TYPE_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Duration_Type( string id, string oDuration_name,string Duration_name, bool andPassword = true) 
        { 
            string response = ""; 
            Duration_Type c = new Duration_Type();   
            string data = "";
            try
            { 
                Equipment_hub_Duration_Type cust = c.get_Duration_Type(" where id = " + id  )[0]; 
                cust.Duration_name =  Duration_name;
                data += ",Duration_name : " + oDuration_name + " -> " + Duration_name;
                response = c.update_Duration_Type(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_DURATION_TYPE_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_DURATION_TYPE_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_DURATION_TYPE_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Equipment(string Equipment_type,string Equipment_code,string Vendor, bool returnID = false )
        { 
            string response = ""; 
            Equipment c = new Equipment();  
            string data = "";
            try
            { 

                Equipment_hub_Equipment cust = new Equipment_hub_Equipment(); 
                cust.Equipment_type =  long.Parse( Equipment_type == null ? "1" : Equipment_type)  ;
                data += ",Equipment_type : " + Equipment_type;
                cust.Equipment_code =  Equipment_code;
                 data += ",Equipment_code : " + Equipment_code;
                cust.Vendor =  long.Parse( Vendor == null ? "1" : Vendor)  ;
                 data += ",Vendor : " + Vendor;
                response = c.add_Equipment(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_EQUIPMENT_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Equipment";
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Equipment_data> get_Equipment(string sql)
        { 
            List<Equipment_hub_Equipment_data> response = null;
            try
            { 
                Equipment c = new Equipment(); 
                response = c.get_Equipment_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Equipment( string id, string oEquipment_type,string oEquipment_code,string oVendor,string Equipment_type,string Equipment_code,string Vendor, bool andPassword = true) 
        { 
            string response = ""; 
            Equipment c = new Equipment();   
            string data = "";
            try
            { 
                Equipment_hub_Equipment cust = c.get_Equipment(" where id = " + id  )[0]; 
                cust.Equipment_type =  long.Parse( Equipment_type == null ? "1" : Equipment_type)  ;
                data += ",Equipment_type : " + oEquipment_type + " -> " + Equipment_type;
                 cust.Equipment_code =  Equipment_code;
                 data += ",Equipment_code : " + oEquipment_code + " -> " + Equipment_code;
                 cust.Vendor =  long.Parse( Vendor == null ? "1" : Vendor)  ;
                 data += ",Vendor : " + oVendor + " -> " + Vendor;
                response = c.update_Equipment(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_EQUIPMENT_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Equipment_Activity(string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date, bool returnID = false )
        { 
            string response = ""; 
            Equipment_Activity c = new Equipment_Activity();  
            string data = "";
            try
            { 

                Equipment_hub_Equipment_Activity cust = new Equipment_hub_Equipment_Activity(); 
                cust.Equipment_type =  long.Parse( Equipment_type == null ? "1" : Equipment_type)  ;
                data += ",Equipment_type : " + Equipment_type;
                cust.Number_of_units =  Int16.Parse( Number_of_units == null ? "1" : Number_of_units)  ;
                 data += ",Number_of_units : " + Number_of_units;
                cust.Rent_rate =  long.Parse( Rent_rate == null ? "1" : Rent_rate)  ;
                 data += ",Rent_rate : " + Rent_rate;
                cust.Rent_rate_qunatity =  Int16.Parse( Rent_rate_qunatity == null ? "1" : Rent_rate_qunatity)  ;
                 data += ",Rent_rate_qunatity : " + Rent_rate_qunatity;
                cust.Customer_company =  long.Parse( Customer_company == null ? "1" : Customer_company)  ;
                 data += ",Customer_company : " + Customer_company;
                cust.Vendor_company =  long.Parse( Vendor_company == null ? "1" : Vendor_company)  ;
                 data += ",Vendor_company : " + Vendor_company;
                cust.Activity_status =  long.Parse( Activity_status == null ? "1" : Activity_status)  ;
                 data += ",Activity_status : " + Activity_status;
                 cust.Activity_date = System.DateTime.Now;
                response = c.add_Equipment_Activity(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_ACTIVITY_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_EQUIPMENT_ACTIVITY_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_ACTIVITY_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Equipment_Activity";
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_ACTIVITY_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Equipment_Activity_data> get_Equipment_Activity(string sql)
        { 
            List<Equipment_hub_Equipment_Activity_data> response = null;
            try
            { 
                Equipment_Activity c = new Equipment_Activity(); 
                response = c.get_Equipment_Activity_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_ACTIVITY_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Equipment_Activity( string id, string oEquipment_type,string oNumber_of_units,string oRent_rate,string oRent_rate_qunatity,string oCustomer_company,string oVendor_company,string oActivity_status,string oActivity_date,string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date, bool andPassword = true) 
        { 
            string response = ""; 
            Equipment_Activity c = new Equipment_Activity();   
            string data = "";
            try
            { 
                Equipment_hub_Equipment_Activity cust = c.get_Equipment_Activity(" where id = " + id  )[0]; 
                cust.Equipment_type =  long.Parse( Equipment_type == null ? "1" : Equipment_type)  ;
                data += ",Equipment_type : " + oEquipment_type + " -> " + Equipment_type;
                 cust.Number_of_units =  Int16.Parse( Number_of_units == null ? "1" : Number_of_units)  ;
                 data += ",Number_of_units : " + oNumber_of_units + " -> " + Number_of_units;
                 cust.Rent_rate =  long.Parse( Rent_rate == null ? "1" : Rent_rate)  ;
                 data += ",Rent_rate : " + oRent_rate + " -> " + Rent_rate;
                 cust.Rent_rate_qunatity =  Int16.Parse( Rent_rate_qunatity == null ? "1" : Rent_rate_qunatity)  ;
                 data += ",Rent_rate_qunatity : " + oRent_rate_qunatity + " -> " + Rent_rate_qunatity;
                 cust.Customer_company =  long.Parse( Customer_company == null ? "1" : Customer_company)  ;
                 data += ",Customer_company : " + oCustomer_company + " -> " + Customer_company;
                 cust.Vendor_company =  long.Parse( Vendor_company == null ? "1" : Vendor_company)  ;
                 data += ",Vendor_company : " + oVendor_company + " -> " + Vendor_company;
                 cust.Activity_status =  long.Parse( Activity_status == null ? "1" : Activity_status)  ;
                 data += ",Activity_status : " + oActivity_status + " -> " + Activity_status;
                response = c.update_Equipment_Activity(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_EQUIPMENT_ACTIVITY_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_ACTIVITY_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_ACTIVITY_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Equipment_Type(string Name_of_equipment, bool returnID = false )
        { 
            string response = ""; 
            Equipment_Type c = new Equipment_Type();  
            string data = "";
            try
            { 

                Equipment_hub_Equipment_Type cust = new Equipment_hub_Equipment_Type(); 
                cust.Name_of_equipment =  Name_of_equipment;
                data += ",Name_of_equipment : " + Name_of_equipment;
                response = c.add_Equipment_Type(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_TYPE_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_EQUIPMENT_TYPE_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_TYPE_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Equipment_Type";
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_TYPE_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Equipment_Type> get_Equipment_Type(string sql)
        { 
            List<Equipment_hub_Equipment_Type> response = null;
            try
            { 
                Equipment_Type c = new Equipment_Type(); 
                response = c.get_Equipment_Type(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_TYPE_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Equipment_Type( string id, string oName_of_equipment,string Name_of_equipment, bool andPassword = true) 
        { 
            string response = ""; 
            Equipment_Type c = new Equipment_Type();   
            string data = "";
            try
            { 
                Equipment_hub_Equipment_Type cust = c.get_Equipment_Type(" where id = " + id  )[0]; 
                cust.Name_of_equipment =  Name_of_equipment;
                data += ",Name_of_equipment : " + oName_of_equipment + " -> " + Name_of_equipment;
                response = c.update_Equipment_Type(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_EQUIPMENT_TYPE_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_EQUIPMENT_TYPE_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_EQUIPMENT_TYPE_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Rent_Rate(string Equipement_type,string Duration_type,string Qunatity,string Price,string Entry_date,string Vendor, bool returnID = false )
        { 
            string response = ""; 
            Rent_Rate c = new Rent_Rate();  
            string data = "";
            try
            { 

                Equipment_hub_Rent_Rate cust = new Equipment_hub_Rent_Rate(); 
                cust.Equipement_type =  long.Parse( Equipement_type == null ? "1" : Equipement_type)  ;
                data += ",Equipement_type : " + Equipement_type;
                cust.Duration_type =  long.Parse( Duration_type == null ? "1" : Duration_type)  ;
                 data += ",Duration_type : " + Duration_type;
                cust.Qunatity =  Int16.Parse( Qunatity == null ? "1" : Qunatity)  ;
                 data += ",Qunatity : " + Qunatity;
                cust.Price =   float.Parse(Price);
                 data += ",Price : " + Price;
                 cust.Entry_date = System.DateTime.Now;
                cust.Vendor =  long.Parse( Vendor == null ? "1" : Vendor)  ;
                 data += ",Vendor : " + Vendor;
                response = c.add_Rent_Rate(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RENT_RATE_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RENT_RATE_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RENT_RATE_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Rent_Rate";
                Audit.InsertAudit((int)eventzz.ERROR_RENT_RATE_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Rent_Rate_data> get_Rent_Rate(string sql, bool addExtra = false)
        { 
            List<Equipment_hub_Rent_Rate_data> response = null;
            try
            { 
                Rent_Rate c = new Rent_Rate();
                response = c.get_Rent_Rate_linked(sql, addExtra); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_RENT_RATE_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Rent_Rate( string id, string oEquipement_type,string oDuration_type,string oQunatity,string oPrice,string oEntry_date,string oVendor,string Equipement_type,string Duration_type,string Qunatity,string Price,string Entry_date,string Vendor, bool andPassword = true) 
        { 
            string response = ""; 
            Rent_Rate c = new Rent_Rate();   
            string data = "";
            try
            { 
                Equipment_hub_Rent_Rate cust = c.get_Rent_Rate(" where id = " + id  )[0]; 
                cust.Equipement_type =  long.Parse( Equipement_type == null ? "1" : Equipement_type)  ;
                data += ",Equipement_type : " + oEquipement_type + " -> " + Equipement_type;
                 cust.Duration_type =  long.Parse( Duration_type == null ? "1" : Duration_type)  ;
                 data += ",Duration_type : " + oDuration_type + " -> " + Duration_type;
                 cust.Qunatity =  Int16.Parse( Qunatity == null ? "1" : Qunatity)  ;
                 data += ",Qunatity : " + oQunatity + " -> " + Qunatity;
                 cust.Price =   float.Parse(Price);
                 data += ",Price : " + oPrice + " -> " + Price;
                 cust.Vendor =  long.Parse( Vendor == null ? "1" : Vendor)  ;
                 data += ",Vendor : " + oVendor + " -> " + Vendor;
                response = c.update_Rent_Rate(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RENT_RATE_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RENT_RATE_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_RENT_RATE_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_right_Admin(string Rightname, bool returnID = false )
        { 
            string response = ""; 
            right_Admin c = new right_Admin();  
            string data = "";
            try
            { 

                Equipment_hub_right_Admin cust = new Equipment_hub_right_Admin(); 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + Rightname;
                response = c.add_right_Admin(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_ADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding right_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_right_Admin> get_right_Admin(string sql)
        { 
            List<Equipment_hub_right_Admin> response = null;
            try
            { 
                right_Admin c = new right_Admin(); 
                response = c.get_right_Admin(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_right_Admin( string id, string oRightname,string Rightname, bool andPassword = true) 
        { 
            string response = ""; 
            right_Admin c = new right_Admin();   
            string data = "";
            try
            { 
                Equipment_hub_right_Admin cust = c.get_right_Admin(" where id = " + id  )[0]; 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + oRightname + " -> " + Rightname;
                response = c.update_right_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_right_Vendor(string Rightname, bool returnID = false )
        { 
            string response = ""; 
            right_Vendor c = new right_Vendor();  
            string data = "";
            try
            { 

                Equipment_hub_right_Vendor cust = new Equipment_hub_right_Vendor(); 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + Rightname;
                response = c.add_right_Vendor(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_VENDOR_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_VENDOR_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_VENDOR_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding right_Vendor";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_VENDOR_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_right_Vendor> get_right_Vendor(string sql)
        { 
            List<Equipment_hub_right_Vendor> response = null;
            try
            { 
                right_Vendor c = new right_Vendor(); 
                response = c.get_right_Vendor(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_VENDOR_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_right_Vendor( string id, string oRightname,string Rightname, bool andPassword = true) 
        { 
            string response = ""; 
            right_Vendor c = new right_Vendor();   
            string data = "";
            try
            { 
                Equipment_hub_right_Vendor cust = c.get_right_Vendor(" where id = " + id  )[0]; 
                cust.Rightname =  Rightname;
                data += ",Rightname : " + oRightname + " -> " + Rightname;
                response = c.update_right_Vendor(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_RIGHT_VENDOR_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_RIGHT_VENDOR_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_RIGHT_VENDOR_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_Admin(string Rolename, string selectedRights, bool returnID = false )
        { 
            string response = ""; 
            role_Admin c = new role_Admin();  
            string data = "";
            try
            { 

                Equipment_hub_role_Admin cust = new Equipment_hub_role_Admin(); 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + Rolename;
                response = c.add_role_Admin(cust,selectedRights,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_ADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_role_Admin> get_role_Admin(string sql)
        { 
            List<Equipment_hub_role_Admin> response = null;
            try
            { 
                role_Admin c = new role_Admin(); 
                response = c.get_role_Admin(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_Admin( string id, string oRolename,string Rolename, string selectedRights, bool andPassword = true) 
        { 
            string response = ""; 
            role_Admin c = new role_Admin();   
            string data = "";
            try
            { 
                Equipment_hub_role_Admin cust = c.get_role_Admin(" where id = " + id  )[0]; 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + oRolename + " -> " + Rolename;
                response = c.update_role_Admin(cust, selectedRights);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_right_Admin(string Role,string Right, bool returnID = false )
        { 
            string response = ""; 
            role_right_Admin c = new role_right_Admin();  
            string data = "";
            try
            { 

                Equipment_hub_role_right_Admin cust = new Equipment_hub_role_right_Admin(); 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + Role;
                cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + Right;
                response = c.add_role_right_Admin(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_ADMIN_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_ADMIN_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_ADMIN_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_right_Admin";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_role_right_Admin_data> get_role_right_Admin(string sql)
        { 
            List<Equipment_hub_role_right_Admin_data> response = null;
            try
            { 
                role_right_Admin c = new role_right_Admin(); 
                response = c.get_role_right_Admin_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_right_Admin( string id, string oRole,string oRight,string Role,string Right, bool andPassword = true) 
        { 
            string response = ""; 
            role_right_Admin c = new role_right_Admin();   
            string data = "";
            try
            { 
                Equipment_hub_role_right_Admin cust = c.get_role_right_Admin(" where id = " + id  )[0]; 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + oRole + " -> " + Role;
                 cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + oRight + " -> " + Right;
                response = c.update_role_right_Admin(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_ADMIN_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_ADMIN_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_right_Vendor(string Role,string Right, bool returnID = false )
        { 
            string response = ""; 
            role_right_Vendor c = new role_right_Vendor();  
            string data = "";
            try
            { 

                Equipment_hub_role_right_Vendor cust = new Equipment_hub_role_right_Vendor(); 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + Role;
                cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + Right;
                response = c.add_role_right_Vendor(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_VENDOR_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_VENDOR_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_VENDOR_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_right_Vendor";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_VENDOR_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_role_right_Vendor_data> get_role_right_Vendor(string sql)
        { 
            List<Equipment_hub_role_right_Vendor_data> response = null;
            try
            { 
                role_right_Vendor c = new role_right_Vendor(); 
                response = c.get_role_right_Vendor_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_VENDOR_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_right_Vendor( string id, string oRole,string oRight,string Role,string Right, bool andPassword = true) 
        { 
            string response = ""; 
            role_right_Vendor c = new role_right_Vendor();   
            string data = "";
            try
            { 
                Equipment_hub_role_right_Vendor cust = c.get_role_right_Vendor(" where id = " + id  )[0]; 
                cust.Role =  long.Parse( Role == null ? "1" : Role)  ;
                data += ",Role : " + oRole + " -> " + Role;
                 cust.Right =  long.Parse( Right == null ? "1" : Right)  ;
                 data += ",Right : " + oRight + " -> " + Right;
                response = c.update_role_right_Vendor(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_RIGHT_VENDOR_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_RIGHT_VENDOR_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_RIGHT_VENDOR_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_role_Vendor(string Rolename,string Company, string selectedRights, bool returnID = false )
        { 
            string response = ""; 
            role_Vendor c = new role_Vendor();  
            string data = "";
            try
            { 

                Equipment_hub_role_Vendor cust = new Equipment_hub_role_Vendor(); 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + Rolename;
                cust.Company =  long.Parse( Company == null ? "1" : Company)  ;
                 data += ",Company : " + Company;
                response = c.add_role_Vendor(cust,selectedRights,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_VENDOR_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_VENDOR_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_VENDOR_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding role_Vendor";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_VENDOR_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_role_Vendor_data> get_role_Vendor(string sql)
        { 
            List<Equipment_hub_role_Vendor_data> response = null;
            try
            { 
                role_Vendor c = new role_Vendor(); 
                response = c.get_role_Vendor_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_VENDOR_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_role_Vendor( string id, string oRolename,string oCompany,string Rolename,string Company, string selectedRights, bool andPassword = true) 
        { 
            string response = ""; 
            role_Vendor c = new role_Vendor();   
            string data = "";
            try
            { 
                Equipment_hub_role_Vendor cust = c.get_role_Vendor(" where id = " + id  )[0]; 
                cust.Rolename =  Rolename;
                data += ",Rolename : " + oRolename + " -> " + Rolename;
                 cust.Company =  long.Parse( Company == null ? "1" : Company)  ;
                 data += ",Company : " + oCompany + " -> " + Company;
                response = c.update_role_Vendor(cust, selectedRights);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_ROLE_VENDOR_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_ROLE_VENDOR_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_ROLE_VENDOR_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Selected_Equipement(string Equipment_activity,string Equipment, bool returnID = false )
        { 
            string response = ""; 
            Selected_Equipement c = new Selected_Equipement();  
            string data = "";
            try
            { 

                Equipment_hub_Selected_Equipement cust = new Equipment_hub_Selected_Equipement(); 
                cust.Equipment_activity =  long.Parse( Equipment_activity == null ? "1" : Equipment_activity)  ;
                data += ",Equipment_activity : " + Equipment_activity;
                cust.Equipment =  long.Parse( Equipment == null ? "1" : Equipment)  ;
                 data += ",Equipment : " + Equipment;
                response = c.add_Selected_Equipement(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_SELECTED_EQUIPEMENT_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_SELECTED_EQUIPEMENT_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_SELECTED_EQUIPEMENT_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Selected_Equipement";
                Audit.InsertAudit((int)eventzz.ERROR_SELECTED_EQUIPEMENT_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Selected_Equipement_data> get_Selected_Equipement(string sql)
        { 
            List<Equipment_hub_Selected_Equipement_data> response = null;
            try
            { 
                Selected_Equipement c = new Selected_Equipement(); 
                response = c.get_Selected_Equipement_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_SELECTED_EQUIPEMENT_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Selected_Equipement( string id, string oEquipment_activity,string oEquipment,string Equipment_activity,string Equipment, bool andPassword = true) 
        { 
            string response = ""; 
            Selected_Equipement c = new Selected_Equipement();   
            string data = "";
            try
            { 
                Equipment_hub_Selected_Equipement cust = c.get_Selected_Equipement(" where id = " + id  )[0]; 
                cust.Equipment_activity =  long.Parse( Equipment_activity == null ? "1" : Equipment_activity)  ;
                data += ",Equipment_activity : " + oEquipment_activity + " -> " + Equipment_activity;
                 cust.Equipment =  long.Parse( Equipment == null ? "1" : Equipment)  ;
                 data += ",Equipment : " + oEquipment + " -> " + Equipment;
                response = c.update_Selected_Equipement(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_SELECTED_EQUIPEMENT_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_SELECTED_EQUIPEMENT_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_SELECTED_EQUIPEMENT_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Vendor_Company(string Company_name, bool returnID = false )
        { 
            string response = ""; 
            Vendor_Company c = new Vendor_Company();  
            string data = "";
            try
            { 

                Equipment_hub_Vendor_Company cust = new Equipment_hub_Vendor_Company(); 
                cust.Company_name =  Company_name;
                data += ",Company_name : " + Company_name;
                response = c.add_Vendor_Company(cust,  returnID  );
               if( returnID  ){
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_VENDOR_COMPANY_ADD, data, getVal(), true); 
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_VENDOR_COMPANY_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = "Creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_VENDOR_COMPANY_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Vendor_Company";
                Audit.InsertAudit((int)eventzz.ERROR_VENDOR_COMPANY_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Vendor_Company> get_Vendor_Company(string sql)
        { 
            List<Equipment_hub_Vendor_Company> response = null;
            try
            { 
                Vendor_Company c = new Vendor_Company(); 
                response = c.get_Vendor_Company(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_VENDOR_COMPANY_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Vendor_Company( string id, string oCompany_name,string Company_name, bool andPassword = true) 
        { 
            string response = ""; 
            Vendor_Company c = new Vendor_Company();   
            string data = "";
            try
            { 
                Equipment_hub_Vendor_Company cust = c.get_Vendor_Company(" where id = " + id  )[0]; 
                cust.Company_name =  Company_name;
                data += ",Company_name : " + oCompany_name + " -> " + Company_name;
                response = c.update_Vendor_Company(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_VENDOR_COMPANY_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_VENDOR_COMPANY_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_VENDOR_COMPANY_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
        public static string add_new_Vendor_Payments(string Equipement_activity,string Amount,string Date_payment_was_due,string Date_payment_was_made, bool returnID = false )
        { 
            string response = ""; 
            Vendor_Payments c = new Vendor_Payments();  
            string data = "";
            try
            { 

                Equipment_hub_Vendor_Payments cust = new Equipment_hub_Vendor_Payments(); 
                cust.Equipement_activity =  long.Parse( Equipement_activity == null ? "1" : Equipement_activity)  ;
                data += ",Equipement_activity : " + Equipement_activity;
                cust.Amount =   float.Parse(Amount);
                 data += ",Amount : " + Amount;
                 cust.Date_payment_was_due = System.DateTime.Now;
                response = c.add_Vendor_Payments(cust,  returnID  );
               if( returnID  ){
                    return response;
               }
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_VENDOR_PAYMENTS_ADD, response + " (" + data + ")", getVal(), true);
                    response = "failed create attempt";
                }
                else
                { 
                    response = " creation successful"; 
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_VENDOR_PAYMENTS_ADD, data, getVal(), true); 
                } 
            }
            catch (Exception d)
            {
                response = "Error adding Vendor_Payments";
                Audit.InsertAudit((int)eventzz.ERROR_VENDOR_PAYMENTS_ADD, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " (  " + data + " ) ", getVal(), true); 
            }
            return response;
        }
        public static List<Equipment_hub_Vendor_Payments_data> get_Vendor_Payments(string sql)
        { 
            List<Equipment_hub_Vendor_Payments_data> response = null;
            try
            { 
                Vendor_Payments c = new Vendor_Payments(); 
                response = c.get_Vendor_Payments_linked(sql); 
            }
            catch (Exception d)
            {
                Audit.InsertAudit((int)eventzz.ERROR_VENDOR_PAYMENTS_GET, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : ""), getVal(), true); 
            }  
            return response;
        }
        public static string update_Vendor_Payments( string id, string oEquipement_activity,string oAmount,string oDate_payment_was_due,string oDate_payment_was_made,string Equipement_activity,string Amount,string Date_payment_was_due,string Date_payment_was_made, bool andPassword = true) 
        { 
            string response = ""; 
            Vendor_Payments c = new Vendor_Payments();   
            string data = "";
            try
            { 
                Equipment_hub_Vendor_Payments cust = c.get_Vendor_Payments(" where id = " + id  )[0]; 
                cust.Equipement_activity =  long.Parse( Equipement_activity == null ? "1" : Equipement_activity)  ;
                data += ",Equipement_activity : " + oEquipement_activity + " -> " + Equipement_activity;
                 cust.Amount =   float.Parse(Amount);
                 data += ",Amount : " + oAmount + " -> " + Amount;
                response = c.update_Vendor_Payments(cust);
                if (response.Trim().Length > 0)
                {
                    Audit.InsertAudit((int)eventzz.FAILED_VENDOR_PAYMENTS_UPDATE, data, getVal(), true);
                    response = "Error saving data";
                }
                else
                { 
                    response = "VENDOR_PAYMENTS update successful";
                    Audit.InsertAudit((int)eventzz.SUCCESSFUL_VENDOR_PAYMENTS_UPDATE, data, getVal(), true);
                }
            }
            catch (Exception d)
            {
                response = "Error updating data";
                Audit.InsertAudit((int)eventzz.ERROR_VENDOR_PAYMENTS_UPDATE, d.Message + "  " + (d.InnerException != null ? d.InnerException.Message : "") + " ( " + data + " ) ", getVal(), true);
            }
            return response;
        }
    }
}
