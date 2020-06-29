#region using
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http.Features;
using MpSoft.DynamicMiddlewares.Internal;
using System;
#endregion using

namespace MpSoft.DynamicMiddlewares
{
	public class DynamicApplicationBuilderFactory:IApplicationBuilderFactory
	{
		readonly IServiceProvider _serviceProvider;
		readonly IDynamicApplicationInternal _dynamicApplicationInternal;
		public DynamicApplicationBuilderFactory(IServiceProvider serviceProvider,IDynamicApplicationInternal dynamicApplicationInternal)
		{
			_serviceProvider=serviceProvider;
			_dynamicApplicationInternal=dynamicApplicationInternal;
		}

		public IApplicationBuilder CreateBuilder(IFeatureCollection serverFeatures)
			=> new DynamicApplicationBuilder(_serviceProvider,serverFeatures,_dynamicApplicationInternal);
	}
}
