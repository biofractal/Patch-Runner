using Nancy;

namespace Patch_Runner.Modules
{
	public class RootModule : NancyModule
	{
		public RootModule()
		{
			Get["/"] = _ =>
			{
				return Response.AsRedirect("/patches");
			};
		}
	}
}			
