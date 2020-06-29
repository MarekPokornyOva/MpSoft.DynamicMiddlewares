using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using MpSoft.DynamicMiddlewares;

namespace InvalidConfiguration
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration=configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDynamicMiddlewares();

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,IWebHostEnvironment env,IDynamicApplication appDynam)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			if (CheckConfiguration())
				ConfigureSiteMiddlewares(app);
			else
			{
				//set up replaceble middleware/s
				string wrongConfigurationName = "wrong-conf";
				ISpan span = app.BeginSpan(wrongConfigurationName);
				app.Use(async (ctx,next) =>
				{
					ctx.Response.StatusCode=500;
					await ctx.Response.WriteAsync("Invalid configuration.");
				});
				span.End();

				//set up configuration change watch
				IDisposable changeToken=null;
				changeToken=ChangeToken.OnChange(
					() => Configuration.GetReloadToken(),
					() =>
					{
						if (CheckConfiguration())
						{
							//configure the site middlewares
							appDynam.InsertAfter(wrongConfigurationName,ConfigureSiteMiddlewares);
							//remove the blocking middleware
							appDynam.RemoveSpan(wrongConfigurationName);
							//remove the watch if we no need it anymore
							changeToken.Dispose();
						}
					}
				);
			}
		}

		void ConfigureSiteMiddlewares(IApplicationBuilder app)
		{
			app.UseRouting();
			
			app.UseAuthorization();
			
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
						 name: "default",
						 pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		bool CheckConfiguration()
			=> !object.Equals(Configuration.GetValue<object>("Success"),"");
	}
}
