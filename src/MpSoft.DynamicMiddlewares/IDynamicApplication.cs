#region using
using Microsoft.AspNetCore.Builder;
using System;
#endregion using

namespace MpSoft.DynamicMiddlewares
{
	public interface IDynamicApplication
	{
		void RemoveSpan(string name);
		void InsertBefore(string name,Action<IApplicationBuilder> appAction);
		void InsertAfter(string name,Action<IApplicationBuilder> appAction);
	}
}
