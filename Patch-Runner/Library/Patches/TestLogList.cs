using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Patch_Runner
{
	public class TestLogList : IPatch
	{

		public void Run(dynamic caller)
		{
			for (int i = 0; i < 20; i++)
			{
				Thread.Sleep(500);
				var percent = (i + 1) * 5;
				var message = "Processed # " + i;
				ClientLog.Progress(caller, percent, message, isList: true);
			}
		}
	}
}
