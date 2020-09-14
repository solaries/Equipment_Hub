using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using Equipment_hub.Models;
using Equipment_hub.BusinessLogic;
using Equipment_hub.Data.Models;
    
namespace Equipment_hub
{
    public class AuthorizationProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.OwinContext.Set<string>("userType", context.Parameters["clientid"]);
            context.Validated();
        } 
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType); 
            bool found = false;  
            if (context.OwinContext.Get<string>("userType") == "Admin")
            {
                List<Equipment_hub_authenticate_Admin_data> response = centralCalls.get_authenticate_Admin(" where replace(password, '@','#')  = '" + Audit.GetEncodedHash(context.Password, "doing it well").Replace("@", "#") + "' and replace(email, '@','#') = '" + context.UserName.Replace("@", "#") + "' ");
                found = response.Count > 0;
            }

            if (context.OwinContext.Get<string>("userType") == "Customer")
            {
                List<Equipment_hub_authenticate_Customer_data> response = centralCalls.get_authenticate_Customer(" where replace(password, '@','#')  = '" + Audit.GetEncodedHash(context.Password, "doing it well").Replace("@", "#") + "' and replace(email, '@','#') = '" + context.UserName.Replace("@", "#") + "' ");
                found = response.Count > 0;
            }

            if (context.OwinContext.Get<string>("userType") == "Vendor")
            {
                List<Equipment_hub_authenticate_Vendor_data> response = centralCalls.get_authenticate_Vendor(" where replace(password, '@','#')  = '" + Audit.GetEncodedHash(context.Password, "doing it well").Replace("@", "#") + "' and replace(email, '@','#') = '" + context.UserName.Replace("@", "#") + "' ");
                found = response.Count > 0;
            }

            if (found)
            { 
                identity.AddClaim(new Claim("username", ".")); 
                context.Validated(identity);
            }
            else
            { 
                context.SetError("invalid_grant", "Provided username and password invalid");
                return;
            } 
        }
    }
}
