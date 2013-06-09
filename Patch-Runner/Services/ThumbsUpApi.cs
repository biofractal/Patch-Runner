using Nancy;
using Nancy.Helper;
using Nancy.Authentication.Forms;
using Nancy.Security;
using RestSharp;
using System;
using System.Configuration;
using System.Runtime.Caching;

namespace Patch_Runner.Services
{
	public class ThumbsUpApi : IUserMapper
	{
		private static readonly string ThumbsUpsApplicationId = ConfigurationManager.AppSettings["ThumbsUp.Application.Id"];
		private static readonly string ThumbsUpsUrl = ConfigurationManager.AppSettings["ThumbsUp.Url"];
		private class ThumbsUpResult { public Guid ThumbKey { get; set; } public User User { get; set; } public bool IsValid { get; set; } }


		public IUserIdentity GetUserFromIdentifier(Guid thumbKey, NancyContext context)
		{
			var key = thumbKey.ToString();
			if (!MemoryCache.Default.Contains(key)) return null;

			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("check", Method.POST);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			request.AddParameter("identifier", key);
			var response = client.Execute<ThumbsUpResult>(request);
			if (response.StatusCode.IsOK() && response.Data.IsValid) return (User)MemoryCache.Default.Get(key);
			
			Logout(thumbKey);
			return null;
		}

		public static Guid? ValidateUser(string username, string password)
		{
			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("login", Method.POST);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			request.AddParameter("username", username);
			request.AddParameter("password", password);
			var response = client.Execute<ThumbsUpResult>(request);
			if (!response.StatusCode.IsOK()) return null;

			var thumbKey = response.Data.ThumbKey;
			var user = response.Data.User;
			user.ThumbKey = thumbKey;
			MemoryCache.Default.Add(thumbKey.ToString(), user, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(1, 0, 0) });
			return thumbKey;

		}

		public static void Logout(Guid thumbKey)
		{
			var key = thumbKey.ToString();
			if (!MemoryCache.Default.Contains(key)) return;

			MemoryCache.Default.Remove(key);
			var client = new RestClient(ThumbsUpsUrl);
			var request = new RestRequest("logout", Method.POST);
			request.AddParameter("applicationid", ThumbsUpsApplicationId);
			request.AddParameter("identifier", key);
			client.Execute(request);
		}
	}
}
