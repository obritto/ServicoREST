﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

using ServicoREST.Models;

using System.Web.OData.Builder;
using System.Web.OData.Extensions;




namespace ServicoREST
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // odata
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Pessoas>("PessoasOData");
            config.MapODataServiceRoute(routeName: "ODataRoute", routePrefix: "odata", model: builder.GetEdmModel());

        }
    }
}
