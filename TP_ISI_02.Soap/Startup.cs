using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TP_ISI_02.Data;
using TP_ISI_02.Data.Repositories;
using TP_ISI_02.Domain.Interfaces;

namespace TP_ISI_02.Soap
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
            services.AddServiceModelServices();
            services.AddServiceModelMetadata();
            services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

            // Dependency Injection
            services.AddScoped<DatabaseContext>(provider => 
                new DatabaseContext(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddScoped<IImovelRepository, ImovelRepository>();
            services.AddScoped<IImobiliariaSoapService, ImobiliariaSoapService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceModel(serviceBuilder =>
            {
                serviceBuilder.AddService<ImobiliariaSoapService>();
                serviceBuilder.AddServiceEndpoint<ImobiliariaSoapService, IImobiliariaSoapService>(new BasicHttpBinding(), "/soap");
                
                var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<ServiceMetadataBehavior>();
                serviceMetadataBehavior.HttpGetEnabled = true;
            });
        }
    }
}
