using Nancy;
using Nancy.Helper;
using Nancy.Session;
using Patch_Runner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Patch_Runner.Services
{
	public static class PatchService
	{
		private const string RecentPatchesKey = "RecentPatches";

		public static string[] GetAllPatches(string filter = null)
		{
			var patch = typeof(IPatch);
			return patch.Assembly.GetTypes()
								.Where(t => !t.IsInterface && patch.IsAssignableFrom(t) && (filter == null || t.Name.ToLower().IndexOf(filter.ToLower()) > -1))
								.Select(t => t.Name.CamelHumpToSpace())
								.ToArray();
		}

		public static void Run(dynamic caller, string name)
		{
			var className = name.SpaceToCamelHump();
			var type = typeof(IPatch).Assembly
									.GetTypes()
									.FirstOrDefault(t => !t.IsInterface && t.Name == className);
			if (type == null) throw new Exception("Patch not found");
			var patch = (IPatch)Activator.CreateInstance(type);
			patch.Run(caller);
		}
	}
}
