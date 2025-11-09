using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfCrazyZoo.Infrastructure;
using WpfCrazyZoo.Logging;
using WpfCrazyZoo.Repositories;
using WpfCrazyZoo.Data;
using WpfCrazyZoo.Models;


namespace WpfCrazyZoo
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var logMode = ConfigurationManager.AppSettings["Logger"] ?? "xml";
            if (logMode.ToLowerInvariant() == "json")
                DI.Register<ILogger>(() => new JsonLogger());
            else
                DI.Register<ILogger>(() => new XmlLogger());

            var repoMode = (ConfigurationManager.AppSettings["Repository"] ?? "memory").ToLowerInvariant();
            if (repoMode == "sql")
            {
                try
                {
                    ZooDb.EnsureCreated();
                    var cs = ConfigurationManager.ConnectionStrings["ZooDb"].ConnectionString;
                    DI.Register<IRepository<Animal>>(() => new SqlAnimalRepository(cs));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL repository is unavailable. Falling back to memory. Reason: " + ex.Message);
                    DI.Register<IRepository<Animal>>(() => new AnimalRepository());
                }
            }
            else
            {
                DI.Register<IRepository<Animal>>(() => new AnimalRepository());
            }
        }
    }
}
