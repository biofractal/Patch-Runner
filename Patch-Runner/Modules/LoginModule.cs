using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using System;
using ThumbsUp.Client;
using ThumbsUp.Nancy.FormsAuthentication;


namespace Patch_Runner.Modules
{
	public class LoginModule : NancyModule
	{
		public LoginModule(IThumbsUpNancyApi thumbsUp)
		{
			Get["/login"] = _ =>
			{
				if (Request.Query.error.HasValue)
				{
					ViewBag.HasError = true;
					ViewBag.ErrorMessage = thumbsUp.GetErrorMessage((int)Request.Query.errorcode);
				}
				return View["login"];
			};

			Post["/login"] = _ =>
			{
				var result = thumbsUp.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
				if (result.Success) return this.LoginAndRedirect(result.Data.ThumbKey.Value);
				return ProcessError(result);
			};

			Get["/sso/{thumbkey}"] = url =>
			{
				Guid thumbKey = new Guid(url.thumbkey);
				var result = thumbsUp.ValidateKey(thumbKey);
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
