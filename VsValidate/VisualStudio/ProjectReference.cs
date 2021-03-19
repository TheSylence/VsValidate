using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal class ProjectReference : IProjectReference
	{
		public ProjectReference(XElement element, ICondition? condition)
		{
			Condition = condition;
			Name = element.Attribute("Include")?.Value ?? string.Empty;
		}

		public ICondition? Condition { get; }

		public string Name { get; }
	}
}