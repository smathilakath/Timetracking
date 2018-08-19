/*
 * Author : Sumod Madhavan
 * Date : 4/9/2015
 * **/
using System;
using System.Configuration;
using System.Web.Mvc;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using TimeTracking.Models;
using TimeTracking.Models.Service;
using TimeTracking.Models.Repository;

namespace TimeTracking.Controllers
{
    /// <summary>
    /// An existing Customer when clicks Connect to QuickBooks button, this controller will initiate Autorization flow.
    /// 
    /// </summary>
    public class OauthController : BaseController
    {
        /// <summary>
        /// CosumerSecret, ConsumerKey, OAuthLink, RequestToken, TokenSecret, OAuthCallbackUrl
        /// </summary>

        OAuthorizationdto oAuthorizationdto = null;
        OAuthTokens oAuthorizationDB = null;
        OAuthService oAuthService = null;
        /// <summary>
        /// Action Result for Index, This flow will create OAuthConsumer Context using Consumer key and Consuler Secret key
        /// obtained when Application is added at intuit workspace. It creates OAuth Session out of OAuthConsumer and Calls 
        /// Intuit Workpsace endpoint for OAuth.
        /// </summary>
        /// <returns>Redirect Result.</returns>
        public RedirectResult Index()
        {
            oAuthorizationdto = new OAuthorizationdto();
            oAuthService = new OAuthService(oAuthorizationdto);
            oAuthorizationdto.CallBackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Oauth/Response";
            return Redirect(oAuthService.GrantUrl(this));
        }
        /// <summary>
        /// OAuth response and retrieve the access token.
        /// Save the data post encrypt.
        /// Entityframework updated.
        /// </summary>
        /// <returns></returns>
        public ActionResult Response()
        {
            oAuthorizationDB = new OAuthTokens();
            oAuthService = new OAuthService(oAuthorizationdto);
            oAuthorizationdto = oAuthService.GetRequestToken(this);
            if (Request.QueryString.HasKeys())
            {
                oAuthorizationdto.OauthVerifyer = Request.QueryString["oauth_verifier"].ToString();
                oAuthorizationDB.realmid = Convert.ToInt32(Request.QueryString["realmId"].ToString());
                oAuthorizationdto.Realmid = oAuthorizationDB.realmid;
                oAuthorizationDB.datasource = Request.QueryString["dataSource"].ToString();
                oAuthorizationdto.DataSource = oAuthorizationDB.datasource;
                oAuthorizationdto = oAuthService.GetAccessTokenFromServer(this,oAuthorizationdto);
                //encrypt the tokens
                oAuthorizationDB.access_secret = Utility.Encrypt(oAuthorizationdto.AccessTokenSecret, oAuthorizationdto.SecurityKey);
                oAuthorizationDB.access_token = Utility.Encrypt(oAuthorizationdto.AccessToken, oAuthorizationdto.SecurityKey);
                using (var oAuthorizationDBContext = new OAuthdataContext("DBContext"))
                {
                    //store the encrypted tokens to DB.
                    oAuthorizationDBContext.Tokens.Add(oAuthorizationDB);
                    oAuthorizationDBContext.SaveChanges();
                }
            }
            return RedirectToAction("Close", "Home");
        }
    }
}
