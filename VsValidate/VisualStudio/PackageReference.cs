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
		}

		public ICondition? Condition { get; }

		public string Name { get; }
		public string Version { get; }

		private static string ReadVersion(XElement element)
		{
			var attribute = element.Attribute("Version");
			if (attribute != null)
				return attribute.Value;

			var child = element.Descendants("Version").FirstOrDefault();
			if (child != null)
				return child.Value;

			return string.Empty;
		}
	}
}