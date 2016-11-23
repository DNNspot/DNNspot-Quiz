using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNNspot.Quiz
{
    public class PathHelper
    {
        private ModuleBase module;

        public PathHelper(ModuleBase module)
        {
            this.module = module;
        }

        public string ModuleRootWebPath
        {
            get { return string.Format(@"/DesktopModules/{0}/", this.module.ModuleConfiguration.FolderName); }
        }

        public string ModuleRootFilePath
        {
            get { return HttpContext.Current.Server.MapPath(string.Format(@"~/DesktopModules/{0}/", this.module.ModuleConfiguration.FolderName)); }
        }

        public string ModuleImageDir
        {
            get { return ModuleRootWebPath + "images/"; }
        }

        public string QuizFilePath
        {
            get { return ModuleRootFilePath + @"QuizFiles\"; }
        }

    }
}