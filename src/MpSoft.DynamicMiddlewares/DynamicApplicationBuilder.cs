#region using
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
#endregion using

namespace MpSoft.DynamicMiddlewares.Internal
{
	public class DynamicApplicationBuilder:IDynamicApplicationBuilder, IApplicationBuilder
	{
		#region .ctor
		readonly IDynamicApplicationInternal _dynamicApplicationInternal;
		IApplicationBuilder _applicationBuilder;
		public DynamicApplicationBuilder(IServiceProvider serviceProvider,object server,IDynamicApplicationInternal dynamicApplicationInternal)
			: this(new ApplicationBuilderFactory(serviceProvider).CreateBuilder((IFeatureCollection)server) /*new ApplicationBuilder(serviceProvider,server)*/,new Dictionary<string,(int, int)>(),dynamicApplicationInternal)
		{
		}

		DynamicApplicationBuilder(IApplicationBuilder applicationBuilder,IDictionary<string,(int Begin, int End)> spans,IDynamicApplicationInternal dynamicApplicationInternal)
		{
			_applicationBuilder=applicationBuilder;
			_spans=spans;
			_dynamicApplicationInternal=dynamicApplicationInternal;

			_componentsFieldInfo=applicationBuilder.GetType().GetField("_components",BindingFlags.NonPublic|BindingFlags.Instance);
		}

		#endregion .ctor

		#region IApplicationBuilder implementation
		public IServiceProvider ApplicationServices
		{
			get => _applicationBuilder.ApplicationServices;
			set => _applicationBuilder.ApplicationServices=value;
		}
		
		public IFeatureCollection ServerFeatures => _applicationBuilder.ServerFeatures;
		
		public IDictionary<string,object> Properties => _applicationBuilder.Properties;
		
		
		public IApplicationBuilder Use(Func<RequestDelegate,RequestDelegate> middleware)
		{
			_applicationBuilder=_applicationBuilder.Use(middleware);
			return this;
		}
		
		public IApplicationBuilder New()
			=> new DynamicApplicationBuilder(_applicationBuilder.New(),_spans,_dynamicApplicationInternal);
		
		public RequestDelegate Build()
		{
			_dynamicApplicationInternal.SetApplicationBuilder(this);
			_builtApplication=_applicationBuilder.Build();
			return Do;
		}
		#endregion IApplicationBuilder implementation

		RequestDelegate _builtApplication;
		Task Do(HttpContext httpContext)
			=> _builtApplication(httpContext);
		
		void Rebuild()
		{
			_builtApplication=_applicationBuilder.Build();
		}

		#region IDynamicApplicationBuilder implementation
		public ISpan BeginSpan(string name)
			=> new Span { Begin=GetComponents().Count,Name=name,_endAction=EndSpan };

		public void RemoveSpan(string name)
		{
			(int begin, int end)=_spans[name];
			int delta = end-begin;

			lock (_componentsFieldInfo)
			{
				IList<Func<RequestDelegate,RequestDelegate>> components = GetComponents();
				IList<Func<RequestDelegate,RequestDelegate>> newVersion = new List<Func<RequestDelegate,RequestDelegate>>(components.Count-delta);

				int a = 0;
				foreach (Func<RequestDelegate,RequestDelegate> item in components)
				{
					if ((a<begin)||(a>=end))
						newVersion.Add(item);
					a++;
				}

				SetComponents(newVersion);
				Rebuild();

				_spans.Remove(name);
				foreach (string key in _spans.Keys)
				{
					(int beginKey, int endKey)=_spans[key];
					if (beginKey>end)
						_spans[key]=(beginKey-delta, endKey-delta);
				}
			}
		}

		public void InsertBefore(string name,Action<IApplicationBuilder> appAction)
			=> Insert(appAction,_spans[name].Begin);

		public void InsertAfter(string name,Action<IApplicationBuilder> appAction)
			=> Insert(appAction,_spans[name].End);

		#endregion IDynamicApplicationBuilder implementation

		#region spans internal
		readonly IDictionary<string,(int Begin, int End)> _spans;
		
		void EndSpan(ISpan span)
			=> _spans.Add(span.Name,(span.Begin, GetComponents().Count));

		void Insert(Action<IApplicationBuilder> appAction,int newPos)
		{
			MiddleWareCollector mc = new MiddleWareCollector(this);
			appAction(mc);
			IList<Func<RequestDelegate,RequestDelegate>> toInsert = mc.GetComponents();
			int delta = toInsert.Count;

			lock (_componentsFieldInfo)
			{
				IList<Func<RequestDelegate,RequestDelegate>> components = GetComponents();
				IList<Func<RequestDelegate,RequestDelegate>> newVersion = new List<Func<RequestDelegate,RequestDelegate>>(components.Count+delta);
				int a = 0;
				foreach (Func<RequestDelegate,RequestDelegate> item in components)
				{
					newVersion.Add(item);
					if (++a==newPos)
						foreach (Func<RequestDelegate,RequestDelegate> item2 in toInsert)
							newVersion.Add(item2);
				}

				SetComponents(newVersion);
				Rebuild();

				foreach (string key in _spans.Keys)
				{
					(int beginKey, int endKey)=_spans[key];
					if (beginKey>newPos)
						_spans[key]=(beginKey+delta, endKey+delta);
				}
			}
		}

		class Span:ISpan
		{
			public int Begin { get; internal set; }
			public string Name { get; internal set; }

			internal Action<ISpan> _endAction;
			public void End()
				=> _endAction(this);
		}

		class MiddleWareCollector:IApplicationBuilder
		{
			readonly IApplicationBuilder _inner;

			internal MiddleWareCollector(IApplicationBuilder inner)
				=> _inner=inner;

			public IServiceProvider ApplicationServices { get => _inner.ApplicationServices; set => throw new NotImplementedException(); }

			public IDictionary<string,object> Properties => _inner.Properties;

			public IFeatureCollection ServerFeatures => _inner.ServerFeatures;

			public RequestDelegate Build()
			{
				throw new NotImplementedException();
			}

			public IApplicationBuilder New()
				=> this;

			readonly IList<Func<RequestDelegate,RequestDelegate>> _components = new List<Func<RequestDelegate,RequestDelegate>>();
			public IApplicationBuilder Use(Func<RequestDelegate,RequestDelegate> middleware)
			{
				_components.Add(middleware);
				return this;
			}

			internal IList<Func<RequestDelegate,RequestDelegate>> GetComponents()
				=> _components;
		}
		#endregion spans internal

		#region components
		readonly FieldInfo _componentsFieldInfo;
		IList<Func<RequestDelegate,RequestDelegate>> _components;
		IList<Func<RequestDelegate,RequestDelegate>> GetComponents()
			=> _components??=(IList<Func<RequestDelegate,RequestDelegate>>)_componentsFieldInfo.GetValue(_applicationBuilder);
		void SetComponents(IList<Func<RequestDelegate,RequestDelegate>> newVersion)
			=> _componentsFieldInfo.SetValue(_applicationBuilder,_components=newVersion);
		#endregion components
	}
}
