using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Helper;
using Nancy.Security;
using Patch_Runner.Services;
using System;

namespace Patch_Runner.Modules
{
	public class MainModule : NancyModule
	{
		public MainModule()
		{
			this.RequiresAuthentication();

			Get["/patches"] = _ =>
			{
				ViewBag.AllPatches = PatchService.GetAllPatches();
				return View["patches"];
			};

			Get["/logout"] = x =>
			{
				var thumbKey = ((ThumbsUpApi.ThumbsUpUser)Context.CurrentUser).ThumbKey;
				ThumbsUpApi.Logout(thumbKey);
				return this.LogoutAndRedirect("~/");
			};
		}
	}
}


