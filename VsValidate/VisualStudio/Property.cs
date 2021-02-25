using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal class Property : IProperty
	{
		public Property(XElement xml, IPropertyGroup group)
		{
			Group = group;

			Name = xml.Name.LocalName;
			Value = xml.Value;
		}

		public IPropertyGroup Group { get; }

		public string Name { get; }
		public string? Value { get; }
	}
}