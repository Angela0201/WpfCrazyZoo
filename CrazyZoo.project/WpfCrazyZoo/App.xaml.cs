using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CrazyZoo.Infrastructure.Repositories;
using CrazyZoo.Infrastructure.Data;
using CrazyZoo.Infrastructure.Infrastructure;
using CrazyZoo.Domain.Models;
using CrazyZoo.Domain.Interfaces;
using CrazyZoo.Application.Interfaces;
using CrazyZoo.Application.Services;

namespace WpfCrazyZoo
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var repoMode = (ConfigurationManager.AppSettings["Repository"] ?? "ef").ToLowerInvariant();

            if (repoMode == "ef")
            {
                DI.Register<ZooDbContext>(() => new ZooDbContext());
                DI.Register<IRepository<Animal>>(() => new EfAnimalRepository(DI.Resolve<ZooDbContext>()));
            }
            else
            {
                DI.Register<IRepository<Animal>>(() => new AnimalRepository());
            }

            DI.Register<IAnimalService>(() => new AnimalService(DI.Resolve<IRepository<Animal>>()));
        }
    }
}
