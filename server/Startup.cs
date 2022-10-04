using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using VirtualLeague.Data;

using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;

namespace VirtualLeague
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        partial void OnConfigureServices(IServiceCollection services);

        partial void OnConfiguringServices(IServiceCollection services);

        public void ConfigureServices(IServiceCollection services)
        {
            OnConfiguringServices(services);

            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAny",
                    x =>
                    {
                        x.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(isOriginAllowed: _ => true)
                        .AllowCredentials();
                    });
            });
            var oDataBuilder = new ODataConventionModelBuilder();


            var fixtureGenerators = oDataBuilder.Function("FixtureGeneratorsFunc");
            fixtureGenerators.Parameter<string>("Team1");
            fixtureGenerators.Parameter<string>("Team2");
            fixtureGenerators.Parameter<string>("Team3");
            fixtureGenerators.Parameter<string>("Team4");
            fixtureGenerators.Parameter<string>("Team5");
            fixtureGenerators.Parameter<string>("Team6");
            fixtureGenerators.Parameter<string>("Team7");
            fixtureGenerators.Parameter<string>("Team8");
            fixtureGenerators.Parameter<string>("Team9");
            fixtureGenerators.Parameter<string>("Team10");
            fixtureGenerators.Parameter<string>("Team11");
            fixtureGenerators.Parameter<string>("Team12");
            fixtureGenerators.Parameter<string>("Team13");
            fixtureGenerators.Parameter<string>("Team14");
            fixtureGenerators.Parameter<string>("Team15");
            fixtureGenerators.Parameter<string>("Team16");
            fixtureGenerators.Parameter<string>("Team17");
            fixtureGenerators.Parameter<string>("Team18");
            fixtureGenerators.Parameter<string>("Team19");
            fixtureGenerators.Parameter<string>("Team20");
            fixtureGenerators.ReturnsCollectionFromEntitySet<VirtualLeague.Models.ConData.FixtureGenerator>("FixtureGenerators");
            oDataBuilder.EntitySet<VirtualLeague.Models.ConData.FixtureTemplate>("FixtureTemplates");
            oDataBuilder.EntitySet<VirtualLeague.Models.ConData.LeagueSeason>("LeagueSeasons");
            oDataBuilder.EntitySet<VirtualLeague.Models.ConData.MatchDay>("MatchDays");
            oDataBuilder.EntitySet<VirtualLeague.Models.ConData.SeasonFixture>("SeasonFixtures");

            var seasonFixturesLists = oDataBuilder.Function("SeasonFixturesListsFunc");
            seasonFixturesLists.Parameter<int>("SeasonID");
            seasonFixturesLists.ReturnsCollectionFromEntitySet<VirtualLeague.Models.ConData.SeasonFixturesList>("SeasonFixturesLists");
            oDataBuilder.EntitySet<VirtualLeague.Models.ConData.Team>("Teams");
            oDataBuilder.EntitySet<VirtualLeague.Models.ConData.VirtualLeagueResult>("VirtualLeagueResults");

            this.OnConfigureOData(oDataBuilder);


            var model = oDataBuilder.GetEdmModel();
            services.AddControllers().AddOData(opt => { 
              opt.AddRouteComponents("odata/ConData", model).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
            });

            

            services.AddDbContext<VirtualLeague.Data.ConDataContext>(options =>
            {
              options.UseSqlServer(Configuration.GetConnectionString("ConDataConnection"));
            });

            services.AddRazorPages();

            OnConfigureServices(services);
        }

        partial void OnConfigure(IApplicationBuilder app, IWebHostEnvironment env);
        partial void OnConfigureOData(ODataConventionModelBuilder builder);
        partial void OnConfiguring(IApplicationBuilder app, IWebHostEnvironment env);

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            OnConfiguring(app, env);
            if (env.IsDevelopment())
            {
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.Use((ctx, next) =>
                {
                    return next();
                });
            }
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            IServiceProvider provider = app.ApplicationServices.GetRequiredService<IServiceProvider>();
            app.UseCors("AllowAny");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });

            OnConfigure(app, env);
        }
    }


}
