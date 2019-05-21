﻿using Newtonsoft.Json;
using Syncfusion.Reporting.Web.ReportViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ReportServices.Controllers.docs
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomAjaxHeadersController : ApiController, IReportController
    {
        //Post action for processing the rdl/rdlc report 
        public string DefaultParameter = null;
        string authenticationHeader;

        public object PostReportAction(Dictionary<string, object> jsonResult)
        {
            if (jsonResult != null)
            {
                if (jsonResult.ContainsKey("CustomData"))
                {
                    //Get client side custom data and store in local variable. Here parameter values are sent.
                    DefaultParameter = jsonResult["CustomData"].ToString();
                }

                //Get client side custom ajax header and store in local variable
                authenticationHeader = HttpContext.Current.Request.Headers["Authorization"];

                //Perform your custom validation here
                if (authenticationHeader == "")
                {
                    return new Exception("Authentication failed!!!");
                }
                else
                {
                    return ReportHelper.ProcessReport(jsonResult, this);
                }
            }

            return null;
        }

        //Get action for getting resources from the report
        [System.Web.Http.ActionName("GetResource")]
        [AcceptVerbs("GET")]
        public object GetResource(string key, string resourcetype, bool isPrint)
        {
            return ReportHelper.GetResource(key, resourcetype, isPrint);
        }

        //Method will be called when initialize the report options before start processing the report        
        public void OnInitReportOptions(ReportViewerOptions reportOption)
        {
            var resourcesPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Scripts");

            reportOption.ReportModel.ExportResources.Scripts = new List<string>
            {
                resourcesPath + @"\reports\common\ej.reporting.common.min.js",
                resourcesPath + @"\reports\common\ej.reporting.widgets.min.js",
                //Chart component script
                resourcesPath + @"\reports\data-visualization\ej.chart.min.js",
                //Gauge component scripts
                resourcesPath + @"\reports\data-visualization\ej.lineargauge.min.js",
                resourcesPath + @"\reports\data-visualization\ej.circulargauge.min.js",
                //Map component script
                resourcesPath + @"\reports\data-visualization\ej.map.min.js",
                //Report Viewer Script
                resourcesPath + @"\reports\ej.report-viewer.min.js"
            };

            reportOption.ReportModel.ExportResources.DependentScripts = new List<string>
            {
                "https://code.jquery.com/jquery-1.10.2.min.js"
            };
        }

        //Method will be called when reported is loaded
        public void OnReportLoaded(ReportViewerOptions reportOption)
        {
            if (DefaultParameter != null)
            {
                //Set client side custom header data 
                reportOption.ReportModel.Parameters = JsonConvert.DeserializeObject<List<Syncfusion.Reporting.Web.ReportParameter>>(DefaultParameter);
            }
        }
    }
}