using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace experiment2.ActionFilters
{
    public class LogActionFilter : ActionFilterAttribute
    {
        public string system { get; set; }
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    Log("OnActionExecuting", filterContext.RouteData);
        //}

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    Log("OnActionExecuted", filterContext.RouteData);
        //}

        //public override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    Log("OnResultExecuting", filterContext.RouteData);
        //}

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Log("OnResultExecuted", filterContext.RouteData);
        }


        private void Log(string methodName, RouteData routeData)
        {

            string username = HttpContext.Current.User.Identity.GetUserName();
            string user_id = HttpContext.Current.User.Identity.GetUserId();


            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];


            string ip = HttpContext.Current.Request.UserHostAddress;
            var timestamp =  HttpContext.Current.Timestamp.ToString();
            var status = HttpContext.Current.Request.Params;
            var path = HttpContext.Current.Request.Path;
            var session_id = HttpContext.Current.Session.SessionID;
            

            string[] parameters = new string[] { };

            

            string[] src = new string[] { "__RequestVerificationToken","RememberMe", "Username","Password","ASP.NET_SessionId","ai_user","HTTP_REFERER","HTTP_ORIGIN","HTTP_CONTENT_LENGTH", "HTTP_CONTENT_LENGTH",
                "HTTP_CONTENT_TYPE", "HTTP_CACHE_CONTROL",".ASPXAUTH", ".AspNet.ApplicationCookie", 
                "ALL_HTTP", "ALL_RAW", "APPL_MD_PATH", "APPL_PHYSICAL_PATH", "AUTH_TYPE", "AUTH_USER", "AUTH_PASSWORD", "LOGON_USER", "REMOTE_USER",
                "CERT_COOKIE", "CERT_FLAGS", "CERT_ISSUER", "CERT_KEYSIZE", "CERT_SECRETKEYSIZE", "CERT_SERIALNUMBER", "CERT_SERVER_ISSUER", 
                "CERT_SERVER_SUBJECT", "CERT_SUBJECT", "CONTENT_LENGTH", "CONTENT_TYPE", "GATEWAY_INTERFACE", "HTTPS", "HTTPS_KEYSIZE",
                "HTTPS_SECRETKEYSIZE", "HTTPS_SERVER_ISSUER", "HTTPS_SERVER_SUBJECT", "INSTANCE_ID", "INSTANCE_META_PATH", "LOCAL_ADDR",
                "PATH_INFO", "PATH_TRANSLATED", "QUERY_STRING", "REMOTE_ADDR", "REMOTE_HOST", "REMOTE_PORT", "REQUEST_METHOD", "SCRIPT_NAME",
                "SERVER_NAME", "SERVER_PORT", "SERVER_PORT_SECURE", "SERVER_PROTOCOL", "SERVER_SOFTWARE", "URL", "HTTP_CONNECTION", "HTTP_ACCEPT",
                "HTTP_ACCEPT_ENCODING", "HTTP_ACCEPT_LANGUAGE", "HTTP_COOKIE", "HTTP_HOST", "HTTP_USER_AGENT", "HTTP_UPGRADE_INSECURE_REQUESTS" };

            Dictionary<string, string> finalToDumpToDB = new Dictionary<string, string>();



            foreach (string keyval in status.Keys)
            {
                bool isToAdd = true;
                foreach (string _src in src)
                {
                    if (_src.ToString() == keyval.ToString())
                        isToAdd = false;

                }

                if (isToAdd) { 
                finalToDumpToDB.Add(keyval, status[keyval]);
                 parameters = finalToDumpToDB.Values.ToArray();
                }

            }



           // var message = String.Format("Date: {0} | Username: {1} | IP Addresss: {2} | Path: /{3}/{4} | RequestType: {5} ", timestamp, username, ip, controllerName, actionName, methodName);

            //var message = String.Format("{0} PATH: /{1}/{2}. USERNAME: {3} IP ADDRESS:{4} DATE: {5} | {6}", methodName, controllerName, actionName, username,ip, timestamp);
            //Debug.WriteLine(message, "Log");


            // Enter db
            using(var dbContext = new LogTestDBEntities7())
            {

                var logData = dbContext.LogDatas.SingleOrDefault(s => s.SID == session_id);

                if (logData == null)
                {
                    logData = new LogData();
                    logData.IP = ip;
                    logData.UID = user_id;
                    logData.SID = session_id;
                    //
                }

                string DBParameters = string.Concat(parameters);

                //dbContext.LogDatas.Add();

                var logDetails = new LogDetail();
                logDetails.Timestamp = timestamp;
                logDetails.SID = session_id;
                logDetails.Details = path;
                logDetails.Queries = DBParameters;
               

                logData.LogDetails.Add(logDetails);

              // dbContext.LogDatas.Add(logData);
               dbContext.SaveChanges();
           }
        }
   
    }
}