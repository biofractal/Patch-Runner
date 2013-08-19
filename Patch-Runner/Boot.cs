using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Helper;
using Nancy.TinyIoc;
using Patch_Runner.Services;
using System.Web.Routing;
using ThumbsUp.Client;

namespace Patch_Runner
{
	public class Boot : DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
		{
			base.ApplicationStartup(container, pipelines);

			this.Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
			container.Register<IUserMapper, ThumbsUpApi>();	
			pipelines.AfterRequest += ctx =>
			{
				ctx.CheckForIfNoneMatch();
				ctx.CheckForIfModifiedSince();
			};
			RouteTable.Routes.MapHubs();
		}



		protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
		{
			base.RequestStartup(requestContainer, pipelines, context);	

			pipelines.OnError += (ctx, ex) =>
			{
				Log.Error("Unhandled error on request: " + context.Request.Url, ex);
				return null;
			};

			var formsAuthConfiguration = new FormsAuthenticationConfiguration()
				{
					RedirectUrl = "~/login",
					UserMapper = requestContainer.Resolve<IUserMapper>(),
				};

			FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
		}
	}
}
