using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZoo.Infrastructure.Logging
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;

    public class XmlLogger : ILogger
    {
        public string DefaultExtension => "xml";
        public string FileDialogFilter => "XML files (*.xml)|*.xml|All files (*.*)|*.*";

        public void SaveLogs(IEnumerable<string> logs, string path)
        {
            var root = new XElement("Logs");
            foreach (var line in logs)
            {
                root.Add(new XElement("Log", line ?? string.Empty));
            }
            var doc = new XDocument(root);
            using (var fs = File.Create(path))
            {
                doc.Save(fs);
            }
        }
    }
}
