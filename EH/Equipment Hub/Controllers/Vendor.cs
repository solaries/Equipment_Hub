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
    public class VENDORController : Controller
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
        public ActionResult login(string First_name,string Last_name,string Email,string Role,string Password,string Password2,string Activated,string Company_name,   string forgot)
        {  
            Audit.protocol();
            string token = doAuthenticate(userName: Email, password: Password, clientID: "Vendor");
            bool result = validateAccessToken(token);
            List<Equipment_hub_authenticate_Vendor_data> response =null;
            if (String.IsNullOrEmpty(forgot))
            {
                ActionResult xx = authenticate( password:  Password, email: Email ); 
                response = ( List<Equipment_hub_authenticate_Vendor_data>)Session["response"];
                if(response !=null ){
                    if(response.Count > 0){
                         if (Encoding.ASCII.GetString(response[0].Password) == Encoding.ASCII.GetString(response[0].Password2))
                         { 
                            Session["userID"] = response[0].Id ;
                            Session["userType"] = "Vendor" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Activated"] =  response[0].Activated;
                            Session["Vendor_company"] =  response[0].Company_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            List<Equipment_hub_right_Vendor> rightList = centralCalls.get_right_Vendor(" where id in (select `right` from Equipment_hub_role_right_Vendor where role =" + response[0].Role.ToString() + " )   order by rightname");
                            string strRightList = "";
                            foreach (Equipment_hub_right_Vendor right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList; 
                            Session["role"] = response[0].Role;
                            Session["status"] = "Please change your password";
                            return RedirectToAction("Change_Password", "Vendor");
                        }
                        else  
                        {  
                            Session["userID"] = response[0].Id ;
                            Session["status"] = "";
                            Session["userType"] = "Vendor" ;
                            Session["email"] = response[0].Email;
                            Session["first_name"] = response[0].First_name;
                            Session["last_name"] = response[0].Last_name;
                            Session["Activated"] =  response[0].Activated;
                            Session["Vendor_company"] =  response[0].Company_name;
                            Session["token"] = token;
                            Session["Password"] = Password;
                            Session["role"] = response[0].Role;
                            List<Equipment_hub_right_Vendor> rightList = centralCalls.get_right_Vendor(" where id in (select `right` from Equipment_hub_role_right_Vendor where role =" + response[0].Role.ToString() + " )   order by rightname");
                            string strRightList = "";
                            foreach (Equipment_hub_right_Vendor right in rightList)
                            {
                                strRightList += "sphinxcol" + right.Rightname + "sphinxcol";
                            }
                            Session["strRightList"] = strRightList;
                            return RedirectToAction("view_Equipment_Activity", "Vendor"); 
                        }
                    }
                    else
                    {
                        Session["status"] = "Authentication Not successful";
                        return RedirectToAction("Login", "Vendor");
                    } 
                }
                else
                {
                    Session["status"] = "Authentication Not successful";
                    return RedirectToAction("Login", "Vendor");
                } 
            }
            else { 
                ActionResult xx = forgotauthenticate_Vendor(   Email: Email ); 
                response = ( List<Equipment_hub_authenticate_Vendor_data>)Session["response"]; 
                return RedirectToAction("Login", "Vendor");
            }
        } 

        [AllowAnonymous]
        public ActionResult authenticate(string email, string password )
        { 
            Audit.protocol(); 
            List<Equipment_hub_authenticate_Vendor_data> response = null;  
            password =  Audit.GetEncodedHash(password, "doing it well") ;
            response =  centralCalls.get_authenticate_Vendor(" where replace(password, '@','#')  = '" + password.Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            Session["response"]  = response;
            return Content(JsonConvert.SerializeObject((List<Equipment_hub_authenticate_Vendor_data>)response));
        }

        [AllowAnonymous]
        public ActionResult forgotauthenticate_Vendor(string Email )
        {   
            Audit.protocol();
            List<Equipment_hub_authenticate_Vendor_data> response = null; 
            response =  centralCalls.get_authenticate_Vendor(" where replace(email, '@','#') = '" + Email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = Audit.GenerateRandom();
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        centralCalls.update_authenticate_Vendor(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oActivated: response[0].Activated.ToString() ,oCompany_name: response[0].Company_name.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString(arr),Activated: response[0].Activated.ToString() ,Company_name: response[0].Company_name.ToString() ) ; 
                        string mailSubject = "Profile password reset on (code joh) Equipment HUB";
                        Session["status"] = "Password reset successful, please check your email for your new password.";
                        string mailBody = "Hi <br><br>Your password has been successfully reset on the (code joh) Equipment HUB platform. Please log in with following credentials: <br><br> Email:" + response[0].Email + "<br><br>password :" + strRND + "<br><br><br>Regards<br><br>";
                        Audit.SendMail(Email, mailSubject, mailBody, " "); 
                } 
            } 
            Session["response"]  = response; 
             return Content(JsonConvert.SerializeObject((List<Equipment_hub_authenticate_Vendor_data>)response)); ;
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
        public ActionResult Register(string First_name, string Last_name, string Email, string Activated, string Company_name_Company_name)
        {
            Audit.protocol();
            string response = null;
            ActionResult xx = addRegister(First_name: First_name, Last_name: Last_name, Email: Email, Activated: Activated, Company_name_Company_name: Company_name_Company_name);
            response = (string)Session["response"];
            Session["status"] = response;
            if (response.IndexOf("uccessf") == -1)
            {
                return RedirectToAction("Register", "Vendor");
            }
            else
            {
                return RedirectToAction("Login", "Vendor");
            }
        }

        [AllowAnonymous]
        public ActionResult addRegister(string First_name, string Last_name, string Email, string Activated, string Company_name_Company_name)
        {
            Audit.protocol();
            string response = "";
            string strRND = Audit.GenerateRandom();
            byte[] arr = Encoding.ASCII.GetBytes(Audit.GetEncodedHash(strRND, "doing it well"));

            string Company_name = centralCalls.add_new_Vendor_Company(Company_name: Company_name_Company_name, returnID: true);
            List<Equipment_hub_right_Vendor> r = centralCalls.get_right_Vendor("   order by rightname");
            string selectedRights = "";
            string delim = "";
            for (int i = 0; i < r.Count; i++)
            {
                selectedRights += delim + r[i].Id;
                delim = "sphinxcol";
            }
            string Role = centralCalls.add_new_role_Vendor(Rolename: "Vendor", selectedRights: selectedRights, returnID: true, Company: Company_name);
            response = centralCalls.add_new_authenticate_Vendor(First_name: First_name, Last_name: Last_name, Email: Email, Role: Role, Password: Encoding.ASCII.GetString(arr), Password2: Encoding.ASCII.GetString(arr), Activated: Activated, Company_name: Company_name);
            if (response.IndexOf("uccessf") > -1)
            {
                string mailSubject = "Vendor Profile creation on (code joh) Equipment HUB";
                string mailBody = "Hi <br><br>You have been successfully profiled on the (code joh) Equipment HUB platform. Please log in with following credentials: <br><br> Email: " + Email + "<br><br>password : " + strRND + "<br><br><br>Regards<br><br>";
                Audit.SendMail(Email, mailSubject, mailBody, " ");
            }
            Session["response"] = response;
            return Content(response);
        }



        [AllowAnonymous]
        public ActionResult Change_Password()
        {
                Audit.protocol();
            getStatus(false);
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
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
                    return RedirectToAction("Login", "Vendor") ;
                }
                else
                {
                    return RedirectToAction("Change_Password", "Vendor") ;
                }
        } 


        [AllowAnonymous]
        public ActionResult updatePassword(string email,string password,string npassword )
        {   
            Audit.protocol();
            List<Equipment_hub_authenticate_Vendor_data> response = null; 
            string result = "Authentication failed"; 
            string strRND11 = password;
            byte[] arr11 = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND11, "doing it well")); 
            response =  centralCalls.get_authenticate_Vendor(" where replace(password, '@','#')  = '" + Encoding.ASCII.GetString(arr11).Replace("@", "#") + "' and replace(email, '@','#') = '" + email.Replace("@", "#") + "' ");
            if(response !=null ){
                if(response.Count > 0){
                        string strRND = npassword;
                        byte[] arr = Encoding.ASCII.GetBytes( Audit.GetEncodedHash(strRND, "doing it well")); 
                        result =  centralCalls.update_authenticate_Vendor(  id:  response[0].Id.ToString(),    oFirst_name: response[0].First_name.ToString() ,oLast_name: response[0].Last_name.ToString() ,oEmail: response[0].Email.ToString() ,oRole: response[0].Role.ToString() ,oPassword: Encoding.ASCII.GetString( response[0].Password)  ,oPassword2: Encoding.ASCII.GetString( response[0].Password2)  ,oActivated: response[0].Activated.ToString() ,oCompany_name: response[0].Company_name.ToString()  , First_name: response[0].First_name.ToString() ,Last_name: response[0].Last_name.ToString() ,Email: response[0].Email.ToString() ,Role: response[0].Role.ToString() ,Password: Encoding.ASCII.GetString(arr),Password2: Encoding.ASCII.GetString( response[0].Password2),Activated: response[0].Activated.ToString() ,Company_name: response[0].Company_name.ToString() ) ; 
                } 
            } 
            Session["response"]  = result; 
            return Content((string)result);
        }




        [AllowAnonymous]
        public ActionResult new_Roles()
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Roles' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data0 = centralCalls.get_right_Vendor("   order by rightname");
            ViewBag.Data1 = centralCalls.get_Vendor_Company("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Roles(string Rolename, string Company, string selectedRights)
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Roles' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }

            string response = null;
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            ActionResult xx = add_Roles(Rolename: Rolename, Company: Company, token: Session["token"].ToString(), role: Session["role"].ToString(), selectedRights: selectedRights);
            if (((System.Web.Mvc.ContentResult)(xx)).Content == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            response = (string)Session["response"];
            Session["status"] = response;
            return RedirectToAction("new_Roles", "Vendor");
        }

        [AllowAnonymous]
        public ActionResult add_Roles(string Rolename, string Company, string token, string role, string selectedRights)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Roles' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = "You do not have access to this functionality";
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["status"] = "Invalid Token";
                Session["response"] = "Invalid Token";
                return Content("Invalid Token");
            }
            string response = null;
            response = centralCalls.add_new_role_Vendor(Rolename: Rolename, Company: Company, selectedRights: selectedRights);
            Session["response"] = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Roles()
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Roles' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            getStatus();
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            List<Equipment_hub_role_Vendor> response = null;
            ActionResult d = view_it_Roles(Session["token"].ToString(), Session["role"].ToString());
            if (Session["status"].ToString() == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            return View((List<Equipment_hub_role_Vendor_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Roles(string token, string role)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Roles' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = new List<Equipment_hub_role_Vendor>();
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["response"] = new List<Equipment_hub_role_Vendor>();
                Session["status"] = "Invalid Token";
                return Content("Invalid Token");
            }
            getStatus();
            Session["response"] = centralCalls.get_role_Vendor(" where a.company = " + Session["Vendor_company"]);
            return Content(JsonConvert.SerializeObject(((List<Equipment_hub_role_Vendor_data>)Session["response"])));
        }

        [AllowAnonymous]
        public ActionResult edit_Roles(string id, string Rolename, string Company)
        {
            Audit.protocol();
            ViewBag.Data0 = centralCalls.get_right_Vendor("   order by rightname");
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data1 = centralCalls.get_Vendor_Company("");
            getStatus();
            ViewBag.id = id;
            ViewBag.Rolename = Rolename;
            ViewBag.Company = Company;

            List<Equipment_hub_role_right_Vendor_data> roleRightVendorList = centralCalls.get_role_right_Vendor(" where role = " + id);
            string rightSet = "";
            foreach (Equipment_hub_role_right_Vendor_data roleRightVendor in roleRightVendorList)
            {
                rightSet += "sphinxcol" + roleRightVendor.Right + "sphinxcol";
            }
            ViewBag.rightSet = rightSet;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Roles(string id, string oRolename, string oCompany, string Rolename, string Company, string selectedRights)
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            string response = null;
            ActionResult xx = update_Roles(id: id, oRolename: oRolename, oCompany: oCompany, Rolename: Rolename, Company: Company, token: Session["token"].ToString(), role: Session["role"].ToString(), selectedRights: selectedRights);
            if (((System.Web.Mvc.ContentResult)(xx)).Content == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            response = (string)Session["response"];
            Session["status"] = response;
            if (response.IndexOf("uccess") > -1)
            {
                return RedirectToAction("view_Roles", "Vendor");
            }
            else
            {
                ViewBag.id = id;
                ViewBag.Rolename = Rolename;
                ViewBag.Company = Company;

                List<Equipment_hub_role_right_Vendor_data> roleRightVendorList = centralCalls.get_role_right_Vendor(" where role = " + id);
                string rightSet = "";
                foreach (Equipment_hub_role_right_Vendor_data roleRightVendor in roleRightVendorList)
                {
                    rightSet += "sphinxcol" + roleRightVendor.Right + "sphinxcol";
                }
                ViewBag.rightSet = rightSet;
                return View();
            }
            return RedirectToAction("new_Roles", "Vendor");
        }

        [AllowAnonymous]
        public ActionResult update_Roles(string id, string oRolename, string oCompany, string Rolename, string Company, string token, string role, string selectedRights)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Roles' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = "You do not have access to this functionality";
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["status"] = "Invalid Token";
                Session["response"] = "Invalid Token";
                return Content("Invalid Token");
            }
            string response = null;
            response = centralCalls.update_role_Vendor(id: id, oRolename: oRolename, oCompany: oCompany, Rolename: Rolename, Company: Company, andPassword: false, selectedRights: selectedRights);
            Session["response"] = response;
            return Content((string)response);
        }





        [AllowAnonymous]
        public ActionResult view_Payments()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Payments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
                }
            List<Equipment_hub_Vendor_Payments> response = null; 
           ActionResult d =  view_it_Payments( Session["token"].ToString() , Session["role"].ToString()  ); 
                if( Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Vendor");
                }
            return View((List<Equipment_hub_Vendor_Payments_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Payments(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Payments' )   ").Count ==0){ 
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
            return  Content(JsonConvert.SerializeObject(   (List<Equipment_hub_Vendor_Payments_data>)Session["response"] ));
        }



        [AllowAnonymous]
        public ActionResult new_Equipments()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Equipments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data0 =  centralCalls.get_Equipment_Type("");
            ViewBag.Data1 =  centralCalls.get_authenticate_Vendor("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Equipments(string Equipment_type,string Equipment_code,string Vendor )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Equipments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
                
                string response =null;
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
                }
                ActionResult xx =  add_Equipments(Equipment_type: Equipment_type,Equipment_code: Equipment_code,Vendor: Vendor,token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Vendor");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                return RedirectToAction("new_Equipments", "Vendor");
        } 

        [AllowAnonymous]
        public ActionResult add_Equipments(string Equipment_type,string Equipment_code,string Vendor,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Equipments' )   ").Count ==0){ 
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
            response =  centralCalls.add_new_Equipment(Equipment_type: Equipment_type,Equipment_code: Equipment_code,Vendor: Vendor);
            Session["response"]  = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Equipments()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Equipments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
                }
            List<Equipment_hub_Equipment> response = null; 
           ActionResult d =  view_it_Equipments(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if(Session["status"].ToString()=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Vendor");
                }
            return View((List<Equipment_hub_Equipment_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Equipments(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Equipments' )   ").Count ==0){ 
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
            Session["response"] = centralCalls.get_Equipment(" where vendor = " + Session["Vendor_company"]);
            return Content(JsonConvert.SerializeObject( ((List<Equipment_hub_Equipment_data>)Session["response"]) ));
        }




        [AllowAnonymous]
        public ActionResult new_Equipment_Rent_Rates()
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Equipment Rent Rates' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data0 = centralCalls.get_Equipment_Type("");
            ViewBag.Data1 = centralCalls.get_Duration_Type("");
            ViewBag.Data2 = centralCalls.get_authenticate_Vendor("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Equipment_Rent_Rates(string Equipement_type, string Duration_type, string Qunatity, string Price, string Entry_date, string Vendor)
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Equipment Rent Rates' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }

            string response = null;
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            ActionResult xx = add_Equipment_Rent_Rates(Equipement_type: Equipement_type, Duration_type: Duration_type, Qunatity: Qunatity, Price: Price, Entry_date: Entry_date, Vendor: Vendor, token: Session["token"].ToString(), role: Session["role"].ToString());
            if (((System.Web.Mvc.ContentResult)(xx)).Content == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            response = (string)Session["response"];
            Session["status"] = response;
            return RedirectToAction("new_Equipment_Rent_Rates", "Vendor");
        }

        [AllowAnonymous]
        public ActionResult add_Equipment_Rent_Rates(string Equipement_type, string Duration_type, string Qunatity, string Price, string Entry_date, string Vendor, string token, string role)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Equipment Rent Rates' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = "You do not have access to this functionality";
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["status"] = "Invalid Token";
                Session["response"] = "Invalid Token";
                return Content("Invalid Token");
            }
            string response = null;
            response = centralCalls.add_new_Rent_Rate(Equipement_type: Equipement_type, Duration_type: Duration_type, Qunatity: Qunatity, Price: Price, Entry_date: Entry_date, Vendor: Vendor);
            Session["response"] = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Equipment_Rent_Rates()
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Equipment Rent Rates' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            getStatus();
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            List<Equipment_hub_Rent_Rate> response = null;
            ActionResult d = view_it_Equipment_Rent_Rates(Session["token"].ToString(), Session["role"].ToString());
            if (Session["status"].ToString() == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            return View((List<Equipment_hub_Rent_Rate_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Equipment_Rent_Rates(string token, string role)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Equipment Rent Rates' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = new List<Equipment_hub_Rent_Rate>();
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["response"] = new List<Equipment_hub_Rent_Rate>();
                Session["status"] = "Invalid Token";
                return Content("Invalid Token");
            }
            getStatus();
            Session["response"] = centralCalls.get_Rent_Rate(" where vendor = " + Session["Vendor_company"], true);
            return Content(JsonConvert.SerializeObject(((List<Equipment_hub_Rent_Rate_data>)Session["response"])));
        }

        [AllowAnonymous]
        public ActionResult edit_Equipment_Rent_Rates(string id, string Equipement_type, string Duration_type, string Qunatity, string Price, string Entry_date, string Vendor)
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Equipment Rent Rates' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data0 = centralCalls.get_Equipment_Type("");
            ViewBag.Data1 = centralCalls.get_Duration_Type("");
            ViewBag.Data2 = centralCalls.get_authenticate_Vendor("");
            getStatus();
            ViewBag.id = id;
            ViewBag.Equipement_type = Equipement_type;
            ViewBag.Duration_type = Duration_type;
            ViewBag.Qunatity = Qunatity;
            ViewBag.Price = Price;
            ViewBag.Entry_date = Entry_date;
            ViewBag.Vendor = Vendor;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Equipment_Rent_Rates(string id, string oEquipement_type, string oDuration_type, string oQunatity, string oPrice, string oEntry_date, string oVendor, string Equipement_type, string Duration_type, string Qunatity, string Price, string Entry_date, string Vendor)
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Equipment Rent Rates' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            string response = null;

            ActionResult xx = add_Equipment_Rent_Rates(Equipement_type: Equipement_type, Duration_type: Duration_type, Qunatity: Qunatity, Price: Price, Entry_date: Entry_date, Vendor: Vendor, token: Session["token"].ToString(), role: Session["role"].ToString());

            //ActionResult xx = update_Equipment_Rent_Rates(id: id, oEquipement_type: oEquipement_type, oDuration_type: oDuration_type, oQunatity: oQunatity, oPrice: oPrice, oEntry_date: oEntry_date, oVendor: oVendor, Equipement_type: Equipement_type, Duration_type: Duration_type, Qunatity: Qunatity, Price: Price, Entry_date: Entry_date, Vendor: Vendor, token: Session["token"].ToString(), role: Session["role"].ToString());
            if (((System.Web.Mvc.ContentResult)(xx)).Content == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            response = (string)Session["response"];
            Session["status"] = response;
            if (response.IndexOf("uccess") > -1)
            {
                return RedirectToAction("view_Equipment_Rent_Rates", "Vendor");
            }
            else
            {
                return View();
            }
            return RedirectToAction("new_Equipment_Rent_Rates", "Vendor");
        }

        [AllowAnonymous]
        public ActionResult update_Equipment_Rent_Rates(string id, string oEquipement_type, string oDuration_type, string oQunatity, string oPrice, string oEntry_date, string oVendor, string Equipement_type, string Duration_type, string Qunatity, string Price, string Entry_date, string Vendor, string token, string role)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Equipment Rent Rates' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = "You do not have access to this functionality";
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["status"] = "Invalid Token";
                Session["response"] = "Invalid Token";
                return Content("Invalid Token");
            }
            string response = null;
            response = centralCalls.update_Rent_Rate(id: id, oEquipement_type: oEquipement_type, oDuration_type: oDuration_type, oQunatity: oQunatity, oPrice: oPrice, oEntry_date: oEntry_date, oVendor: oVendor, Equipement_type: Equipement_type, Duration_type: Duration_type, Qunatity: Qunatity, Price: Price, Entry_date: Entry_date, Vendor: Vendor, andPassword: false);
            Session["response"] = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult edit_Equipments(string id,string Equipment_type,string Equipment_code,string Vendor  )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Equipments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data0 =  centralCalls.get_Equipment_Type("");
            ViewBag.Data1 =  centralCalls.get_authenticate_Vendor("");
            getStatus();
            ViewBag.id=id;
             ViewBag.Equipment_type = Equipment_type;
 ViewBag.Equipment_code = Equipment_code;
 ViewBag.Vendor = Vendor;

            return View();
        } 

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Equipments(string id,string oEquipment_type,string oEquipment_code,string oVendor,string Equipment_type,string Equipment_code,string Vendor )
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Equipments' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
                }
            string response =null;
                ActionResult xx =  update_Equipments(id:id,oEquipment_type:  oEquipment_type,oEquipment_code:  oEquipment_code,oVendor:  oVendor,Equipment_type: Equipment_type,Equipment_code: Equipment_code,Vendor: Vendor, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Vendor");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Equipments", "Vendor");
                }
                else{
                     return View();
                }
                return RedirectToAction("new_Equipments", "Vendor");
        } 

        [AllowAnonymous]
        public ActionResult update_Equipments(string id, string oEquipment_type,string oEquipment_code,string oVendor,string Equipment_type,string Equipment_code,string Vendor,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Equipments' )   ").Count ==0){ 
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
            response =  centralCalls.update_Equipment(id:id,oEquipment_type:  oEquipment_type,oEquipment_code:  oEquipment_code,oVendor:  oVendor,Equipment_type: Equipment_type,Equipment_code: Equipment_code,Vendor: Vendor,andPassword: false);
            Session["response"]  = response;
            return Content((string)response);
        }


             


        [AllowAnonymous]
        public ActionResult view_Equipment_Activity()
        {
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Equipment Activity' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
            getStatus();
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
                }
            List<Equipment_hub_Equipment_Activity> response = null; 
           ActionResult d =  view_it_Equipment_Activity(Session["token"].ToString()  , Session["role"].ToString()  ); 
                if( Session["status"].ToString()=="You do not have access to this functionality" ){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Vendor");
                }
            return View((List<Equipment_hub_Equipment_Activity_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Equipment_Activity(string token, string role)
        {
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Equipment Activity' )   ").Count ==0){ 
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
            Session["response"] = centralCalls.get_Equipment_Activity(" where a.Vendor_Company = " + Session["Vendor_company"]);
            return  Content(JsonConvert.SerializeObject(  (List<Equipment_hub_Equipment_Activity_data>)Session["response"] ));
        }

        [AllowAnonymous]
        public ActionResult edit_Equipment_Activity(string id, string Equipment_type, string Number_of_units, string Rent_rate, string Rent_rate_qunatity, string Customer_company, string Vendor_company, string Activity_status, string Activity_date, string Rent_rate_data)
        {  
                Audit.protocol();
            if(Session["userType"] == null){ 
               Session["status"] = "Session Timed out";
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Equipment Activity' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data0 =  centralCalls.get_Equipment_Type("");
            ViewBag.Data1 =  centralCalls.get_Rent_Rate("");
            ViewBag.Data2 = centralCalls.get_Vendor_Company("");
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
             ViewBag.Rent_rate_data = Rent_rate_data;
            
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
               return RedirectToAction("Login", "Vendor");
            }
            if(Session["status"].ToString() == "Please change your password")
            {
               return RedirectToAction("Change_Password", "Vendor");
            }
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Equipment Activity' )   ").Count ==0){ 
               Session["status"] = "You do not have access to this functionality";
               return RedirectToAction("Login", "Vendor");
            }
                if (!validateAccessToken(Session["token"].ToString()))
                {
                    Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
                }
            string response =null;
                ActionResult xx =  update_Equipment_Activity(id:id,oEquipment_type:  oEquipment_type,oNumber_of_units:  oNumber_of_units,oRent_rate:  oRent_rate,oRent_rate_qunatity:  oRent_rate_qunatity,oCustomer_company:  oCustomer_company,oVendor_company:  oVendor_company,oActivity_status:  oActivity_status,oActivity_date:  oActivity_date,Equipment_type: Equipment_type,Number_of_units: Number_of_units,Rent_rate: Rent_rate,Rent_rate_qunatity: Rent_rate_qunatity,Customer_company: Customer_company,Vendor_company: Vendor_company,Activity_status: Activity_status,Activity_date: Activity_date, token: Session["token"].ToString() ,role: Session["role"].ToString()  ); 
                if( ((System.Web.Mvc.ContentResult)(xx)).Content=="You do not have access to this functionality"){ 
                    Session["status"] = "You do not have access to this functionality";
                    return RedirectToAction("Login", "Vendor");
                }
                response = (string)Session["response"];
                Session["status"] = response;
                if(response.IndexOf("uccess") > -1){
                    return RedirectToAction("view_Equipment_Activity", "Vendor");
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
                return RedirectToAction("new_Equipment_Activity", "Vendor");
        } 

        [AllowAnonymous]
        public ActionResult update_Equipment_Activity(string id, string oEquipment_type,string oNumber_of_units,string oRent_rate,string oRent_rate_qunatity,string oCustomer_company,string oVendor_company,string oActivity_status,string oActivity_date,string Equipment_type,string Number_of_units,string Rent_rate,string Rent_rate_qunatity,string Customer_company,string Vendor_company,string Activity_status,string Activity_date,string token, string role)
        { 
                Audit.protocol();
                Session["status"] = "";
            if(centralCalls.get_role_right_Vendor("  where role =  "  + Session["role"]   + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Equipment Activity' )   ").Count ==0){ 
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
        public ActionResult new_Users()
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Users' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data0 = centralCalls.get_role_Vendor("");
            getStatus();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult new_Users(string First_name, string Last_name, string Email, string Role, string Password, string Password2, string Activated, string Company_name)
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Users' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }

            if (Password == null)
            {
                Password = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(Audit.GenerateRandom(), "doing it well"))); ;
            }
            if (Password2 == null)
            {
                Password2 = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(Audit.GetEncodedHash(Audit.GenerateRandom(), "doing it well"))); ;
            }
            string response = null;
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            ActionResult xx = add_Users(First_name: First_name, Last_name: Last_name, Email: Email, Role: Role, Password: Password, Password2: Password2, Activated: Activated, Company_name: Company_name, token: Session["token"].ToString(), role: Session["role"].ToString());
            if (((System.Web.Mvc.ContentResult)(xx)).Content == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            response = (string)Session["response"];
            Session["status"] = response;
            return RedirectToAction("new_Users", "Vendor");
        }

        [AllowAnonymous]
        public ActionResult add_Users(string First_name, string Last_name, string Email, string Role, string Password, string Password2, string Activated, string Company_name, string token, string role)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'new Users' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = "You do not have access to this functionality";
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["status"] = "Invalid Token";
                Session["response"] = "Invalid Token";
                return Content("Invalid Token");
            }
            string response = null;
            response = centralCalls.add_new_authenticate_Vendor(First_name: First_name, Last_name: Last_name, Email: Email, Role: Role, Password: Password, Password2: Password2, Activated: Activated, Company_name: Company_name);
            Session["response"] = response;
            return Content((string)response);
        }

        [AllowAnonymous]
        public ActionResult view_Users()
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Users' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            getStatus();
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            List<Equipment_hub_authenticate_Vendor> response = null;
            ActionResult d = view_it_Users(Session["token"].ToString(), Session["role"].ToString());
            if (Session["status"].ToString() == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            return View((List<Equipment_hub_authenticate_Vendor_data>)Session["response"]);
        }

        [AllowAnonymous]
        public ActionResult view_it_Users(string token, string role)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'view Users' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = new List<Equipment_hub_authenticate_Vendor>();
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["response"] = new List<Equipment_hub_authenticate_Vendor>();
                Session["status"] = "Invalid Token";
                return Content("Invalid Token");
            }
            getStatus();
            Session["response"] = centralCalls.get_authenticate_Vendor("");
            return Content(JsonConvert.SerializeObject(((List<Equipment_hub_authenticate_Vendor_data>)Session["response"])));
        }

        [AllowAnonymous]
        public ActionResult edit_Users(string id, string First_name, string Last_name, string Email, string Role, string Password, string Password2, string Activated, string Company_name)
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Users' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            ViewBag.Data0 = centralCalls.get_role_Vendor("");
            getStatus();
            ViewBag.id = id;
            ViewBag.First_name = First_name;
            ViewBag.Last_name = Last_name;
            ViewBag.Email = Email;
            ViewBag.Role = Role;
            ViewBag.Password = Password;
            ViewBag.Password2 = Password2;
            ViewBag.Activated = Activated;
            ViewBag.Company_name = Company_name;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult edit_Users(string id, string oFirst_name, string oLast_name, string oEmail, string oRole, string oPassword, string oPassword2, string oActivated, string oCompany_name, string First_name, string Last_name, string Email, string Role, string Password, string Password2, string Activated, string Company_name)
        {
            Audit.protocol();
            if (Session["userType"] == null)
            {
                Session["status"] = "Session Timed out";
                return RedirectToAction("Login", "Vendor");
            }
            if (Session["status"].ToString() == "Please change your password")
            {
                return RedirectToAction("Change_Password", "Vendor");
            }
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Users' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            if (!validateAccessToken(Session["token"].ToString()))
            {
                Session["token"] = doAuthenticate(userName: Session["email"].ToString(), password: Session["Password"].ToString(), clientID: "Vendor");
            }
            string response = null;
            ActionResult xx = update_Users(id: id, oFirst_name: oFirst_name, oLast_name: oLast_name, oEmail: oEmail, oRole: oRole, oPassword: oPassword, oPassword2: oPassword2, oActivated: oActivated, oCompany_name: oCompany_name, First_name: First_name, Last_name: Last_name, Email: Email, Role: Role, Password: Password, Password2: Password2, Activated: Activated, Company_name: Company_name, token: Session["token"].ToString(), role: Session["role"].ToString());
            if (((System.Web.Mvc.ContentResult)(xx)).Content == "You do not have access to this functionality")
            {
                Session["status"] = "You do not have access to this functionality";
                return RedirectToAction("Login", "Vendor");
            }
            response = (string)Session["response"];
            Session["status"] = response;
            if (response.IndexOf("uccess") > -1)
            {
                return RedirectToAction("view_Users", "Vendor");
            }
            else
            {
                return View();
            }
            return RedirectToAction("new_Users", "Vendor");
        }

        [AllowAnonymous]
        public ActionResult update_Users(string id, string oFirst_name, string oLast_name, string oEmail, string oRole, string oPassword, string oPassword2, string oActivated, string oCompany_name, string First_name, string Last_name, string Email, string Role, string Password, string Password2, string Activated, string Company_name, string token, string role)
        {
            Audit.protocol();
            Session["status"] = "";
            if (centralCalls.get_role_right_Vendor("  where role =  " + Session["role"] + " and  `right` = (select id from Equipment_hub_right_Vendor where  replace(rightName,'_',' ') = 'edit Users' )   ").Count == 0)
            {
                Session["status"] = "You do not have access to this functionality";
                Session["response"] = "You do not have access to this functionality";
                return Content(Session["status"].ToString());
            }
            if (!validateAccessToken(token))
            {
                Session["status"] = "Invalid Token";
                Session["response"] = "Invalid Token";
                return Content("Invalid Token");
            }
            string response = null;
            response = centralCalls.update_authenticate_Vendor(id: id, oFirst_name: oFirst_name, oLast_name: oLast_name, oEmail: oEmail, oRole: oRole, oPassword: oPassword, oPassword2: oPassword2, oActivated: oActivated, oCompany_name: oCompany_name, First_name: First_name, Last_name: Last_name, Email: Email, Role: Role, Password: Password, Password2: Password2, Activated: Activated, Company_name: Company_name, andPassword: false);
            Session["response"] = response;
            return Content((string)response);
        }

   
    }
} 
