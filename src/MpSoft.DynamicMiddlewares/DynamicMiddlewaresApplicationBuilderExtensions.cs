#region using
using Microsoft.Extensions.DependencyInjection;
using MpSoft.DynamicMiddlewares;
using System;
#endregion using

namespace Microsoft.AspNetCore.Builder
{
	public static class DynamicMiddlewaresApplicationBuilderExtensions
	{
		public static ISpan BeginSpan(this IApplicationBuilder app,string name)
			=> app is IDynamicApplicationBuilder dab
				? dab.BeginSpan(name)
				: throw new InvalidOperationException($"The call is valid for {nameof(IDynamicApplicationBuilder)} only. Check the {nameof(DynamicMiddlewaresServiceCollectionExtensions)}.{nameof(DynamicMiddlewaresServiceCollectionExtensions.AddDynamicMiddlewares)} was called.");
	}
}
