using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using DotNetNuke.Entities.Modules;

namespace DNNspot.Quiz
{
    public class ModuleBase : PortalModuleBase
    {
        public UrlHelper Urls;
        public PathHelper Paths;

        protected void Page_Init(object sender, EventArgs e)
        {            
            Urls = new UrlHelper(PortalId);
            Paths = new PathHelper(this);

            RegisterJQuery();
        }

        [Obsolete]
        private void RegisterJQuery()
        {
            // NOTE - blows up when running in DNN v4
            //if (DnnVersionSingleton.Instance.IsDnn5)
            //{
            //    DotNetNuke.Framework.jQuery.RequestRegistration();
            ////}
            //if (HttpContext.Current.Items["jquery_registered"] == null || HttpContext.Current.Items["jquery14_registered"] == null)
            //{
            //    string pathToJquery = Paths.ModuleRootWebPath + "js/jquery-1.6.1.min.js";

            //    RegisterJavascriptFileInHeader(pathToJquery);

            //    HttpContext.Current.Items.Add("jquery_registered", "true");
            //    HttpContext.Current.Items.Add("jquery14_registered", "true");                
            //}
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // include any needed javascript files here...               
            RegisterJavascriptFileOnceInBody("jquery.validate.min.js", Paths.ModuleRootWebPath + "js/jquery.validate.min.js");
            RegisterJavascriptFileOnceInBody("jquery.browser.min.js", Paths.ModuleRootWebPath + "js/jquery.browser.min.js");
            RegisterJavascriptFileOnceInBody("jquery.printElement.min.js", Paths.ModuleRootWebPath + "js/jquery.printElement.min.js");            
        }

        protected new bool IsEditable
        {
            get
            {
                return base.IsEditable || UserInfo.IsSuperUser || UserInfo.IsInRole(PortalSettings.AdministratorRoleName);
            }
        }

        protected DotNetNuke.Framework.CDefault DnnPage
        {
            get { return (DotNetNuke.Framework.CDefault)Page; }
        }

        protected void RegisterJavascriptFileOnceInBody(string keyName, string pathToJsFile)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(keyName))
            {
                Page.ClientScript.RegisterClientScriptInclude(keyName, pathToJsFile);
            }
        }

        [Obsolete]
        protected void RegisterJavascriptFileInHeader(string pathToJsFile)
        {
            HtmlGenericControl script = new HtmlGenericControl("script");
            script.Attributes.Add("type", "text/javascript");
            script.Attributes.Add("src", pathToJsFile);
            Page.Header.Controls.Add(script);
        }
    }
}