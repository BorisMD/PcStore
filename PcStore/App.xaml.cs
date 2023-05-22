using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PcStore.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PcStore
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            try
            {
                var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
                var mainWindow = new MainWindow(mainViewModel);
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // Log the exception or show a message box.
                MessageBox.Show(ex.ToString());
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();  // Register MainViewModel
            services.AddSingleton<PCBuilderContext>();
        }
    }
}