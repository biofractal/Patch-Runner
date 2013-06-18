using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nancy.Helper;
using Patch_Runner.Services;
using System;

namespace Patch_Runner.Modules
{
	public class LoginModule : NancyModule
	{
		public LoginModule()
		{
			Get["/login"] = x =>
			{
				ViewBag.Error = this.Request.Query.error.HasValue;
				return View["login"];
			};

			Post["/login"] = x =>
			{
				var thumbKey = ThumbsUpApi.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
				if (thumbKey == null)
				{
					return this.Context.GetRedirect("~/login?error=true");
				}
				return this.LoginAndRedirect(thumbKey.Value);
			};
		}
	}
}			
