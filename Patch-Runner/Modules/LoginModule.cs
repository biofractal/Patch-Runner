using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nancy.Helper;
using Patch_Runner.Services;
using System;
using ThumbsUp.Client;

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
					ViewBag.ErrorMessage = ThumbsUpApi.GetErrorMessage((int)Request.Query.errorcode);
				}
				return View["login"];
			};

			Post["/login"] = _ =>
			{
				var result = ThumbsUpApi.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
				if (result.Success) return this.LoginAndRedirect(result.Data.ThumbKey.Value);
				return ProcessError(result);
			};

			Get["/sso/{thumbkey}"] = url =>
			{
				Guid thumbKey = new Guid(url.thumbkey);
				var result = ThumbsUpApi.ValidateKey(thumbKey);
				if (result.Success) return this.LoginAndRedirect(thumbKey);
				return ProcessError(result);
			};
		}

		private dynamic ProcessError(ThumbsUpResult result)
		{
			return this.Context.GetRedirect("~/login?errorcode=" + result.Data.ErrorCode);
		}
	}
}
