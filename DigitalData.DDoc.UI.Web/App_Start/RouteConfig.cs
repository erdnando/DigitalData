using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalData.DDoc.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Clear();
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("signalr/hubs");

            routes.MapRoute("DDocMain", "Main/Welcome", (object)new
            {
                controller = "Main",
                action = "welcome"
            });
            routes.MapRoute("Unauthorized", "Error/Unauthorized", (object)new
            {
                controller = "Error",
                action = "Unauthorized"
            });
            routes.MapRoute("SessionExpired", "Error/SessionExpired", (object)new
            {
                controller = "Error",
                action = "SessionExpired"
            });
            routes.MapRoute("DDocNavigateCollection", "Navigation/Navigate/{collectionId}", (object)new
            {
                controller = "Navigation",
                action = "Navigate",
                collectionId = string.Empty
            });
            routes.MapRoute("DDocNavigateCollection2", "Navigation/Navigate2/{collectionId}", (object)new
            {
                controller = "Navigation",
                action = "Navigate2",
                collectionId = string.Empty
            });
            routes.MapRoute("DDocOpenFolder", "Navigation/OpenFolder/{folderId}", (object)new
            {
                controller = "Navigation",
                action = "OpenFolder",
                folderId = string.Empty
            });
            routes.MapRoute("DDocViewer", "Viewer/OpenViewer/{documentId}", (object)new
            {
                controller = "Viewer",
                action = "OpenViewer",
                documentId = string.Empty,
                hideNavigation = false
            });
            routes.MapRoute("NativePdfViewer", "Viewer/OpenPdf/{documentId}/{file}", (object)new
            {
                controller = "Viewer",
                action = "OpenPdf",
                documentId = string.Empty
            });
            routes.MapRoute("CustomActions", "Custom/{action}/{documentId}", (object)new
            {
                controller = "Custom",
                action = string.Empty,
                documentId = string.Empty
            });
            routes.MapRoute("CustomReports", "CustomReports/{action}", (object)new
            {
                controller = "CustomReports",
                action = string.Empty
            });
            routes.MapRoute("Default", "{controller}/{action}", (object)new
            {
                controller = "Security",
                action = "Login"
            });
        }
    }
}
