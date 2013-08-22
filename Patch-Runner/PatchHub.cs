using Microsoft.AspNet.SignalR;
using Patch_Runner.Services;
using System;
using System.Diagnostics;

namespace Patch_Runner
{
	public class PatchHub : Hub
	{
		public void RunPatch(string name)
		{
			var caller = Clients.Caller;
			try
			{
				caller.patchStart(name);
				var watch = Stopwatch.StartNew();
				PatchService.Run(caller, name);
				watch.Stop();
				caller.patchSuccess(name, watch.Elapsed.Hours + " hrs " + watch.Elapsed.Minutes + " mins " + watch.Elapsed.Seconds + " secs");
			}
			catch (Exception ex)
			{
				ClientLog.Error(caller, name, ex);
			}
		}
	}
}