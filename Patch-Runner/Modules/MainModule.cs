using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Helper;
using Nancy.Security;
using Patch_Runner.Services;
using System;
using ThumbsUp.Helper;

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

			Get["/logout"] = _ =>
			{
				var thumbKey = ((ThumbsUpUser)Context.CurrentUser).ThumbKey;
				ThumbsUpApi.Logout(thumbKey);
				return this.LogoutAndRedirect("~/");
			};
		}
	}
}


