using System;
using System.Threading;

namespace Patch_Runner
{
	public class TestError : IPatch
	{
		public void Run(dynamic caller)
		{
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(500);
				var percent = (i + 1) * 20;
				var message = "Processed # " + i;
				ClientLog.Progress(caller, percent, message);
				if (i == 3) throw new IndexOutOfRangeException();
			}
		}
	}
}
