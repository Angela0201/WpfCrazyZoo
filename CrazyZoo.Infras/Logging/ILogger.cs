using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCrazyZoo.Logging
{
    using System.Collections.Generic;

    public interface ILogger
    {
        void SaveLogs(IEnumerable<string> logs, string path);
        string DefaultExtension { get; }
        string FileDialogFilter { get; }
    }
}
