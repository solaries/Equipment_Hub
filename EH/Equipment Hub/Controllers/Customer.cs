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
using Equipment_hub.BusinessLogic;  
using System.Net;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace Equipment_hub.Controllers
{ 
    [Authorize]
    public class CUSTOMERController : Controller
    {  


          private void getStatus(bool clearStatus =true)
        { 
            if (Session["status"] != null)
            {
                if (Session["status"].ToString().Trim().Length > 0)
                {
                    ViewBag.status = Session["status"].ToString();
                    if (clearStatus)
                    {
                         Session["status"] = "";
                    }
                }
            }
            if (Session["down"] != null)
            {
                if (Session["down"].ToString().Trim().Length > 0)
                {
                    ViewBag.down = Session["down"].ToString();
                    Session["down"] = ""; 
                }
            }
        }
        private bool validateAccessToken(string token)
        {
             string urlPath = Request.Url.ToString().Split(new string[] { Request.Path.ToString() }, StringSplitOptions.RemoveEmptyEntries)[0];
             if (Request.Path.ToString().Trim().Length == 1)
             {
                 urlPath = Request.Url.ToString();
             }
             var newHttpRequest = (HttpWebRequest)WebRequest.Create(urlPath + "/api/GoodToken");
             var data = Encoding.ASCII.GetBytes("");
             newHttpRequest.Method = "GET";
             newHttpRequest.Headers.Add("Authorization", "Bearer " + token); 
             try
             {
                 var newHttpResponse = (HttpWebResponse)newHttpRequest.GetResponse();
                 var responseString = new StreamReader(newHttpResponse.GetResponseStream()).ReadToEnd();
             }
             catch (Exception ex)
             {
                 return false;
             } 
             return true;
        } 
        private string doAuthenticate(string userName, string password, string clientID)
        {
             string result = ""; 
             string dataToSend = "&username=" + HttpUtility.UrlEncode(userName) + "&password=" + HttpUtility.UrlEncode(password) + "&clientid=" + HttpUtility.UrlEncode(clientID) + "&grant_type=password";
             string urlPath = Request.Url.ToString().Split(new string[] { Request.Path.ToString() }, StringSplitOptions.RemoveEmptyEntries)[0];
             if (Request.Path.ToString().Trim().Length == 1)
             {
                 urlPath = Request.Url.ToString();
             }
             var newHttpRequest = (HttpWebRequest)WebRequest.Create(urlPath + "/token");
             var data = Encoding.ASCII.GetBytes(dataToSend);
             newHttpRequest.Method = "POST"; 
             newHttpRequest.ContentType = "application/x-www-form-urlencoded"; 
             newHttpRequest.ContentLength = data.Length;
             using (var streamProcess = newHttpRequest.GetRequestStream())
             {
                 streamProcess.Write(data, 0, data.Length);
             }
             try
             {
                 var newHttpResponse = (HttpWebResponse)newHttpRequest.GetResponse();
                 var responseString = new StreamReader(newHttpResponse.GetResponseStream()).ReadToEnd();
                 dynamic passString = JsonConvert.DeserializeObject<dynamic>(responseString);
                 result = (string)passString.access_token; 
             }
             catch (Exception d)
             { 
             }
             return result;
        }

        [AllowAnonymous]
        public ActionResult login()
        {
            Audit.protocol();
            getStatus();
            Session.Clear(); 
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult login(string First_name,string Last_name,string Email,string Password,string Password2,string Activated,string Company_name,   string forgot)
        {  
            Audit.protocol();
            string token = doAuthenticate(userName: Email, password: Password, clientID: "Customer");
            bool result = validateAccessToken(token);
            List<Equipment_hub_authenticate_Customer_data> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate( password:  Password, email: Email ); 
                response = ( List<Equipment_hub_authenticate_Customer_data>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "Customer" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Activated"] =  response[0].Activated;
                            Session["Customer_company"] =  response[0].Company_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["status"] = "Please change your password";
                            return RedirectToAction("Change_Password", "Customer");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "Customer" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Activated"] =  response[0].Activated;
                            Session["Customer_company"] =  response[0].Company_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            return RedirectToAction("Change_Password", "Customer"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "Customer");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "Customer");
                } 
            }
            else { 
                ActionResult xx = forgotauthenticate_Customer(   Email: Email ); 
                response = ( List<Equipment_hub_authenticate_Customer_data>)Session["response"]; 
                return RedirectToAction("Login", "Customer");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password )
        { 
            Audit.protocol(); 
            List<Equipment_hub_authenticate_Customer_data> response = null;  
            password =  Audit.GetEncodedHash(password, "doing it well") ;
            response =  centralCalls.get_authenticate_Customer(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            Session["response"]  = response;
            return Content(JsonConvert.SerializeObject((List<Equipment_hub_authenticate_Customer_data>)response));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_Customer(string Email )
        {   
            Audit.protocol();
            List<Equipment_hub_authenticate_Customer_data> response = null; 
            response =  centralCalls.get_authenticate_Customer(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_Customer(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oActivated: response[0].Activated.ToString() ,oCompany_name: response[0].Company_name.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr),Activated: response[0].Activated.ToString() ,Company_name: response[0].Company_name.ToString() ) ; 
                        string mailSubject = "Profile password reset on (code joh) Equipment HUB";
                        Session["status"] = "Password reset successful, please check your email for your new password.";
                        string mailBody = "Hi <br><br>Your password has been successfully reset on the (code joh) Equipment HUB platform. Please log in with following credentials: <br><br> Email:" + response[0].Email + "<br><br>password :" + strRND + "<br><br><br>Regards<br><br>";
                        Audit.SendMail(Email, mailSubject, mailBody, " "); 
                } 
            } 
            Session["response"]  = response; 
             return Content(JsonConvert.SerializeObject((List<Equipment_hub_authenticate_Customer_data>)response)); ;
        }   


        [AllowAnonymous]
        public ActionResult Register()
        {
            Audit.protocol();
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Register(string First_name,string Last_name,string Email,string Activated,string Company_name_Company_name)
        {  
                Audit.protocol();
                string response =null;
                ActionResult xx = addRegister(First_name:  First_name,Last_name:  Last_name,Email:  Email,Activated:  Activated,Company_name_Company_name:  Company_name_Company_name ); 
                response = ( string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccessf") == -1 ){
                        return RedirectToAction("Register", "Customer"); 
                }
                else
                {
                    return RedirectToAction("Login", "Customer");
                } 
        } 

        [AllowAnonymous]
        public ActionResult addRegister(string First_name,string Last_name,string Email,string Activated,string Company_name_Company_name)
        { 
                Audit.protocol();
                string response = "";  
                string strRND = Audit.GenerateRandom();
                byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
 
                string Company_name =  centralCalls.add_new_Customer_Company(Company_name:  Company_name_Company_name, returnID: true);
                response =  centralCalls.add_new_authenticate_Customer(First_name:  First_name,Last_name:  Last_name,Email:  Email,Password: Encoding.ASCII.GetString(arr)  ,Password2: Encoding.ASCII.GetString(arr)  ,Activated:  Activated,Company_name:  Company_name);
                if(response.IndexOf("uccessf") > -1 ){
                    string mailSubject = "Customer Profile creation on (code joh) Equipment HUB";
                    string mailBody = "Hi <br><br>You have been successfully profiled on the (code joh) Equipment HUB platform. Please log in with following credentials: <br><br> Email: " + Email + "<br><br>password : " + strRND + "<br><br><br>Regards<br><br>";
                    Audit.SendMail(Email, mailSubject, mailBody, " "); 
                }
                Session["response"]  = response;
                return Content( response);
        }



        [AllowAnonymous]
        public ActionResult Change_Password()
        {
                Audit.protocol();
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Customer");
            }
            ViewBag.first_name=Session["first_name"];
            ViewBag.last_name=Session["last_name"];
            ViewBag.email=Session["email"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Change_Password(string email,string password,string npassword )
        {  
                Audit.protocol();
                ActionResult xx = updatePassword( password:  password,npassword:  npassword , email: email ); 
                Session["status"] = (string)Session["response"];
                if(Session["response"].ToString().IndexOf("update successful") > -1)
                {
                    Session["status"] = "Password Changed Successfully";
                    return RedirectToAction("Login", "Customer") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "Customer") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            Audit.protocol();
            List<Equipment_hub_authenticate_Customer_data> response = null; 
            string result = "Authentication failed"; 
            string strRND11 = password;
            byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_Customer(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_Customer(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oActivated: response[0].Activated.ToString() ,oCompany_name: response[0].Company_name.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2),Activated: response[0].Activated.ToString() ,Company_name: response[0].Company_name.ToString() ) ; 
                } 
            } 
            Session["response"]  = result; 
            return Content((string)result);
        }   


        [AllowAnonymous]
        public ActionResult new_Equipment_Activity()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Customer");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Customer");
            }
            ViewBag.Data0 =  centralCalls.get_Equipment_Type("");
            ViewBag.Data1 =  centralCalls.get_Rent_Rate("");
            ViewBag.Data2 =  centralCalls.get_Vendor_Company("");
            ViewBag.Data3 =  centralCalls.get_Activity_Status("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Equipment_Activity(string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Customer");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Customer");
            }
            Activity_status = "1";
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Customer");
                }
                ActionResult xx =  add_Equipment_Activity(Equipment_type: Equipment_type,Number_of_units: Number_of_units,Rent_rate: Rent_rate,Rent_rate_qunatity: Rent_rate_qunatity,Customer_company: Customer_company,Vendor_company: Vendor_company,Activity_status: Activity_status,Activity_date: Activity_date,token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Equipment_Activity", "Customer");
        } 

        [AllowAnonymous]
        public ActionResult add_Equipment_Activity(string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date,string token)
        { 
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_Equipment_Activity(Equipment_type: Equipment_type,Number_of_units: Number_of_units,Rent_rate: Rent_rate,Rent_rate_qunatity: Rent_rate_qunatity,Customer_company: Customer_company,Vendor_company: Vendor_company,Activity_status: Activity_status,Activity_date: Activity_date);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Equipment_Activity()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Customer");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Customer");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Customer");
                }
            List<Equipment_hub_Equipment_Activity> response = null; 
           ActionResult d =  view_it_Equipment_Activity(Session["token"].ToString()  ); 
            return View((List<Equipment_hub_Equipment_Activity_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Equipment_Activity(string token)
        {
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Equipment_Activity>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Equipment_Activity(" where a.Customer_Company = " + Session["Customer_company"]);
            return Content(JsonConvert.SerializeObject( ((List<Equipment_hub_Equipment_Activity_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Equipment_Activity(string id,string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Customer");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Customer");
            }
            ViewBag.Data0 =  centralCalls.get_Equipment_Type("");
            ViewBag.Data1 =  centralCalls.get_Rent_Rate("");
            ViewBag.Data2 =  centralCalls.get_Vendor_Company("");
            ViewBag.Data3 =  centralCalls.get_Activity_Status("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Equipment_type = Equipment_type;
 ViewBag.Number_of_units = Number_of_units;
 ViewBag.Rent_rate = Rent_rate;
 ViewBag.Rent_rate_qunatity = Rent_rate_qunatity;
 ViewBag.Customer_company = Customer_company;
 ViewBag.Vendor_company = Vendor_company;
 ViewBag.Activity_status = Activity_status;
 ViewBag.Activity_date = Activity_date;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Equipment_Activity(string id,string oEquipment_type,string oNumber_of_units,string oRent_rate,string oRent_rate_qunatity,string oCustomer_company,string oVendor_company,string oActivity_status,string oActivity_date,string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Customer");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Customer");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Customer");
                }
            string response =null;
                ActionResult xx =  update_Equipment_Activity(id:id,oEquipment_type:  oEquipment_type,oNumber_of_units:  oNumber_of_units,oRent_rate:  oRent_rate,oRent_rate_qunatity:  oRent_rate_qunatity,oCustomer_company:  oCustomer_company,oVendor_company:  oVendor_company,oActivity_status:  oActivity_status,oActivity_date:  oActivity_date,Equipment_type: Equipment_type,Number_of_units: Number_of_units,Rent_rate: Rent_rate,Rent_rate_qunatity: Rent_rate_qunatity,Customer_company: Customer_company,Vendor_company: Vendor_company,Activity_status: Activity_status,Activity_date: Activity_date, token: Session["token"].ToString() ); 
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Equipment_Activity", "Customer");
                }
                else{
                     return View();
                }
                return RedirectToAction("new_Equipment_Activity", "Customer");
        } 

        [AllowAnonymous]
        public ActionResult update_Equipment_Activity(string id, string oEquipment_type,string oNumber_of_units,string oRent_rate,string oRent_rate_qunatity,string oCustomer_company,string oVendor_company,string oActivity_status,string oActivity_date,string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date,string token)
        { 
                Audit.protocol();
                Session["status"] = "";
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Equipment_Activity(id:id,oEquipment_type:  oEquipment_type,oNumber_of_units:  oNumber_of_units,oRent_rate:  oRent_rate,oRent_rate_qunatity:  oRent_rate_qunatity,oCustomer_company:  oCustomer_company,oVendor_company:  oVendor_company,oActivity_status:  oActivity_status,oActivity_date:  oActivity_date,Equipment_type: Equipment_type,Number_of_units: Number_of_units,Rent_rate: Rent_rate,Rent_rate_qunatity: Rent_rate_qunatity,Customer_company: Customer_company,Vendor_company: Vendor_company,Activity_status: Activity_status,Activity_date: Activity_date,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }


   
    }
} 
