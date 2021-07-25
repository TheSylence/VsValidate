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
			PropertyGroups = ReadPropertyGroups(xml).ToList();
			_itemGroups = ReadItemGroups(xml).ToList();

			PackageReferences = ReadPackageReferences().ToList();
			ProjectReferences = ReadProjectReferences().ToList();

			Sdk = xml.Root?.Attribute("Sdk")?.Value ?? string.Empty;
		}

		public string FileName { get; }

		public ICollection<IPackageReference> PackageReferences { get; }
		public ICollection<IProjectReference> ProjectReferences { get; }

		public ICollection<IPropertyGroup> PropertyGroups { get; }
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

		private static IEnumerable<ItemGroup> ReadItemGroups(XDocument xml)
		{
			var itemGroupElements = xml.Root?.Descendants("ItemGroup") ?? Enumerable.Empty<XElement>();

			return itemGroupElements.Select(i => new ItemGroup(i));
		}

		private IEnumerable<IPackageReference> ReadPackageReferences()
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

		private IEnumerable<IProjectReference> ReadProjectReferences()
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

		private static IEnumerable<IPropertyGroup> ReadPropertyGroups(XDocument xml)
		{
			var groupElements = xml.Root?.Descendants("PropertyGroup") ?? Enumerable.Empty<XElement>();

			return groupElements
				.Select(e => new PropertyGroup(e));
		}

		private readonly List<ItemGroup> _itemGroups;
	}
}