﻿using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal class PackageReference : IPackageReference
	{
		public PackageReference(XElement element, ICondition? condition)
		{
			Condition = condition;
			Name = element.Attribute("Include")?.Value ?? string.Empty;
			Version = element.Attribute("Version")?.Value ?? string.Empty;
		}

		public ICondition? Condition { get; }

		public string Name { get; }
		public string Version { get; }
	}
}