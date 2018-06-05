//using Microsoft.Azure.ActiveDirectory.GraphClient;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;

//namespace MS.IoT.MarketingPortal.Web.Controllers
//{
//    public static class AuthConfig
//    {
//        public static class ConfiguratinItems
//        {
//            public static string ClientId
//            {
//                get
//                {
//                    return ConfigurationManager.AppSettings["ida:ClientId"];
//                }
//            }

//            public static string Authority
//            {
//                get
//                {
//                    return ConfigurationManager.AppSettings["ida:AADInstance"] + "common";
//                }
//            }

//            public static string AppKey
//            {
//                get
//                {
//                    return ConfigurationManager.AppSettings["ida:AppKey"];
//                }
//            }

//            public static string GraphAppUri
//            {
//                get
//                {
//                    return ConfigurationManager.AppSettings["ida:GraphAppUri"];
//                }
//            }

//            public static string ManagementAppUri
//            {
//                get
//                {
//                    return ConfigurationManager.AppSettings["ida:ManagementAppUri"];
//                }
//            }
//        }

//        public static class SessionItems
//        {
//            public static string AuthCode
//            {
//                get { return (string)System.Web.HttpContext.Current.Session["AuthCode"]; }
//                set { System.Web.HttpContext.Current.Session["AuthCode"] = value; }
//            }

//            public static Uri AuthCodeLastTokenRequestUrl
//            {
//                get { return (Uri)HttpContext.Current.Session["AuthCodeLastReplyUrl"]; }
//                set { HttpContext.Current.Session["AuthCodeLastReplyUrl"] = value; }
//            }

//            public static string UserObjectId
//            {
//                get { return (string)System.Web.HttpContext.Current.Session["AuthUserObjectId"]; }
//                set { System.Web.HttpContext.Current.Session["AuthUserObjectId"] = value; }
//            }

//            public static string GraphAuthToken
//            {
//                get { return (string)System.Web.HttpContext.Current.Session["GraphAuthToken"]; }
//                set { System.Web.HttpContext.Current.Session["GraphAuthToken"] = value; }
//            }

//            public static string GraphTargetTenant
//            {
//                get { return (string)System.Web.HttpContext.Current.Session["GraphTargetTenant"]; }
//                set { System.Web.HttpContext.Current.Session["GraphTargetTenant"] = value; }
//            }

//            public static string ManagementAuthToken
//            {
//                get { return (string)System.Web.HttpContext.Current.Session["ManagementAuthToken"]; }
//                set { System.Web.HttpContext.Current.Session["ManagementAuthToken"] = value; }
//            }
//        }
//    }
//}