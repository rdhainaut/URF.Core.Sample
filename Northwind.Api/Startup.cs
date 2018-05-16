using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Northwind.Data.Models;
using Northwind.Repository;
using Northwind.Service;
using Urf.Core.Abstractions;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF;
using URF.Core.EF.Trackable;

namespace Northwind.Api
{
    public class Startup
    {
        // See https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging#filtering-what-is-logged
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new []{
            new ConsoleLoggerProvider((category, level)
                => category == DbLoggerCategory.Database.Command.Name
                   && level == LogLevel.Information, true)
        });

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddMvc();

            services.AddMvc()
                .AddJsonOptions(options =>
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All);

            services.AddOData();

            var connectionString = Configuration.GetConnectionString(nameof(NorthwindContext));
            services.AddDbContext<NorthwindContext>(options =>
            {
                options.UseLoggerFactory(MyLoggerFactory); // add sql raw logging 
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<DbContext, NorthwindContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITrackableRepository<Products>, TrackableRepository<Products>>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<ITrackableRepository<Suppliers>, TrackableRepository<Suppliers>>();
            services.AddScoped<ISupplierService, SupplierService>();

            // Example: extending IRepository<TEntity>, scope: application-wide and IService<TEntity>, scope: ICustomerService
            services.AddScoped<IRepositoryX<Customers>, RepositoryX<Customers>>();
            services.AddScoped<ICustomerService, CustomerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
                builder.Build();
            });

            var oDataBuilder = new ODataConventionModelBuilder(app.ApplicationServices);
            oDataBuilder.Namespace = "Default";
            
            var productsEntitySetConfiguration = oDataBuilder.EntitySet<Products>(nameof(Products));
            productsEntitySetConfiguration.EntityType.HasKey(x => x.ProductId);
            productsEntitySetConfiguration.EntityType.Ignore(x => x.Category);
            //productsEntitySetConfiguration.EntityType.Ignore(x => x.Supplier);
            productsEntitySetConfiguration.EntityType.Ignore(x => x.OrderDetails);

            var customersEntitySetConfiguration = oDataBuilder.EntitySet<Customers>(nameof(Customers));
            customersEntitySetConfiguration.EntityType.HasKey(x => x.CustomerId);
            customersEntitySetConfiguration.EntityType.Ignore(x => x.CustomerCustomerDemo);
            customersEntitySetConfiguration.EntityType.Ignore(x => x.Orders);
            
            var suppliersEntitySetConfiguration = oDataBuilder.EntitySet<Suppliers>(nameof(Suppliers));
            suppliersEntitySetConfiguration.EntityType.HasKey(x => x.SupplierId);
            suppliersEntitySetConfiguration.EntityType.Ignore(x => x.Products);

            oDataBuilder
                .EntityType<Products>()
                .Collection
                .Function("CalculateStockCashValue")
                .Returns<decimal>();

            app.UseMvc(routeBuilder =>
                {
                    routeBuilder.Select().Expand().Filter().OrderBy().MaxTop(1000).Count();
                    routeBuilder.MapODataServiceRoute(routeName: "ODataRoute", routePrefix: "odata", model: oDataBuilder.GetEdmModel());
                    routeBuilder.EnableDependencyInjection();
                }
            );
        }
    }
}
