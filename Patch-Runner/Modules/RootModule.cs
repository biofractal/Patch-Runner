using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nancy.Helper;
using Patch_Runner.Services;
using System;

namespace Patch_Runner.Modules
{
	public class RootModule : NancyModule
	{
		public RootModule()
		{
			Get["/"] = _ =>
			{
				return Response.AsRedirect("/patches");
			};
		}
	}
}			
