using Blazored.Toast;
using EtkBlazorApp.BL;
using EtkBlazorApp.BL.CronTask;
using EtkBlazorApp.BL.Templates.PriceListTemplates;
using EtkBlazorApp.DataAccess;
using EtkBlazorApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;

namespace EtkBlazorApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //Blazor стандартные
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

            //Blazor дополнительные
            services.AddHttpContextAccessor();

            //Приложение                        
            services.AddTransient<ICompressedFileExtractor, SharpCompressFileExtractor>();
            ConfigureCorrelators(services);
            ConfigureDatabase(services);

            services.AddSingleton<ICurrencyChecker, CurrencyCheckerCbRf>();
            services.AddSingleton<RemoteTemplateFileLoaderFactory>();        
            services.AddSingleton<SystemEventsLogger>();
            services.AddSingleton<NewOrdersNotificationService>();
            services.AddSingleton<UpdateManager>();
            services.AddSingleton<OzonSellerManager>();
            services.AddSingleton<PriceListManager>();
            services.AddSingleton<CronTaskService>();          

            services.AddScoped<AuthenticationStateProvider, MyCustomAuthProvider>();
            services.AddScoped<UserLogger>();
            services.AddScoped<ReportManager>();      

            //Сторонние
            services.AddBlazoredToast();
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddTransient<IDatabaseAccess, EtkDatabaseDapperAccess>();
            services.AddTransient<IProductStorage, ProductStorage>();
            services.AddTransient<IProductUpdateService, ProductUpdateService>();
            services.AddTransient<IPriceListTemplateStorage, PriceListTemplateStorage>();
            services.AddTransient<IPrikatTemplateStorage, PrikatTemplateStorage>();
            services.AddTransient<IOrderStorage, OrderStorage>();
            services.AddTransient<IManufacturerStorage, ManufacturerStorage>();
            services.AddTransient<ILogStorage, LogStorage>();
            services.AddTransient<ISettingStorage, SettingStorage>();
            services.AddTransient<ICronTaskStorage, CronTaskStorage>();
            services.AddTransient<IAuthenticationDataStorage, AuthenticationDataStorage>();
        }

        private void ConfigureCorrelators(IServiceCollection services)
        {
            services.AddTransient<IDatabaseProductCorrelator, FullCompareProductCorrelator>();
            services.AddTransient<IPriceLineLoadCorrelator, SimplePriceLineLoadCorrelator>();
            services.AddTransient<IOzonProductCorrelator, SimpleOzonProductCorrelator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            app.ApplicationServices.GetService<CronTaskService>();
            app.ApplicationServices.GetService<NewOrdersNotificationService>().RefreshInterval = TimeSpan.FromSeconds(5);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
        }
    }
}
