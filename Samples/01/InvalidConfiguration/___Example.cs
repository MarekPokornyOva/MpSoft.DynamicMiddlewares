//		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//		public void Configure(IApplicationBuilder app,IWebHostEnvironment env,IApplicationDynamic appDynam)
//		{
//			if (env.IsDevelopment())
//			{
//				app.UseDeveloperExceptionPage();
//			}
//
//			var changeToken = Microsoft.Extensions.Primitives.ChangeToken.OnChange(
//				() => Configuration.GetReloadToken(),
//				() => { if (Configuration je valid) { nastav_spravne_middlewares; changeToken.Dispose(); }
//		});
//
//			ISpan span = app.BeginSpan("wrong-conf");
//		app.Use(async (ctx, next) =>
//			{
//				ctx.Response.StatusCode=500;
//				await ctx.Response.WriteAsync("Invalid configuration.");
//	});
//			span.End();
//
//			Timer timer = new Timer(state =>
//			{
//				appDynam.InsertAfter("wrong-conf",appInner =>
//				{
//					appInner.UseRouting();
//
//					appInner.UseEndpoints(a =>
//					{
//						a.MapDefaultControllerRoute();
//					});
//				});
//				appDynam.RemoveSpan("wrong-conf");
//			},null,20_000,Timeout.Infinite);
//
//	//app.UseRouting();
//	//
//	//app.UseEndpoints(a =>
//	//{
//	//	a.MapDefaultControllerRoute();
//	//});
//}
//	}
//}
//