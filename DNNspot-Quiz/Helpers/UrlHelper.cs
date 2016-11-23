using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using WA.Extensions;

namespace DNNspot.Quiz
{
    public class UrlHelper
    {
        int portalId;
        int? dispatchTabId = null;
        int? adminDispatchTabId = null;

        public UrlHelper(int portalId)
        {
            this.portalId = portalId;

            var tabs = DnnHelper.GetTabsWithModuleByModuleDefinitionName(portalId, "EBookLibrary-Dispatch");
            if (tabs != null && tabs.Count > 0)
            {
                dispatchTabId = tabs[0].TabId;
            }

            tabs = DnnHelper.GetTabsWithModuleByModuleDefinitionName(portalId, "EBookLibrary-Admin");
            if (tabs != null && tabs.Count > 0)
            {
                adminDispatchTabId = tabs[0].TabId;
            }
        }

        public string BookDetailUrl(int bookId)
        {
            if(dispatchTabId.HasValue)
            {
                return DotNetNuke.Common.Globals.NavigateURL(dispatchTabId.Value, string.Empty, new string[] { "v=BookDetail", "id=" + bookId });
            }
            return string.Empty;            
        }

        public string BookImageUrl(string imageFile)
        {
            return string.Format(@"/DesktopModules/EBookLibrary/BookFiles/{0}", imageFile);
        }

        public string SearchResultsUrl(string searchQuery)
        {
            return SearchResultsUrl(new List<string>() { "q=" + searchQuery });
        }

        public string SearchResultsUrl(IList<string> queryParams)
        {
            queryParams.Insert(0,"v=SearchResults");
            
            if(dispatchTabId.HasValue)
            {
                return DotNetNuke.Common.Globals.NavigateURL(dispatchTabId.Value, string.Empty, queryParams.Select(HttpUtility.UrlPathEncode).ToArray());
            }
            return string.Empty;
        }

        public string BookDownloadUrl(int bookId, IList<Guid> downloadTokens)
        {
            if (dispatchTabId.HasValue)
            {
                return DotNetNuke.Common.Globals.NavigateURL(dispatchTabId.Value, string.Empty, new string[] { "v=BookDownload", "bookId=" + bookId, "tokens=" + downloadTokens.ToCsv() });
            }
            return string.Empty;
        }


        public string DownloadTokenUrl(Guid downloadTokenId)
        {
            var portalSettings = PortalController.GetCurrentPortalSettings();
            string root = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Scheme) + portalSettings.PortalAlias.HTTPAlias.EnsureEndsWith("/");

            return string.Format(@"{0}DesktopModules/EBookLibrary/Download.ashx?id={1}", root, downloadTokenId);
        }

        public string AdminView(string viewName)
        {
            return AdminView(viewName, new List<string>());
        }

        public string AdminView(string viewName, IList<string> queryParams)
        {
            queryParams.Insert(0, "v=" + viewName);

            if (adminDispatchTabId.HasValue)
            {
                return DotNetNuke.Common.Globals.NavigateURL(adminDispatchTabId.Value, string.Empty, queryParams.Select(HttpUtility.UrlPathEncode).ToArray());
            }
            return string.Empty;            
        }
    }
}