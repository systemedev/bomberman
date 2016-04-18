using Bomberman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Bomberman
{
    // Remarque : pour obtenir des instructions sur l'activation du mode classique IIS6 ou IIS7, 
    // visitez http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            var waitHandle = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(waitHandle,
                // Method to execute
                (state, timeout) =>
                {
                    using (BOMBERMANEntities db = new BOMBERMANEntities())
                    {
                       db.LookForBombToExplode();
                       db.LookForGameToUpdate();

                    }
                
                
                },
                // optional state object to pass to the method
                null,
                // Execute the method each 1 second               
                TimeSpan.FromSeconds(1),
                // Set this to false to execute it repeatedly every 5 seconds
                false
            );

        }
    }
}