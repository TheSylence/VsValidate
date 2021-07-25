using System;
using System.Collections.Generic;
using System.Xml.Linq;
using VsValidate.VisualStudio;

namespace VsValidate.Tests
{
	internal class ProjectBuilder
	{
		public ProjectBuilder(string sdk = "sdk")
		{
			_sdk = sdk;
		}

		public ProjectBuilder BeginChoose()
		{
			_currentChoose = new Choose();
			_chooses.Add(_currentChoose);
			return this;
		}

		public Project Build()
		{
			var doc = BuildXml();
			return new Project(doc, "##TestProject##");
		}

		public XDocument BuildXml()
		{
			var project = new XElement("Project", new XAttribute("Sdk", _sdk));

			foreach (var propertyGroup in _propertyGroups)
			{
				var element = CreatePropertyGroupXml(propertyGroup);
				project.Add(element);
			}

			foreach (var itemGroup in _itemGroups)
			{
				var element = CreateItemGroupXml(itemGroup);
				project.Add(element);
			}

			foreach (var choose in _chooses)
			{
				var element = CreateChooseXml(choose);
				project.Add(element);
			}

			var doc = new XDocument(project);
			return doc;
		}

		public ProjectBuilder EndChoose()
		{
			_currentChoose = null;
			return this;
		}

		public ProjectBuilder WithItemGroup(string? condition = null)
		{
			_currentItemGroup = new ItemGroup(condition);
			if (_currentWhen != null)
				_currentWhen.ItemGroups.Add(_currentItemGroup);
			else
				_itemGroups.Add(_currentItemGroup);

			return this;
		}

		public ProjectBuilder WithPackageReference(string name, string? version,
			bool versionNested = false, string? includeAssets = null, string? excludeAssets = null,
			string? privateAssets = null)
		{
			if (_currentItemGroup == null)
				throw new InvalidOperationException("No ItemGroup created");

			var reference =
				new PackageReference(name, version, versionNested, includeAssets, excludeAssets, privateAssets);
			_currentItemGroup.PackageReferences.Add(reference);
			return this;
		}

		public ProjectBuilder WithProjectReference(string path)
		{
			if (_currentItemGroup == null)
				throw new InvalidOperationException("No ItemGroup created");

			var reference = new ProjectReference(path);
			_currentItemGroup.ProjectReferences.Add(reference);
			return this;
		}

		public ProjectBuilder WithProperty(string name, string value)
		{
			if (_currentPropertyGroup == null)
				throw new InvalidOperationException("No PropertyGroup created");

			var prop = new Property(name, value);
			_currentPropertyGroup.Properties.Add(prop);
			return this;
		}

		public ProjectBuilder WithPropertyGroup(string? condition = null)
		{
			_currentPropertyGroup = new PropertyGroup(condition);

			if (_currentWhen != null)
				_currentWhen.PropertyGroups.Add(_currentPropertyGroup);
			else
				_propertyGroups.Add(_currentPropertyGroup);

			return this;
		}

		public ProjectBuilder WithWhen(string condition)
		{
			if (_currentChoose == null)
				throw new InvalidOperationException("No choose created");

			_currentWhen = new When(condition);
			_currentChoose.Whens.Add(_currentWhen);
			return this;
		}

		private static XElement CreateChooseXml(Choose choose)
		{
			var element = new XElement("Choose");
			foreach (var when in choose.Whens)
			{
				element.Add(CreateWhenXml(when));
			}

			return element;
		}

		private static XElement CreateItemGroupXml(ItemGroup itemGroup)
		{
			var element = new XElement("ItemGroup");
			if (itemGroup.Condition != null)
				element.Add(new XAttribute("Condition", itemGroup.Condition));

			foreach (var packageReference in itemGroup.PackageReferences)
			{
				var xElement = new XElement("PackageReference",
					new XAttribute("Include", packageReference.Name));

				if (packageReference.Version != null)
				{
					if (packageReference.IsVersionNested)
						xElement.Add(new XElement("Version", packageReference.Version));
					else
						xElement.Add(new XAttribute("Version", packageReference.Version));
				}

				if (!string.IsNullOrEmpty(packageReference.IncludeAssets))
					xElement.Add(new XElement("IncludeAssets", packageReference.IncludeAssets));
				if (!string.IsNullOrEmpty(packageReference.ExcludeAssets))
					xElement.Add(new XElement("ExcludeAssets", packageReference.ExcludeAssets));
				if (!string.IsNullOrEmpty(packageReference.PrivateAssets))
					xElement.Add(new XElement("PrivateAssets", packageReference.PrivateAssets));

				element.Add(xElement);
			}

			foreach (var projectReference in itemGroup.ProjectReferences)
			{
				element.Add(new XElement("ProjectReference",
					new XAttribute("Include", projectReference.Path)));
			}

			return element;
		}

		private static XElement CreatePropertyGroupXml(PropertyGroup propertyGroup)
		{
			var element = new XElement("PropertyGroup");
			if (propertyGroup.Condition != null)
				element.Add(new XAttribute("Condition", propertyGroup.Condition));

			foreach (var property in propertyGroup.Properties)
			{
				element.Add(new XElement(property.Name, property.Value));
			}

			return element;
		}

		private static XElement CreateWhenXml(When when)
		{
			var element = new XElement("When");
			element.Add(new XAttribute("Condition", when.Condition));

			foreach (var itemGroup in when.ItemGroups)
			{
				element.Add(CreateItemGroupXml(itemGroup));
			}

			foreach (var propertyGroup in when.PropertyGroups)
			{
				element.Add(CreatePropertyGroupXml(propertyGroup));
			}

			return element;
		}

		private readonly string _sdk;
		private readonly List<PropertyGroup> _propertyGroups = new();
		private readonly List<ItemGroup> _itemGroups = new();
		private readonly List<Choose> _chooses = new();
		private PropertyGroup? _currentPropertyGroup;
		private ItemGroup? _currentItemGroup;
		private When? _currentWhen;
		private Choose? _currentChoose;

		private class Choose
		{
			public List<When> Whens { get; } = new();
		}

		private class When
		{
			public When(string condition)
			{
				Condition = condition;
			}

			public string Condition { get; }

			public List<ItemGroup> ItemGroups { get; } = new();
			public List<PropertyGroup> PropertyGroups { get; } = new();
		}

		private class PropertyGroup
		{
			public PropertyGroup(string? condition)
			{
				Condition = condition;
			}

			public string? Condition { get; }
			public List<Property> Properties { get; } = new();
		}

		private class ItemGroup
		{
			public ItemGroup(string? condition)
			{
				Condition = condition;
			}

			public string? Condition { get; }
			public List<PackageReference> PackageReferences { get; } = new();
			public List<ProjectReference> ProjectReferences { get; } = new();
		}

		private class PackageReference
		{
			public PackageReference(string name, string? version, bool nestVersion, string? includeAssets,
				string? excludeAssets, string? privateAssets)
			{
				Name = name;
				Version = version;
				IsVersionNested = nestVersion;
				IncludeAssets = includeAssets;
				ExcludeAssets = excludeAssets;
				PrivateAssets = privateAssets;
			}

			public string? ExcludeAssets { get; }
			public string? IncludeAssets { get; }
			public bool IsVersionNested { get; }

			public string Name { get; }
			public string? PrivateAssets { get; }
			public string? Version { get; }
		}

		private class ProjectReference
		{
			public ProjectReference(string path)
			{
				Path = path;
			}

			public string Path { get; }
		}

		private class Property
		{
			public Property(string name, string value)
			{
				Name = name;
				Value = value;
			}

			public string Name { get; }
			public string Value { get; }
		}
	}
}