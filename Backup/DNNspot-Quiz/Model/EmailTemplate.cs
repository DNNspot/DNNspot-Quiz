using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WA.Extensions;

namespace DNNspot.Quiz
{
    public class EmailTemplate
    {
        public string FromEmail { get; set; }
        public List<string> ToList { get; set; }
        public List<string> CcList { get; set; }
        public List<string> BccList { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public EmailTemplate()
        {
            ToList = new List<string>();
            CcList = new List<string>();
            BccList = new List<string>();
        }

        private static string ResolveTemplateNameToFile(string templateName)
        {
            return HttpContext.Current.Server.MapPath(string.Format("~/DesktopModules/{0}/EmailTemplates/{1}.xml", Constants.FolderName, templateName));
        }

        public static EmailTemplate Load(string templateName)
        {
            string templateFile = ResolveTemplateNameToFile(templateName);
            if (File.Exists(templateFile))
            {
                var template = new EmailTemplate();

                var xml = XElement.Load(templateFile);                
                template.FromEmail = xml.Element("from").Value;
                template.ToList = ParseEmailStringToList(xml.Element("to"));
                template.CcList = ParseEmailStringToList(xml.Element("cc"));
                template.BccList = ParseEmailStringToList(xml.Element("bcc"));
                template.Subject = HttpUtility.HtmlDecode(xml.Element("subject").Value);
                template.Body = HttpUtility.HtmlDecode(xml.Element("body").Value);

                return template;
            }
            return null;
        }

        public static void Save(string templateName, EmailTemplate template)
        {
            var xml = new XElement(
                "email",
                new XElement("from",template.FromEmail),
                new XElement("to", template.ToList.ToCsv()),
                new XElement("cc", template.CcList.ToCsv()),
                new XElement("bcc", template.BccList.ToCsv()),
                new XElement("subject", new XCData(template.Subject)),
                new XElement("body", new XCData(template.Body))
                );

            string templateFile = ResolveTemplateNameToFile(templateName);
            File.WriteAllText(templateFile, xml.ToString());
        }

        private static List<string> ParseEmailStringToList(XElement xelm)
        {
            if (xelm != null)
            {
                if (!string.IsNullOrEmpty(xelm.Value))
                {
                    return xelm.Value.Replace(';', ',').Split(',').ToList();
                }
            }
            return new List<string>();
        }
    }
}