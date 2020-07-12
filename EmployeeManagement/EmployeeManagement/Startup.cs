using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            //services.AddMvc(options => options.EnableEndpointRouting = false);  // for ASP.NET Core 3.1 if we use app.UseMvcWithDefaultRoute(); (the old 2.2 way)
            services.AddMvc();  // for ASP.NET Core 3.1 if we use app.UseEndpoints (the new way)
            // services.AddMvcCore();  // is contained by AddMvc; this provides less options, the core ones - Json Formatter for example is not added 

            // This line uses the Mock repository, the one in memory
            //services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();  // when someone asks for IEmployee - create an instance of , Mock and inject
            
            // After configuring the DB connection, this line will simply change repository and in just one change switch to DB
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();  // when someone asks for IEmployee - create an instance of , Mock and inject

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            #region custom Developer Exception Page
            //if (env.IsDevelopment()) // can be changed to a custom environment if created and env.IsEnvironment("UAT") true
            //{
            //    DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions()
            //    {
            //        SourceCodeLineCount = 10
            //    };
            //    app.UseDeveloperExceptionPage(developerExceptionPageOptions);
            //}
            //else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))
            //{
            //    app.UseExceptionHandler("/Error");
            //}
            #endregion

            if (env.IsDevelopment())  
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            
            #region middlwareExample
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("Hello World from first middleware");
            //    await next();
            //});

            //app.Use(async (context, next) =>
            //{
            //    logger.LogInformation("MW1: Incoming Request");
            //    await next();
            //    logger.LogInformation("MW1: Outgoing Response");
            //});

            //app.Use(async (context, next) =>
            //{
            //    logger.LogInformation("MW2: Incoming Request");
            //    await next();
            //    logger.LogInformation("MW2: Outgoing Response");
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("MW3: Request handled and response produced");
            //    logger.LogInformation("MW3: Request handled and response produced");
            //});
            #endregion middlewareExample

            #region using static files
            // order is obviously important as demonstrated earlier
            //app.UseDefaultFiles(); // uses the default files like default.html landing page (can use default.htm, index.html, index.htm)

            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("foo.html");
            ////app.UseDefaultFiles(defaultFilesOptions);
            ////app.UseStaticFiles(); // serves static files in wwwroot folder

            ////app.UseFileServer(); // default files
            //FileServerOptions fileServerOptions = new FileServerOptions();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("foo.html");
            //app.UseFileServer(fileServerOptions);
            #endregion

            //app.UseFileServer();
            app.UseStaticFiles();


            #region MVC Core 2.2 way
            // MVC middleware - big oof from Core 2.2 - https://stackoverflow.com/questions/57684093/using-usemvc-to-configure-mvc-is-not-supported-while-using-endpoint-routing
            //app.UseMvcWithDefaultRoute();
            //app.UseMvc();
            #endregion

            #region MVC Core 3.1 way
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            #endregion


            //app.Run(async (context) =>
            //{
            //    // throw new Exception("Some error processing the request."); // used to prove cutom dev page config
            //    await context.Response.WriteAsync("Last middleware Hello Wrld aka no mid caught it 404");
            //});

            #region newASP.NETCore3.1defaultConfig
            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        //await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            //        //await context.Response.WriteAsync(_config["MyKey"]);
            //        await context.Response.WriteAsync("Hello world");

            //    });
            //});
            #endregion newASP.NETCore3.1defaultConfig
        }
    }
}
