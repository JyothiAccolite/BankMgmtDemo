using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BankManagementDemo.Services;

namespace BankManagementDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private static Dictionary<string, string> _swaggerEntityTypeNames = new Dictionary<string, string>();

        private readonly Func<string, string> _tryGetOrAddSwaggerEntityTypeName = (fullName) =>
        {
            var entityName = fullName.Replace("+", "_");
            var originalName = entityName;
            var lastDotPosition = entityName.LastIndexOf(".");
            if (lastDotPosition != -1)
            {
                entityName = entityName.Substring(lastDotPosition + 1);
            }
            var currentType = _swaggerEntityTypeNames.FirstOrDefault(x => x.Value == entityName);
            if (!string.IsNullOrEmpty(currentType.Key) &&
                currentType.Key != originalName)
            {
                var errorMessage = $"Entity type names are duplicated in the locations: {currentType.Key} and {originalName}.";
                throw new Exception(errorMessage);
            }
            else if (string.IsNullOrEmpty(currentType.Value))
            {
                _swaggerEntityTypeNames.Add(originalName, entityName);
            }
            return entityName;
        };

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Bank Management Demo",
                    Description =
                        "This is an ASP.NET Core Web API for Bank Management. This has APIs which performs all CRUD operations to Get Accounts, Create & delete Accounts, Deposit & Withdraw money",
                    
                });
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo Bank v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
