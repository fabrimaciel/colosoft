/* 
 * Colosoft Framework - generic framework to assist in development on the .NET platform
 * Copyright (C) 2013  <http://www.colosoft.com.br/framework> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.AspNet.SignalR;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web
{
	/// <summary>
	/// Aplicação web para acessar os recursos do HMI.
	/// </summary>
	public class WebApplication : IHttpApplication, IDisposable
	{
		private IDisposable _webApp;

		private IEnumerable<System.Web.Hosting.VirtualPathProvider> _virtualPathProviders;

		private RazorEngine.Configuration.ITemplateServiceConfiguration _templateServiceConfiguration;

		private IEnumerable<System.Reflection.Assembly> _referenceAssemblies;

		private int _webAppPort;

		private Colosoft.Owin.Razor.RazorViewEngine _viewEngine;

		private readonly System.Threading.CancellationTokenSource _shutdownTokenSource;

		private List<IHttpModule> _modules = new List<IHttpModule>();

		/// <summary>
		/// Número da porta da aplicação web.
		/// </summary>
		public int WebAppPort
		{
			get
			{
				return _webAppPort;
			}
			set
			{
				_webAppPort = value;
			}
		}

		/// <summary>
		/// ViewEngine
		/// </summary>
		internal Owin.Razor.RazorViewEngine ViewEngine
		{
			get
			{
				return _viewEngine;
			}
		}

		/// <summary>
		/// Configuração do serviço de modelos.
		/// </summary>
		public RazorEngine.Configuration.ITemplateServiceConfiguration TemplateServiceConfiguration
		{
			get
			{
				return _templateServiceConfiguration;
			}
		}

		/// <summary>
		/// Construtor protegido.
		/// </summary>
		protected WebApplication(int port)
		{
			_shutdownTokenSource = new System.Threading.CancellationTokenSource();
			_webAppPort = port;
			_referenceAssemblies = new System.Reflection.Assembly[0];
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="virtualPathProviders"></param>
		public WebApplication(int port, IEnumerable<System.Web.Hosting.VirtualPathProvider> virtualPathProviders) : this(port, virtualPathProviders, null)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="port"></param>
		/// <param name="virtualPathProviders"></param>
		/// <param name="referenceAssemblies"></param>
		public WebApplication(int port, IEnumerable<System.Web.Hosting.VirtualPathProvider> virtualPathProviders, IEnumerable<System.Reflection.Assembly> referenceAssemblies)
		{
			_shutdownTokenSource = new System.Threading.CancellationTokenSource();
			_webAppPort = port;
			_virtualPathProviders = virtualPathProviders;
			_referenceAssemblies = referenceAssemblies ?? new System.Reflection.Assembly[0];
			Initialize();
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~WebApplication()
		{
			Dispose(false);
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		protected virtual void Initialize()
		{
			_webApp = Microsoft.Owin.Hosting.WebApp.Start(CreateStartOptions(), Startup);
		}

		/// <summary>
		/// Recupera a configuração do serviço de modelos.
		/// </summary>
		/// <returns></returns>
		protected virtual RazorEngine.Configuration.ITemplateServiceConfiguration GetTemplateServiceConfiguration()
		{
			var configuration = Owin.Razor.RazorViewEngine.GetDefaultConfiguration((name, resolveType, context) => _viewEngine.EngineService.GetKey(name, resolveType, context)) as RazorEngine.Configuration.TemplateServiceConfiguration;
			configuration.ReferenceResolver = new ReferenceResolve();
			configuration.Namespaces.Add("System.Web.Mvc.Html");
			return configuration;
		}

		/// <summary>
		/// Recupera os provedores de caminhos virtuais.
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<System.Web.Hosting.VirtualPathProvider> GetVirtualPathProviders()
		{
			return _virtualPathProviders;
		}

		/// <summary>
		/// Método acionado quando começar uma requisição.
		/// </summary>
		protected virtual void OnBeginRequest()
		{
		}

		/// <summary>
		/// Método acionado quando finalizar uma requisição.
		/// </summary>
		protected virtual void OnEndRequest()
		{
		}

		/// <summary>
		/// Método acionado quando a aplicação iniciar.
		/// </summary>
		protected virtual void OnStart()
		{
		}

		/// <summary>
		/// COnfigura os middlewares padrões.
		/// </summary>
		/// <param name="builder"></param>
		protected virtual void ConfigureDefaultMiddleware(IAppBuilder builder)
		{
			builder.Use(typeof(Owin.Server.Middlewares.HttpRequestMiddleware), this);
		}

		/// <summary>
		/// Configura o JsonConverte
		/// </summary>
		protected virtual void ConfigureJsonConvert()
		{
			var settings = new Newtonsoft.Json.JsonSerializerSettings {
				DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local,
				DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime,
			};
			settings.Converters.Add(new Serializers.FlagEnumJsonConverter());
			Newtonsoft.Json.JsonConvert.DefaultSettings = (() => settings);
		}

		/// <summary>
		/// Configura o SignalrR
		/// </summary>
		/// <param name="builder"></param>
		protected virtual void ConfigureSignalR(IAppBuilder builder)
		{
			GlobalHost.HubPipeline.AddModule(new Colosoft.Web.SignalR.ExceptionLoggingPipelineModule());
			var hubConfiguration = CreateHubConfiguration();
			var jsonSerializer = Newtonsoft.Json.JsonSerializer.Create(Newtonsoft.Json.JsonConvert.DefaultSettings != null ? Newtonsoft.Json.JsonConvert.DefaultSettings() : new Newtonsoft.Json.JsonSerializerSettings {
				DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local,
				DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime,
			});
			GlobalHost.DependencyResolver.Register(typeof(Newtonsoft.Json.JsonSerializer), () => jsonSerializer);
			builder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
			builder.MapSignalR(hubConfiguration);
			builder.Properties["colosoft.owin.WebApplication"] = this;
			builder.Properties["server.Capabilities"] = new Dictionary<string, object>();
			builder.Properties["host.AppName"] = "ColosoftWebApp(" + GetHashCode() + ")";
			builder.Properties["host.OnAppDisposing"] = _shutdownTokenSource.Token;
			ConfigureDefaultMiddleware(builder);
		}

		/// <summary>
		/// Cria o Hub de configuração do SignalR.
		/// </summary>
		/// <returns></returns>
		protected virtual HubConfiguration CreateHubConfiguration()
		{
			var dependencyResolver = new SignalR.DependencyResolver();
			var hubConfiguration = new Microsoft.AspNet.SignalR.HubConfiguration {
				EnableJSONP = true,
				EnableJavaScriptProxies = true,
				EnableDetailedErrors = true,
				Resolver = dependencyResolver
			};
			GlobalHost.DependencyResolver = dependencyResolver;
			return hubConfiguration;
		}

		/// <summary>
		/// Inicializa o ControllerBuilder.
		/// </summary>
		public static void InitializeControllerBuilder()
		{
			var controllerBuilder = System.Web.Mvc.ControllerBuilder.Current;
			if(controllerBuilder != null && !(controllerBuilder.GetControllerFactory() is Owin.Server.ControllerFactory))
				controllerBuilder.SetControllerFactory(new Owin.Server.ControllerFactory());
		}

		/// <summary>
		/// Configura o ambiente da aplicação.
		/// </summary>
		/// <param name="applicationId"></param>
		/// <param name="domainId"></param>
		/// <param name="rootPath"></param>
		public static void ConfigureEnvironment(string applicationId, string domainId, string rootPath)
		{
			AppDomain.CurrentDomain.Load(typeof(Microsoft.Owin.Host.HttpListener.OwinHttpListener).Assembly.GetName());
			if(!System.IO.Directory.Exists(rootPath))
				try
				{
					System.IO.Directory.CreateDirectory(rootPath);
				}
				catch
				{
				}
			var domain = System.Threading.Thread.GetDomain();
			domain.SetData(".appPath", rootPath);
			domain.SetData(".appVPath", "/");
			domain.SetData(".appDomain", "*");
			domain.SetData(".appId", applicationId);
			domain.SetData(".domainId", domainId);
			domain.SetData(".hostingVirtualPath", "/");
			domain.SetData(".hostingInstallDir", rootPath);
			var theRuntimeField = typeof(System.Web.HttpRuntime).GetField("_theRuntime", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			object runtime = null;
			if(theRuntimeField != null)
			{
				runtime = theRuntimeField.GetValue(null);
				typeof(System.Web.HttpRuntime).GetMethod("Init", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(runtime, null);
			}
			var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Temporary ASP.NET Files");
			string str = System.IO.Path.Combine(path, applicationId);
			try
			{
				domain.SetDynamicBase(str);
			}
			catch
			{
			}
			if(theRuntimeField != null)
			{
				typeof(System.Web.HttpRuntime).GetField("_codegenDir", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(runtime, System.Threading.Thread.GetDomain().DynamicDirectory);
			}
			var appIdField = typeof(System.Web.Hosting.HostingEnvironment).GetField("_appId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			System.Web.Hosting.HostingEnvironment hostingEnviroment = null;
			try
			{
				hostingEnviroment = new System.Web.Hosting.HostingEnvironment();
				if(appIdField != null)
					appIdField.SetValue(hostingEnviroment, System.Web.HttpRuntime.AppDomainAppId);
			}
			catch
			{
			}
			var configMapPathField = typeof(System.Web.Hosting.HostingEnvironment).GetField("_configMapPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(configMapPathField != null)
			{
				var mapPath = new WebApplicationMapPath(rootPath);
				configMapPathField.SetValue(hostingEnviroment, mapPath);
			}
			var appVirtualPathField = typeof(System.Web.Hosting.HostingEnvironment).GetField("_appVirtualPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(appVirtualPathField != null)
				appVirtualPathField.SetValue(hostingEnviroment, typeof(System.Web.HttpRuntime).GetProperty("AppDomainAppVirtualPathObject", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null));
			var appPhysicalPathField = typeof(System.Web.Hosting.HostingEnvironment).GetField("_appPhysicalPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(appPhysicalPathField != null)
				appPhysicalPathField.SetValue(hostingEnviroment, typeof(System.Web.HttpRuntime).GetProperty("AppDomainAppPathInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null));
		}

		/// <summary>
		/// Configura o IIS Express.
		/// </summary>
		private static void ConfigureIISExpress()
		{
			var serverConfigType = typeof(System.Web.Configuration.AssemblyInfo).Assembly.GetType("System.Web.Configuration.ServerConfig", false);
			if(serverConfigType != null)
			{
				try
				{
					var iisVersionField = serverConfigType.GetField("s_iisExpressVersion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					if(iisVersionField != null)
						iisVersionField.SetValue(null, "7.0");
					var iisExpressConfigType = serverConfigType.Assembly.GetType("System.Web.Configuration.ExpressServerConfig", false);
					if(iisExpressConfigType != null)
					{
						var instanceField = iisExpressConfigType.GetField("s_instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
						if(instanceField != null)
						{
							var constructor = iisExpressConfigType.GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new Type[] {
								typeof(string)
							}, null);
							if(constructor != null)
							{
								var instance = constructor.Invoke(new object[] {
									"7.0"
								});
								instanceField.SetValue(null, instance);
							}
						}
					}
					var getDefaultDomainInstanceMethod = serverConfigType.GetMethod("GetDefaultDomainInstance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					if(getDefaultDomainInstanceMethod != null)
						getDefaultDomainInstanceMethod.Invoke(null, new object[] {
							"7.0"
						});
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Inicializa a WebApplication.
		/// </summary>
		/// <param name="builder"></param>
		private void Startup(IAppBuilder builder)
		{
			var virtualPathProvider = new Hosting.VirtualFileProviderContainer(GetVirtualPathProviders());
			System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(virtualPathProvider);
			var buildManagerField = typeof(System.Web.Compilation.BuildManager).GetField("_theBuildManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			object buildManager = null;
			if(buildManagerField != null)
				buildManager = buildManagerField.GetValue(null);
			var setType = typeof(System.Web.HttpRuntime).Assembly.GetType("System.Web.Util.StringSet");
			if(setType != null)
			{
				var stringSetConstructor = setType.GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FirstOrDefault();
				if(stringSetConstructor != null)
				{
					var forbiddenTopLevelDirectories = setType.GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).First().Invoke(null);
					var forbiddenTopLevelDirectoriesField = typeof(System.Web.Compilation.BuildManager).GetField("_forbiddenTopLevelDirectories", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
					if(forbiddenTopLevelDirectoriesField != null)
						forbiddenTopLevelDirectoriesField.SetValue(buildManager, forbiddenTopLevelDirectories);
				}
			}
			var preStartInitStageType = typeof(System.Web.HttpRuntime).Assembly.GetType("System.Web.Compilation.PreStartInitStage");
			var preStartInitStageProperty = typeof(System.Web.Compilation.BuildManager).GetProperty("PreStartInitStage", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			if(buildManager != null && preStartInitStageProperty != null && preStartInitStageType != null)
				preStartInitStageProperty.SetValue(buildManager, Enum.Parse(preStartInitStageType, "AfterPreStartInit"));
			if(buildManager != null)
				typeof(System.Web.Compilation.BuildManager).GetMethod("Initialize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(buildManager, null);
			if(buildManager != null && preStartInitStageProperty != null && preStartInitStageType != null)
			{
				var oldValue = preStartInitStageProperty.GetValue(buildManager);
				preStartInitStageProperty.SetValue(buildManager, Enum.Parse(preStartInitStageType, "DuringPreStartInit"));
				OnStart();
				try
				{
					foreach (var i in _referenceAssemblies)
						System.Web.Compilation.BuildManager.AddReferencedAssembly(i);
				}
				finally
				{
					preStartInitStageProperty.SetValue(buildManager, oldValue);
				}
			}
			else
			{
				OnStart();
				foreach (var i in _referenceAssemblies)
					System.Web.Compilation.BuildManager.AddReferencedAssembly(i);
			}
			var executePreAppStartMethod = typeof(System.Web.Compilation.BuildManager).GetMethod("ExecutePreAppStart", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			if(executePreAppStartMethod != null)
			{
				try
				{
					executePreAppStartMethod.Invoke(null, null);
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}
			System.Web.Mvc.ViewEngines.Engines.Clear();
			System.Web.Mvc.ViewEngines.Engines.Add(_viewEngine = new Owin.Razor.RazorViewEngine(_templateServiceConfiguration = GetTemplateServiceConfiguration()));
			System.Web.Mvc.ValueProviderFactories.Factories.Remove(System.Web.Mvc.ValueProviderFactories.Factories.OfType<System.Web.Mvc.JsonValueProviderFactory>().FirstOrDefault());
			System.Web.Mvc.ValueProviderFactories.Factories.Add(new Web.Mvc.JsonNetValueProviderFactory());
			ConfigureJsonConvert();
			ConfigureSignalR(builder);
			InitializeModules();
		}

		/// <summary>
		/// Cria as opções de início.
		/// </summary>
		/// <returns></returns>
		private Microsoft.Owin.Hosting.StartOptions CreateStartOptions()
		{
			var options = new Microsoft.Owin.Hosting.StartOptions();
			options.Port = WebAppPort;
			options.Urls.Add(string.Format("http://*:{0}/", WebAppPort));
			options.Urls.Add(string.Format("http://+:{0}/", WebAppPort));
			return options;
		}

		/// <summary>
		/// Inicializa os módulos.
		/// </summary>
		private void InitializeModules()
		{
			_modules.Add(new Owin.Server.Security.FormsAuthenticationModule());
			foreach (var module in _modules)
				module.Init(this);
		}

		/// <summary>
		/// Evento acionado quando a autenticação for requisitada.
		/// </summary>
		public event EventHandler AuthenticateRequest;

		/// <summary>
		/// Evento acionado no início da requisição.
		/// </summary>
		public event EventHandler BeginRequest;

		/// <summary>
		/// Evento acionado no fim da requisição.
		/// </summary>
		public event EventHandler EndRequest;

		/// <summary>
		/// Contexto associado.
		/// </summary>
		Colosoft.Owin.Server.HttpContext IHttpApplication.Context
		{
			get
			{
				return Colosoft.Owin.Server.HttpContext.Current;
			}
		}

		/// <summary>
		/// Método acionado na autenticação da requisição.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void IHttpApplication.OnAuthenticateRequest(object sender, EventArgs e)
		{
			if(AuthenticateRequest != null)
				AuthenticateRequest(this, e);
		}

		/// <summary>
		/// Método acionado quando começar uma requisição.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void IHttpApplication.OnBeginRequest(object sender, EventArgs e)
		{
			OnBeginRequest();
			if(BeginRequest != null)
				BeginRequest(this, e);
		}

		/// <summary>
		/// Método acionado quando finalizar uma requisição.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void IHttpApplication.OnEndRequest(object sender, EventArgs e)
		{
			OnEndRequest();
			if(EndRequest != null)
				EndRequest(this, e);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_shutdownTokenSource != null)
			{
				try
				{
					_shutdownTokenSource.Cancel();
					_shutdownTokenSource.Dispose();
				}
				catch
				{
				}
			}
			if(_webApp != null)
				_webApp.Dispose();
			_webApp = null;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Customização do provedor de capabilidade do browser.
		/// </summary>
		class CustomHttpCapabilitiesProvider : System.Web.Configuration.HttpCapabilitiesProvider
		{
			private System.Web.Configuration.HttpCapabilitiesProvider _provider;

			private readonly System.Web.HttpBrowserCapabilities _default;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="provider"></param>
			public CustomHttpCapabilitiesProvider(System.Web.Configuration.HttpCapabilitiesProvider provider)
			{
				_provider = provider;
				_default = new System.Web.HttpBrowserCapabilities {
					Capabilities = new System.Collections.Hashtable()
				};
			}

			public override System.Web.HttpBrowserCapabilities GetBrowserCapabilities(System.Web.HttpRequest request)
			{
				if(_provider == null)
					return _default;
				return _provider.GetBrowserCapabilities(request) ?? _default;
			}
		}

		/// <summary>
		/// Implementação do MatpPath para o agente.
		/// </summary>
		[Serializable]
		class WebApplicationMapPath : MarshalByRefObject, System.Web.Configuration.IConfigMapPath
		{
			private string _rootPath;

			private string _virtualPath;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="rootPath"></param>
			/// <param name="virtualPath"></param>
			public WebApplicationMapPath(string rootPath, string virtualPath = null)
			{
				_rootPath = rootPath;
				_virtualPath = virtualPath;
			}

			/// <summary>
			/// Recupera o caminho da aplicação para o caminho informado.
			/// </summary>
			/// <param name="siteID"></param>
			/// <param name="path"></param>
			/// <returns></returns>
			public string GetAppPathForPath(string siteID, string path)
			{
				return path;
			}

			/// <summary>
			/// Recupera o nome e id padrão do site.
			/// </summary>
			/// <param name="siteName"></param>
			/// <param name="siteID"></param>
			public void GetDefaultSiteNameAndID(out string siteName, out string siteID)
			{
				siteName = "Default";
				siteID = "/";
			}

			/// <summary>
			/// Recupera o arquivo de configuração da máquina.
			/// </summary>
			/// <returns></returns>
			public string GetMachineConfigFilename()
			{
				return System.IO.Path.Combine(_rootPath, "machine.config");
			}

			/// <summary>
			/// Recupera o caminho do arquivo de configurão.
			/// </summary>
			/// <param name="siteID"></param>
			/// <param name="path"></param>
			/// <param name="directory"></param>
			/// <param name="baseName"></param>
			public void GetPathConfigFilename(string siteID, string path, out string directory, out string baseName)
			{
				if(path == "/")
					directory = _rootPath;
				else
					directory = System.IO.Path.Combine(_rootPath, path);
				baseName = "Test";
			}

			/// <summary>
			/// Recupera o arquivo de configuração root.
			/// </summary>
			/// <returns></returns>
			public string GetRootWebConfigFilename()
			{
				return System.IO.Path.Combine(_rootPath, "web.config");
			}

			/// <summary>
			/// Mapeia o caminho do site.
			/// </summary>
			/// <param name="siteID"></param>
			/// <param name="path"></param>
			/// <returns></returns>
			public string MapPath(string siteID, string path)
			{
				if(path.StartsWith("/"))
					path = path.Substring(1);
				return System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, path.Replace("/", "\\"));
			}

			/// <summary>
			/// Resolve o argumento do site.
			/// </summary>
			/// <param name="siteArgument"></param>
			/// <param name="siteName"></param>
			/// <param name="siteID"></param>
			public void ResolveSiteArgument(string siteArgument, out string siteName, out string siteID)
			{
				siteName = "Default";
				siteID = "/";
			}
		}

		/// <summary>
		/// Classe responsável por resolver as referencias.
		/// </summary>
		class ReferenceResolve : RazorEngine.Compilation.ReferenceResolver.IReferenceResolver
		{
			/// <summary>
			/// Recupera as referencias.
			/// </summary>
			/// <param name="context"></param>
			/// <param name="includeAssemblies"></param>
			/// <returns></returns>
			public IEnumerable<RazorEngine.Compilation.ReferenceResolver.CompilerReference> GetReferences(RazorEngine.Compilation.TypeContext context, IEnumerable<RazorEngine.Compilation.ReferenceResolver.CompilerReference> includeAssemblies = null)
			{
				var result = new RazorEngine.Compilation.ReferenceResolver.UseCurrentAssembliesReferenceResolver().GetReferences(context, includeAssemblies).ToList();
				result.Add(RazorEngine.Compilation.ReferenceResolver.CompilerReference.From(typeof(System.Web.Mvc.Html.PartialExtensions).Assembly));
				result.Add(RazorEngine.Compilation.ReferenceResolver.CompilerReference.From(typeof(System.Web.Helpers.Json).Assembly));
				result.Add(RazorEngine.Compilation.ReferenceResolver.CompilerReference.From(typeof(System.Web.Mvc.Html.PartialExtensions).Assembly));
				return result;
			}
		}
	}
}
