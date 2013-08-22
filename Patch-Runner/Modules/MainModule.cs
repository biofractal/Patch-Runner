using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using Patch_Runner.Services;
using ThumbsUp.Nancy.FormsAuthentication;

namespace Patch_Runner.Modules
{
	public class MainModule : NancyModule
	{
		public MainModule(IThumbsUpNancyApi thumbsUp)
		{
			this.RequiresAuthentication();

			Get["/patches"] = _ =>
			{
				ViewBag.AllPatches = PatchService.GetAllPatches();
				return View["patches"];
			};

			Get["/logout"] = _ =>
			{
				var thumbKey = ((ThumbsUpNancyUser)Context.CurrentUser).ThumbKey;
				thumbsUp.Logout(thumbKey);
				return this.LogoutAndRedirect("~/");
			};
		}
	}
}