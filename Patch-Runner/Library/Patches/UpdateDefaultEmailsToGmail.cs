using AutoDalLib;
using GoodPractice.ClassLibrary.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patch_Runner.Library.Patches
{
	public class UpdateDefaultEmailsToGmail:IPatch
	{
		public void Run(dynamic caller)
		{
			ClientLog.Progress(caller, 0, "Gathering defualt emails to update");
			var brandings = AutoDal.GetFilteredList<Branding>("EmailFromAddress = 'admin@goodpractice.net' or EmailFromName = 'goodpractice.net'");
			var count = brandings.Count;
			var increment = 100 / (double)count;
			var progress = (double)1;
			var index = 0;
			foreach (var branding in brandings)
			{
				progress += increment;
				ClientLog.Progress(caller, (int)progress, "Replacing default email #" + index++);
				branding.EmailFromAddress = "support@goodpractice.com";
				branding.EmailFromName = "goodpractice.com";
				branding.Save();
			}			
		}
	}
}