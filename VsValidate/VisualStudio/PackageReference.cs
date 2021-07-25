using System;
using System.Linq;
using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal class PackageReference : IPackageReference
	{
		public PackageReference(XElement element, ICondition? condition)
		{
			Condition = condition;
			Name = element.Attribute("Include")?.Value ?? string.Empty;
			Version = ReadVersion(element);
			IncludeAssets = ReadAssets(element, "IncludeAssets");
			ExcludeAssets = ReadAssets(element, "ExcludeAssets");
			PrivateAssets = ReadAssets(element, "PrivateAssets");
		}

		public ICondition? Condition { get; }
		public string[] ExcludeAssets { get; }
		public string[] IncludeAssets { get; }
		public string Name { get; }
		public string[] PrivateAssets { get; }
		public string Version { get; }

		private static string[] ReadAssets(XContainer element, string assets)
		{
			var child = element.Descendants(assets).FirstOrDefault();

			return child?.Value.Split(';') ?? Array.Empty<string>();
		}

		private static string ReadVersion(XElement element)
		{
			var attribute = element.Attribute("Version");
			if (attribute != null)
				return attribute.Value;

			var child = element.Descendants("Version").FirstOrDefault();
			return child?.Value ?? string.Empty;
		}
	}
}