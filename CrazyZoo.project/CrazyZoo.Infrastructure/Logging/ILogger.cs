namespace CrazyZoo.Infrastructure.Logging
{
    using System.Collections.Generic;

    public interface ILogger
    {
        void SaveLogs(IEnumerable<string> logs, string path);
        string DefaultExtension { get; }
        string FileDialogFilter { get; }
    }
}
