#region using
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MpSoft.DynamicMiddlewares;
using MpSoft.DynamicMiddlewares.Internal;
#endregion using

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DynamicMiddlewaresServiceCollectionExtensions
	{
		public static IServiceCollection AddDynamicMiddlewares(this IServiceCollection services)
		{
			services.Replace(new ServiceDescriptor(typeof(IApplicationBuilderFactory),typeof(DynamicApplicationBuilderFactory),ServiceLifetime.Singleton));
			DynamicApplication applicationDynamic = new DynamicApplication();
			services.AddSingleton<IDynamicApplication>(applicationDynamic);
			services.AddSingleton<IDynamicApplicationInternal>(applicationDynamic);
			return services;
		}
	}
}
