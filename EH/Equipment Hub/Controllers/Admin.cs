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
    public class ADMINController : Controller
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
        public ActionResult login(string First_name,string Last_name,string Email,string Role,string Password,string Password2,   string forgot)
        {  
            Audit.protocol();
            string token = doAuthenticate(userName: Email, password: Password, clientID: "Admin");
            bool result = validateAccessToken(token);
            List<Equipment_hub_authenticate_Admin_data> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate( password:  Password, email: Email ); 
                response = ( List<Equipment_hub_authenticate_Admin_data>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "Admin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            List<Equipment_hub_right_Admin> rightList = centralCalls.get_right_Admin(" where id in (select `right` from equipment_hub_role_right_admin where role =" + response[0].Role.ToString() + " )   order by rightname");
                            string strRightList = "";
                            foreach (Equipment_hub_right_Admin right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList; 
                            Session["role"] = response[0].Role;
                            Session["status"] = "Please change your password";
                            return RedirectToAction("Change_Password", "Admin");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "Admin" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["role"] = response[0].Role;
                            List<Equipment_hub_right_Admin> rightList = centralCalls.get_right_Admin(" where id in (select `right` from equipment_hub_role_right_admin where role =" + response[0].Role.ToString() + " )   order by rightname");
                            string strRightList = "";
                            foreach (Equipment_hub_right_Admin right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList;
                            return RedirectToAction("view_Equipment_Activity", "Admin"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "Admin");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "Admin");
                } 
            }
            else { 
                ActionResult xx = forgotauthenticate_Admin(   Email: Email ); 
                response = ( List<Equipment_hub_authenticate_Admin_data>)Session["response"]; 
                return RedirectToAction("Login", "Admin");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password )
        { 
            Audit.protocol(); 
            List<Equipment_hub_authenticate_Admin_data> response = null;  
            password =  Audit.GetEncodedHash(password, "doing it well") ;
            response =  centralCalls.get_authenticate_Admin(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            Session["response"]  = response;
            return Content(JsonConvert.SerializeObject((List<Equipment_hub_authenticate_Admin_data>)response));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_Admin(string Email )
        {   
            Audit.protocol();
            List<Equipment_hub_authenticate_Admin_data> response = null; 
            response =  centralCalls.get_authenticate_Admin(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_Admin(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)   , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr)) ; 
                        string mailSubject = "Profile password reset on (code joh) Equipment HUB";
                        Session["status"] = "Password reset successful, please check your email for your new password.";
                        string mailBody = "Hi <br><br>Your password has been successfully reset on the (code joh) Equipment HUB platform. Please log in with following credentials: <br><br> Email:" + response[0].Email + "<br><br>password :" + strRND + "<br><br><br>Regards<br><br>";
                        Audit.SendMail(Email, mailSubject, mailBody, " "); 
                } 
            } 
            Session["response"]  = response; 
             return Content(JsonConvert.SerializeObject((List<Equipment_hub_authenticate_Admin_data>)response)); ;
        }   


        [AllowAnonymous]
        public ActionResult Change_Password()
        {
                Audit.protocol();
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
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
                    return RedirectToAction("Login", "Admin") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "Admin") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            Audit.protocol();
            List<Equipment_hub_authenticate_Admin_data> response = null; 
            string result = "Authentication failed"; 
            string strRND11 = password;
            byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_Admin(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_Admin(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)   , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2)) ; 
                } 
            } 
            Session["response"]  = result; 
            return Content((string)result);
        }   


        [AllowAnonymous]
        public ActionResult new_Roles()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_right_Admin("   order by rightname");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Roles(string Rolename, string selectedRights )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Roles(Rolename: Rolename,token: Session["token"].ToString() ,role: Session["role"].ToString()  , selectedRights: selectedRights); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Roles", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Roles(string Rolename,string token, string role, string selectedRights)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_role_Admin(Rolename: Rolename, selectedRights: selectedRights);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Roles()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_role_Admin> response = null; 
           ActionResult d =  view_it_Roles( Session["token"].ToString() , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_role_Admin>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Roles(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_role_Admin>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_role_Admin>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_role_Admin("");
            return Content(JsonConvert.SerializeObject( ((List<Equipment_hub_role_Admin>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Roles(string id  ,string Rolename  )
        {  
                Audit.protocol();
            ViewBag.Data0 = centralCalls.get_right_Admin("   order by rightname");
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Rolename = Rolename;

            List<Equipment_hub_role_right_Admin_data> roleRightAdminList = centralCalls.get_role_right_Admin(" where role = " + id);
            string rightSet = "";
            foreach(Equipment_hub_role_right_Admin_data roleRightAdmin in roleRightAdminList)
            {
                rightSet += "sphinxcol" + roleRightAdmin.Right + "sphinxcol";
            }
            ViewBag.rightSet = rightSet;
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Roles(string id,string oRolename,string Rolename , string selectedRights )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Roles(id:id,oRolename:  oRolename,Rolename: Rolename, token: Session["token"].ToString() ,role: Session["role"].ToString()  , selectedRights: selectedRights ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Roles", "Admin");
                }
                else{
                    ViewBag.id=id;
                     ViewBag.Rolename = Rolename;

                    List<Equipment_hub_role_right_Admin_data> roleRightAdminList = centralCalls.get_role_right_Admin(" where role = " + id);
                    string rightSet = "";
                    foreach(Equipment_hub_role_right_Admin_data roleRightAdmin in roleRightAdminList)
                    {
                        rightSet += "sphinxcol" + roleRightAdmin.Right + "sphinxcol";
                    }
                    ViewBag.rightSet = rightSet;
                     return View();
                }
                return RedirectToAction("new_Roles", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Roles(string id, string oRolename,string Rolename,string token, string role, string selectedRights)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_role_Admin(id:id,oRolename:  oRolename,Rolename: Rolename,andPassword: false, selectedRights: selectedRights);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Duration_Type()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Duration Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Duration_Type(string Duration_name )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Duration Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Duration_Type(Duration_name: Duration_name,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Duration_Type", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Duration_Type(string Duration_name,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Duration Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_Duration_Type(Duration_name: Duration_name);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Duration_Type()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Duration Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_Duration_Type> response = null; 
           ActionResult d =  view_it_Duration_Type(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_Duration_Type>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Duration_Type(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Duration Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_Duration_Type>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Duration_Type>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Duration_Type("");
            return Content(JsonConvert.SerializeObject( ((List<Equipment_hub_Duration_Type>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Duration_Type(string id,string Duration_name  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Duration Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Duration_name = Duration_name;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Duration_Type(string id,string oDuration_name,string Duration_name )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Duration Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Duration_Type(id:id,oDuration_name:  oDuration_name,Duration_name: Duration_name, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Duration_Type", "Admin");
                }
                else{
                     return View();
                }
                return RedirectToAction("new_Duration_Type", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Duration_Type(string id, string oDuration_name,string Duration_name,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Duration Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Duration_Type(id:id,oDuration_name:  oDuration_name,Duration_name: Duration_name,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult new_Equipment_Type()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Equipment Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Equipment_Type(string Name_of_equipment )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Equipment Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Equipment_Type(Name_of_equipment: Name_of_equipment,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Equipment_Type", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Equipment_Type(string Name_of_equipment,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Equipment Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_Equipment_Type(Name_of_equipment: Name_of_equipment);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Equipment_Type()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_Equipment_Type> response = null; 
           ActionResult d =  view_it_Equipment_Type(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_Equipment_Type>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Equipment_Type(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_Equipment_Type>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Equipment_Type>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Equipment_Type("");
            return Content(JsonConvert.SerializeObject( ((List<Equipment_hub_Equipment_Type>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Equipment_Type(string id,string Name_of_equipment  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Equipment Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Name_of_equipment = Name_of_equipment;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Equipment_Type(string id,string oName_of_equipment,string Name_of_equipment )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Equipment Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Equipment_Type(id:id,oName_of_equipment:  oName_of_equipment,Name_of_equipment: Name_of_equipment, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Equipment_Type", "Admin");
                }
                else{
                     return View();
                }
                return RedirectToAction("new_Equipment_Type", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Equipment_Type(string id, string oName_of_equipment,string Name_of_equipment,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Equipment Type' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Equipment_Type(id:id,oName_of_equipment:  oName_of_equipment,Name_of_equipment: Name_of_equipment,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult view_Equipment_Activity()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment Activity' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_Equipment_Activity> response = null; 
           ActionResult d =  view_it_Equipment_Activity(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if( Session["status"].ToString()=="You do not have access to this functionality" ){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_Equipment_Activity_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Equipment_Activity(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment Activity' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_Equipment_Activity>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Equipment_Activity>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Equipment_Activity("");
            return  Content(JsonConvert.SerializeObject(  (List<Equipment_hub_Equipment_Activity_data>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Equipment_Activity(string id,string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Equipment Activity' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 = centralCalls.get_Equipment_Type("");
            ViewBag.Data1 = centralCalls.get_Rent_Rate("");
            ViewBag.Data2 = centralCalls.get_Vendor_Company("");
            ViewBag.Data3 = centralCalls.get_Activity_Status("");
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
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Equipment Activity' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Equipment_Activity(id:id,oEquipment_type:  oEquipment_type,oNumber_of_units:  oNumber_of_units,oRent_rate:  oRent_rate,oRent_rate_qunatity:  oRent_rate_qunatity,oCustomer_company:  oCustomer_company,oVendor_company:  oVendor_company,oActivity_status:  oActivity_status,oActivity_date:  oActivity_date,Equipment_type: Equipment_type,Number_of_units: Number_of_units,Rent_rate: Rent_rate,Rent_rate_qunatity: Rent_rate_qunatity,Customer_company: Customer_company,Vendor_company: Vendor_company,Activity_status: Activity_status,Activity_date: Activity_date, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Equipment_Activity", "Admin");
                }
                else{
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
                return RedirectToAction("new_Equipment_Activity", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Equipment_Activity(string id, string oEquipment_type,string oNumber_of_units,string oRent_rate,string oRent_rate_qunatity,string oCustomer_company,string oVendor_company,string oActivity_status,string oActivity_date,string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment Activity' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
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



        [AllowAnonymous]
        public ActionResult view_Vendor_Company()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Vendor Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_Vendor_Company> response = null; 
           ActionResult d =  view_it_Vendor_Company(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if( Session["status"].ToString()=="You do not have access to this functionality" ){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_Vendor_Company>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Vendor_Company(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Vendor Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_Vendor_Company>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Vendor_Company>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Vendor_Company("");
            return  Content(JsonConvert.SerializeObject(  (List<Equipment_hub_Vendor_Company>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Vendor_Company(string id,string Company_name  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Vendor Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Company_name = Company_name;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Vendor_Company(string id,string oCompany_name,string Company_name )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Vendor Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Vendor_Company(id:id,oCompany_name:  oCompany_name,Company_name: Company_name, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Vendor_Company", "Admin");
                }
                else{
                      ViewBag.Company_name = Company_name;
                     
                     return View();
                }
                return RedirectToAction("new_Vendor_Company", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Vendor_Company(string id, string oCompany_name,string Company_name,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Vendor Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Vendor_Company(id:id,oCompany_name:  oCompany_name,Company_name: Company_name,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult view_Customer_Company()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Customer Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_Customer_Company> response = null; 
           ActionResult d =  view_it_Customer_Company(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if( Session["status"].ToString()=="You do not have access to this functionality" ){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_Customer_Company>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Customer_Company(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Customer Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_Customer_Company>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Customer_Company>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Customer_Company("");
            return  Content(JsonConvert.SerializeObject(  (List<Equipment_hub_Customer_Company>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Customer_Company(string id,string Company_name  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Customer Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
            ViewBag.id=id;
             ViewBag.Company_name = Company_name;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Customer_Company(string id,string oCompany_name,string Company_name )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Customer Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Customer_Company(id:id,oCompany_name:  oCompany_name,Company_name: Company_name, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Customer_Company", "Admin");
                }
                else{
                      ViewBag.Company_name = Company_name;
                     
                     return View();
                }
                return RedirectToAction("new_Customer_Company", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Customer_Company(string id, string oCompany_name,string Company_name,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Customer Company' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Customer_Company(id:id,oCompany_name:  oCompany_name,Company_name: Company_name,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult view_Vendor_Payments()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Vendor Payments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_Vendor_Payments> response = null; 
           ActionResult d =  view_it_Vendor_Payments(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if( Session["status"].ToString()=="You do not have access to this functionality" ){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_Vendor_Payments_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Vendor_Payments(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Vendor Payments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_Vendor_Payments>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Vendor_Payments>(); 
               Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Vendor_Payments("");
            return  Content(JsonConvert.SerializeObject(  (List<Equipment_hub_Vendor_Payments_data>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Vendor_Payments(string id,string Equipement_activity,string Amount,string Date_payment_was_due,string Date_payment_was_made  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Vendor Payments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_Equipment_Activity("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Equipement_activity = Equipement_activity;
             ViewBag.Amount = Amount;
             ViewBag.Date_payment_was_due = Date_payment_was_due;
             ViewBag.Date_payment_was_made = Date_payment_was_made;
            
            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Vendor_Payments(string id,string oEquipement_activity,string oAmount,string oDate_payment_was_due,string oDate_payment_was_made,string Equipement_activity,string Amount,string Date_payment_was_due,string Date_payment_was_made )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Vendor Payments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Vendor_Payments(id:id,oEquipement_activity:  oEquipement_activity,oAmount:  oAmount,oDate_payment_was_due:  oDate_payment_was_due,oDate_payment_was_made:  oDate_payment_was_made,Equipement_activity: Equipement_activity,Amount: Amount,Date_payment_was_due: Date_payment_was_due,Date_payment_was_made: Date_payment_was_made, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Vendor_Payments", "Admin");
                }
                else{
                      ViewBag.Equipement_activity = Equipement_activity;
                      ViewBag.Amount = Amount;
                      ViewBag.Date_payment_was_due = Date_payment_was_due;
                      ViewBag.Date_payment_was_made = Date_payment_was_made;
                     
                     return View();
                }
                return RedirectToAction("new_Vendor_Payments", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Vendor_Payments(string id, string oEquipement_activity,string oAmount,string oDate_payment_was_due,string oDate_payment_was_made,string Equipement_activity,string Amount,string Date_payment_was_due,string Date_payment_was_made,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Vendor Payments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_Vendor_Payments(id:id,oEquipement_activity:  oEquipement_activity,oAmount:  oAmount,oDate_payment_was_due:  oDate_payment_was_due,oDate_payment_was_made:  oDate_payment_was_made,Equipement_activity: Equipement_activity,Amount: Amount,Date_payment_was_due: Date_payment_was_due,Date_payment_was_made: Date_payment_was_made,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }



        [AllowAnonymous]
        public ActionResult view_Equipment()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_Equipment> response = null; 
           ActionResult d =  view_it_Equipment( Session["token"].ToString() , Session["role"].ToString()  ); 
                if( Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_Equipment_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Equipment(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_Equipment>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Equipment>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Equipment("");
            return  Content(JsonConvert.SerializeObject(   (List<Equipment_hub_Equipment_data>)Session["response"] ));
        }



        [AllowAnonymous]
        public ActionResult view_Equipment_Rent_Rate()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment Rent Rate' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_Rent_Rate> response = null; 
           ActionResult d =  view_it_Equipment_Rent_Rate( Session["token"].ToString() , Session["role"].ToString()  ); 
                if( Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_Rent_Rate_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Equipment_Rent_Rate(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Equipment Rent Rate' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_Rent_Rate>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_Rent_Rate>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_Rent_Rate("");
            return  Content(JsonConvert.SerializeObject(   (List<Equipment_hub_Rent_Rate_data>)Session["response"] ));
        }



        [AllowAnonymous]
        public ActionResult new_Administrators()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Admin("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Administrators(string First_name,string Last_name,string Email,string Role,string Password,string Password2 )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                
            if(Password == null)
            {
                 Password = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes( Audit.GetEncodedHash(Audit.GenerateRandom(), "doing it well")));;
            }
            if(Password2 == null)
            {
                 Password2 = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes( Audit.GetEncodedHash(Audit.GenerateRandom(), "doing it well")));;
            }
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
                ActionResult xx =  add_Administrators(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Administrators", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult add_Administrators(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'new Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.add_new_authenticate_Admin(First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Administrators()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            List<Equipment_hub_authenticate_Admin> response = null; 
           ActionResult d =  view_it_Administrators(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
            return View((List<Equipment_hub_authenticate_Admin_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Administrators(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'view Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"] = new List<Equipment_hub_authenticate_Admin>(); 
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["response"] = new List<Equipment_hub_authenticate_Admin>(); 
                Session["status"] = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            getStatus();
            Session["response"] = centralCalls.get_authenticate_Admin("");
            return Content(JsonConvert.SerializeObject( ((List<Equipment_hub_authenticate_Admin_data>)Session["response"]) ));
        }

        [AllowAnonymous]
        public ActionResult edit_Administrators(string id,string First_name,string Last_name,string Email,string Role,string Password,string Password2  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
            ViewBag.Data0 =  centralCalls.get_role_Admin("");
            getStatus();
            ViewBag.id=id;
             ViewBag.First_name = First_name;
 ViewBag.Last_name = Last_name;
 ViewBag.Email = Email;
 ViewBag.Role = Role;
 ViewBag.Password = Password;
 ViewBag.Password2 = Password2;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Administrators(string id,string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string First_name,string Last_name,string Email,string Role,string Password,string Password2 )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Admin");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Admin");
            }
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Admin");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Admin");
                }
            string response =null;
                ActionResult xx =  update_Administrators(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Admin");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Administrators", "Admin");
                }
                else{
                     return View();
                }
                return RedirectToAction("new_Administrators", "Admin");
        } 

        [AllowAnonymous]
        public ActionResult update_Administrators(string id, string oFirst_name,string oLast_name,string oEmail,string oRole,string oPassword,string oPassword2,string First_name,string Last_name,string Email,string Role,string Password,string Password2,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Admin("  where role =  "  + Session["role"]   + " and  `right` = (select id from equipment_hub_right_admin where  replace(rightName,'_',' ') = 'edit Administrators' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               Session["response"]  = "You do not have access to this functionality";
               return Content(Session["status"].ToString() );
            }
            if (!validateAccessToken(token)) 
            { 
                Session["status"] = "Invalid Token";
                Session["response"]  = "Invalid Token";
                return Content("Invalid Token"); 
            } 
            string response = null;  
            response =  centralCalls.update_authenticate_Admin(id:id,oFirst_name:  oFirst_name,oLast_name:  oLast_name,oEmail:  oEmail,oRole:  oRole,oPassword:  oPassword,oPassword2:  oPassword2,First_name: First_name,Last_name: Last_name,Email: Email,Role: Role,Password: Password,Password2: Password2,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }


   
    }
} 
