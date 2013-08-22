using System.Text.RegularExpressions;

namespace Patch_Runner
{
	public static class Extensions
	{
		public static string CamelHumpToSpace(this string candidate)
		{
			return Regex.Replace(candidate, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
		}

		public static string SpaceToCamelHump(this string candidate)
		{
			return candidate.Replace(" ", "");
		}


	}
}