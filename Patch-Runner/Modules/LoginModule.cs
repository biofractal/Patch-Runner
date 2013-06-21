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
			Get["/login"] = _ =>
			{
				if (Request.Query.error.HasValue)
				{
					ViewBag.HasError = true;
					ViewBag.ErrorNumber = Request.Query.error;
				}
				return View["login"];
			};

			Post["/login"] = _ =>
			{
				var thumbKey = ThumbsUpApi.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
				if (thumbKey == null)
				{
					return this.Context.GetRedirect("~/login?error=1");
				}
				return this.LoginAndRedirect(thumbKey.Value);
			};

			Get["/sso/{thumbkey}"] = url =>
			{
				Guid thumbKey;
				var success = Guid.TryParse(url.thumbkey, out thumbKey);
				if (success) success = ThumbsUpApi.ValidateKey(thumbKey);
				if (!success)
				{
					return this.Context.GetRedirect("~/login?error=2");
				}
				return this.LoginAndRedirect(thumbKey);
			};
		}
	}
}
