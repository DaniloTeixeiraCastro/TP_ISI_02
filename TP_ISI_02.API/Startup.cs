using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using TP_ISI_02.API.Services;
using TP_ISI_02.API.Services.Soap;
using TP_ISI_02.Data;
using TP_ISI_02.Data.Repositories;
using TP_ISI_02.Domain.Interfaces;

namespace TP_ISI_02.API
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
            services.AddControllers();

            // CoreWCF Services
            services.AddServiceModelServices();
            services.AddServiceModelMetadata();

            // Swagger Configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TP_ISI_02 API", Version = "v1" });
                
                // JWT Authentication for Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Database Context (ADO.NET)
            services.AddScoped<DatabaseContext>(provider => 
                new DatabaseContext(Configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IImovelRepository, ImovelRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IEventoRepository, EventoRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddHttpClient<IWeatherService, OpenWeatherService>();
            services.AddScoped<IImobiliariaSoapService, ImobiliariaSoapService>(); // Register SOAP Service Implementation

            // JWT Authentication
            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:SecretKey"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"]
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger Middleware
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TP_ISI_02 API v1");
                c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Initialize Database
            try
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var dbInitializer = new DbInitializer(dbContext);
                    dbInitializer.Initialize();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // CoreWCF Middleware
            app.UseServiceModel(serviceBuilder =>
            {
                serviceBuilder.AddService<ImobiliariaSoapService>();
                
                // Configure Binding for HTTPS
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                serviceBuilder.AddServiceEndpoint<ImobiliariaSoapService, IImobiliariaSoapService>(binding, "/soap");
                
                var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<ServiceMetadataBehavior>();
                serviceMetadataBehavior.HttpGetEnabled = true;
                serviceMetadataBehavior.HttpsGetEnabled = true;
            });
        }
    }
}
