#region Using

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using AutoDalLib;
using AutoDalLib.Utility;
using CsvHelper;
using System.Web;

#endregion

namespace Patch_Runner
{
	public class FixInvalidHttpsUrls : IPatch
	{
		public class CsvData
		{
			public string Page { get; set; }
			private string referrer;
			public string Referrer {
				get { return referrer; }
				set { referrer = HttpUtility.UrlDecode(value); }
			}
			public string UrlName { get { return Path.GetFileNameWithoutExtension(this.Referrer); } }
			public string Url { get { return "~" +  HttpUtility.UrlDecode(new Uri(this.Referrer).AbsolutePath); } }
			public string ReplaceMe { get { return this.Page.Replace("https:", "").Replace("'", "''").TrimEnd('/'); } }
			public string Valid { get { return this.Page.Replace("https:", "http:").Replace("'", "''").Replace("///", "//").TrimEnd('/'); } }
		}

		public void Run(dynamic caller)
		{
			var sourceTables = new []{"ContentItem", "TopicalItem"};
			var csv = new CsvReader(new StreamReader(HttpContext.Current.Server.MapPath("/Library/Resources/links-for-staging.csv")));
			var spyderUrls = csv.GetRecords<CsvData>().ToArray();
			var count = spyderUrls.Length;
			var increment = 100 / (double)count;
			var progress = 0d;
			var index = 0;
			foreach (var spyderUrl in spyderUrls)
			{
				progress += increment;
				ClientLog.Progress(caller, (int)progress, "Replacing invalid https url # " + index++);
				Guid? id = null;
				foreach(var source in sourceTables)
				{
					id = (Guid?)AutoDal.ExecuteScalar(string.Format("select [OriginalContentId] from [{0}] where [Url] = '{1}'", source, spyderUrl.Url));
					if (id != null) break;
				}
				if (id == null) continue;
				var table = StormId.Decode(id).Table;
				var replaceInvalid = string.Format("update [{0}] set [Xhtml] = replace(cast(Xhtml as nvarchar(max)),'{1}', '{2}') where [Id] = '{3}'", table, spyderUrl.ReplaceMe, spyderUrl.Valid, id);
				var fixDoubles = string.Format("update [{0}] set [Xhtml] = replace(cast(Xhtml as nvarchar(max)),'http:http:', 'http:') where [Id] = '{1}'", table, id);
				AutoDal.ExecuteNonQuery(replaceInvalid);
				AutoDal.ExecuteNonQuery(fixDoubles);
			}
		}
	}
}