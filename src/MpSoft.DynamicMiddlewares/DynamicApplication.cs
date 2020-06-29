#region using
using Microsoft.AspNetCore.Builder;
using System;
#endregion using

namespace MpSoft.DynamicMiddlewares.Internal
{
	public class DynamicApplication:IDynamicApplication, IDynamicApplicationInternal
	{
		internal IDynamicApplicationBuilder _applicationBuilder;

		public void SetApplicationBuilder(IDynamicApplicationBuilder applicationBuilder)
			=> _applicationBuilder=applicationBuilder;

		public void RemoveSpan(string name)
			=> _applicationBuilder.RemoveSpan(name);

		public void InsertBefore(string name,Action<IApplicationBuilder> appAction)
			=> _applicationBuilder.InsertBefore(name,appAction);

		public void InsertAfter(string name,Action<IApplicationBuilder> appAction)
			=> _applicationBuilder.InsertAfter(name,appAction);
	}
}
