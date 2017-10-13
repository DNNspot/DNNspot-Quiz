using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Security.Membership;

namespace DNNspot.Quiz
{
    public static class DnnHelper
    {
        public static bool LoginUser(string username, string password, bool setPersistentCookie, out UserLoginStatus userLoginStatus, out UserInfo userInfo)
        {
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
            userLoginStatus = UserLoginStatus.LOGIN_FAILURE;

            userInfo = UserController.UserLogin(portalSettings.PortalId, username, password, "", portalSettings.PortalName, HttpContext.Current.Request.UserHostAddress, ref userLoginStatus, setPersistentCookie);

            if (userInfo != null && (userLoginStatus == UserLoginStatus.LOGIN_SUCCESS || userLoginStatus == UserLoginStatus.LOGIN_SUPERUSER))
            {                
                return true;
            }
            return false;            
        }

        public static UserInfo GetUserInfo(int userId, int portalId)
        {
            UserController userController = new UserController();

            return userController.GetUser(portalId, userId);
        }

        public static List<ListItem> GetCountryListItems()
        {
            return GetListNameAsListItems("Country", string.Empty);
        }

        public static List<ListItem> GetRegionListItems()
        {
            return GetListNameAsListItems("Region", string.Empty);
        }

        public static List<ListItem> GetRegionListItems(string countryCode)
        {
            return GetListNameAsListItems("Region", "Country." + countryCode);
        }
        
        private static List<ListItem> GetListNameAsListItems(string listName, string parentKey)
        {
            ListController listController = new ListController();

            ListEntryInfoCollection entries;
            if (string.IsNullOrEmpty(parentKey))
            {
                entries = listController.GetListEntryInfoCollection(listName);
            }
            else
            {
                entries = listController.GetListEntryInfoCollection(listName, parentKey);
            }
            List<ListItem> items = new List<ListItem>(entries.Count);

            foreach (ListEntryInfo entry in entries)
            {
                items.Add(new ListItem() { Text = entry.Text.Trim(), Value = entry.Value.Trim() });
            }

            return items;
        }

        internal static List<CountryInfo> GetCountryListAdoNet()
        {
            string sql = string.Format(@"
                SELECT
                country.Value as CountryCode
                ,country.[Text] as CountryName
                ,region.Value as RegionCode
                ,region.[Text] as RegionName
                FROM {0}Lists country
                LEFT JOIN {0}Lists region ON country.EntryID = region.ParentID
                WHERE country.ListName = 'Country'
                --ORDER BY country.[Text], region.[Text]
            ", GetDbObjectQualifier() ?? "");

            Dictionary<string, CountryInfo> countries = new Dictionary<string, CountryInfo>();

            using (IDataReader reader = DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sql))
            {
                while (reader.Read())
                {
                    string countryCode = reader.GetString(0);
                    string countryName = reader.GetString(1);

                    CountryInfo country;
                    if(!countries.TryGetValue(countryCode, out country))   
                    {
                        country = new CountryInfo() { CountryCode = countryCode, Name = countryName };
                        countries[countryCode] = country;
                    }

                    if (!reader.IsDBNull(2) && !reader.IsDBNull(3))
                    {
                        country.Regions.Add(reader.GetString(3), new RegionInfo() { RegionCode = reader.GetString(2), Name = reader.GetString(3) });
                        //country.Regions.Add(new RegionInfo() { RegionCode = reader.GetString(2), Name = reader.GetString(3) });
                    }
                }
            }

            var list = countries.Values.ToList();
            list.Sort((left, right) => left.Name.CompareTo(right.Name));

            return list;
        }

        #region Methods to find modules by name

        public static List<TabModuleMatch> GetTabsWithModuleByModuleName(int portalId, string moduleName)
        {
            return GetTabsWithModule(portalId, moduleName, string.Empty, string.Empty, string.Empty);
        }

        public static List<TabModuleMatch> GetTabsWithModuleByModuleDefinitionName(int portalId, string moduleDefinitionName)
        {
            return GetTabsWithModule(portalId, string.Empty, string.Empty, moduleDefinitionName, string.Empty);
        }

        private static List<TabModuleMatch> GetTabsWithModule(int portalId, string moduleName, string moduleFolderName, string moduleDefinitionName, string tabPathStartsWith)
        {
            string objectQualifier = GetDbObjectQualifier() ?? "";

            const string extraWhereForDnn5 = @"     
	                AND tm.IsDeleted = 0	                
            ";

            string sql = string.Format(@"
                        SELECT DISTINCT
                        t.PortalID
                        ,t.TabID
                        ,t.TabName
                        ,t.TabPath
                        ,m.ModuleId
                        FROM {0}Tabs t
                        INNER JOIN {0}TabModules tm ON t.TabID = tm.TabID
                        INNER JOIN {0}Modules m ON tm.ModuleID = m.ModuleID
                        INNER JOIN {0}ModuleDefinitions md ON m.ModuleDefID = md.ModuleDefID
                        INNER JOIN {0}DesktopModules dm ON md.DesktopModuleID = dm.DesktopModuleID
                        WHERE
                        t.PortalID = @portalId
                        AND m.IsDeleted = 0
                        AND t.IsDeleted = 0           
                        {1}"
                , objectQualifier, DnnVersionSingleton.Instance.IsDnn5 ? extraWhereForDnn5 : "");

            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@portalId", portalId));

            if (!string.IsNullOrEmpty(tabPathStartsWith))
            {
                sql += " AND t.TabPath LIKE @tabPath + '%'";
                sqlParams.Add(new SqlParameter("@tabPath", tabPathStartsWith));
            }

            if (!string.IsNullOrEmpty(moduleName))
            {
                sql += " AND dm.ModuleName = @moduleName";
                sqlParams.Add(new SqlParameter("@moduleName", moduleName));
            }

            if (!string.IsNullOrEmpty(moduleFolderName))
            {
                sql += " AND dm.FolderName = @folderName";
                sqlParams.Add(new SqlParameter("@folderName", moduleFolderName));
            }

            if (!string.IsNullOrEmpty(moduleDefinitionName))
            {
                sql += " AND md.FriendlyName = @friendlyName";
                sqlParams.Add(new SqlParameter("@friendlyName", moduleDefinitionName));
            }

            sql += " ORDER BY t.TabID, m.ModuleId";

            return GetTabInfoFromSql(sql, sqlParams.ToArray());
        }

        private static List<TabModuleMatch> GetTabInfoFromSql(string sql, params IDataParameter[] commandParameters)
        {
            List<TabModuleMatch> tabModuleMatches = new List<TabModuleMatch>();
            using (IDataReader reader = DotNetNuke.Data.DataProvider.Instance().ExecuteSQL(sql, commandParameters))
            {
                while (reader.Read())
                {
                    TabModuleMatch match = new TabModuleMatch
                    {
                        PortalId = reader.GetInt32(0),
                        TabId = reader.GetInt32(1),
                        TabName = reader.GetString(2),
                        TabPath = reader.GetString(3),
                        ModuleId = reader.GetInt32(4)
                    };

                    tabModuleMatches.Add(match);
                }
            }
            return tabModuleMatches;
        }

        private static string GetDbObjectQualifier()
        {
            ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration("data");
            Provider provider = (Provider)providerConfiguration.Providers[providerConfiguration.DefaultProvider];

            string objectQualifier = provider.Attributes["objectQualifier"];

            if (!string.IsNullOrEmpty(objectQualifier) && !objectQualifier.EndsWith("_"))
            {
                objectQualifier += "_";
            }

            return objectQualifier;
        }

        #endregion
    }

    /// <summary>
    /// Holds basic tab data returned from searching for tabs by module name/definition
    /// </summary>
    public class TabModuleMatch
    {
        public int PortalId;
        public int TabId;
        public string TabName;
        public string TabPath;
        public int ModuleId;
    }

    internal class CountryInfo
    {
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public SortedList<string, RegionInfo> Regions { get; set; }
        //public List<RegionInfo> Regions { get; set; }

        public CountryInfo()
        {
            //Regions = new List<RegionInfo>();
            Regions = new SortedList<string, RegionInfo>();
        }
    }

    internal class RegionInfo
    {
        public string RegionCode { get; set; }
        public string Name { get; set; }
    }
}
