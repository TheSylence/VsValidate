using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal class Project : IProject
	{
		public Project(XDocument xml)
			: this(xml, string.Empty)
		{
		}

		public Project(XDocument xml, string fileName)
		{
			FileName = fileName;

			var rootXml = xml.Root ?? new XElement("Project");

			_propertyGroups = ReadPropertyGroups(rootXml).Cast<IPropertyGroup>().ToList();
			_itemGroups = ReadItemGroups(rootXml).ToList();
			ReadChooses(rootXml);

			PackageReferences = ReadPackageReferences().Cast<IPackageReference>().ToList();
			ProjectReferences = ReadProjectReferences().Cast<IProjectReference>().ToList();

			Sdk = rootXml.Attribute("Sdk")?.Value ?? string.Empty;
		}

		public string FileName { get; }

		public ICollection<IPackageReference> PackageReferences { get; }
		public ICollection<IProjectReference> ProjectReferences { get; }
		public ICollection<IPropertyGroup> PropertyGroups => _propertyGroups;
		public string Sdk { get; }

		public IEnumerable<IProperty> FindPropertyByName(string name)
		{
			return PropertyGroups.SelectMany(g => g.Properties).Where(p => p.Name == name);
		}

		public string? PropertyValue(string propertyName)
		{
			foreach (var group in PropertyGroups)
			{
				foreach (var property in group.Properties)
				{
					if (property.Name.Equals(propertyName))
						return property.Value;
				}
			}

			return null;
		}

		private void ReadChooses(XElement xml)
		{
			var chooses = xml.DirectDescendants("Choose");

			foreach (var choose in chooses)
			{
				var whens = choose.DirectDescendants("When");
				foreach (var when in whens)
				{
					var condition = XElementHelper.ReadCondition(when);

					var itemGroups = ReadItemGroups(when);
					foreach (var itemGroup in itemGroups)
					{
						itemGroup.Condition = condition;
						_itemGroups.Add(itemGroup);
					}

					var propertyGroups = ReadPropertyGroups(when);
					foreach (var propertyGroup in propertyGroups)
					{
						propertyGroup.Condition = condition;
						_propertyGroups.Add(propertyGroup);
					}
				}
			}
		}

		private static IEnumerable<ItemGroup> ReadItemGroups(XElement xml)
		{
			var itemGroupElements = xml.DirectDescendants("ItemGroup");

			return itemGroupElements.Select(i => new ItemGroup(i));
		}

		private IEnumerable<PackageReference> ReadPackageReferences()
		{
			foreach (var itemGroup in _itemGroups)
			{
				var references = itemGroup.Descendants("PackageReference");

				foreach (var packageReference in references.Select(r => new PackageReference(r, itemGroup.Condition)))
				{
					yield return packageReference;
				}
			}
		}

		private IEnumerable<ProjectReference> ReadProjectReferences()
		{
			foreach (var itemGroup in _itemGroups)
			{
				var references = itemGroup.Descendants("ProjectReference");

				foreach (var projectReference in references.Select(r => new ProjectReference(r, itemGroup.Condition)))
				{
					yield return projectReference;
				}
			}
		}

		private static IEnumerable<PropertyGroup> ReadPropertyGroups(XElement xml)
		{
			var groupElements = xml.DirectDescendants("PropertyGroup");

			return groupElements
				.Select(e => new PropertyGroup(e));
		}

		private readonly List<IPropertyGroup> _propertyGroups;
		private readonly List<ItemGroup> _itemGroups;
	}

	internal static class XElementExtensions
	{
		public static IEnumerable<XElement> DirectDescendants(this XElement xml, XName name)
		{
			return xml.Descendants(name).Where(d => d.Parent == xml);
		}
	}
}