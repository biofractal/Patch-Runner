using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patch_Runner
{
	public static class ClientLog
	{
		public static dynamic Caller;

		public static void Progress(dynamic caller, int percent, string message = "", bool isList = false)
		{
			caller.patchProgress(percent + "%");
			if (!string.IsNullOrEmpty(message)) ClientLog.Write(caller, message, isList);
		}

		public static void Write(dynamic caller, string message = "", bool isList = false)
		{
			caller.patchLog(message, isList);
		}

		public static void Error(dynamic caller, string name, Exception ex)
		{
			var st = new StackTrace(ex.GetBaseException(), true);
			var frame = st.GetFrames()[0];
			var line = frame.GetFileLineNumber();
			var method = string.Join(".", Path.GetFileNameWithoutExtension(frame.GetFileName()), frame.GetMethod().Name);
			caller.patchError(name, method, line, ex.Message);
		}
	}
}
