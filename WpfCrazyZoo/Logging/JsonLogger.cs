using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCrazyZoo.Logging
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    public class JsonLogger : ILogger
    {
        public string DefaultExtension => "json";
        public string FileDialogFilter => "JSON files (*.json)|*.json|All files (*.*)|*.*";

        public void SaveLogs(IEnumerable<string> logs, string path)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(logs, options);
            File.WriteAllText(path, json);
        }
    }
}
