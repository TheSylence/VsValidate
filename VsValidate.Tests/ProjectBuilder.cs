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

		public Project Build()
		{
			var doc = BuildXml();
			return new Project(doc);
		}

		public XDocument BuildXml()
		{
			var project = new XElement("Project", new XAttribute("Sdk", _sdk));

			foreach (var propertyGroup in _propertyGroups)
			{
				var element = new XElement("PropertyGroup");
				if (propertyGroup.Condition != null)
					element.Add(new XAttribute("Condition", propertyGroup.Condition));

				foreach (var property in propertyGroup.Properties)
				{
					element.Add(new XElement(property.Name, property.Value));
				}

				project.Add(element);
			}

			foreach (var itemGroup in _itemGroups)
			{
				var element = new XElement("ItemGroup");
				if (itemGroup.Condition != null)
					element.Add(new XAttribute("Condition", itemGroup.Condition));

				foreach (var packageReference in itemGroup.PackageReferences)
				{
					element.Add(new XElement("PackageReference",
						new XAttribute("Include", packageReference.Name),
						new XAttribute("Version", packageReference.Version)));
				}

				project.Add(element);
			}

			var doc = new XDocument(project);
			return doc;
		}

		public ProjectBuilder WithItemGroup(string? condition = null)
		{
			_currentItemGroup = new ItemGroup(condition);
			_itemGroups.Add(_currentItemGroup);
			return this;
		}

		public ProjectBuilder WithPackageReference(string name, string version)
		{
			if (_currentItemGroup == null)
				throw new Exception();

			var reference = new PackageReference(name, version);
			_currentItemGroup.PackageReferences.Add(reference);
			return this;
		}

		public ProjectBuilder WithProperty(string name, string value)
		{
			if (_currentPropertyGroup == null)
				throw new Exception();

			var prop = new Property(name, value);
			_currentPropertyGroup.Properties.Add(prop);
			return this;
		}

		public ProjectBuilder WithPropertyGroup(string? condition = null)
		{
			_currentPropertyGroup = new PropertyGroup(condition);
			_propertyGroups.Add(_currentPropertyGroup);
			return this;
		}

		private readonly string _sdk;
		private readonly List<PropertyGroup> _propertyGroups = new();
		private readonly List<ItemGroup> _itemGroups = new();
		private PropertyGroup? _currentPropertyGroup;
		private ItemGroup? _currentItemGroup;

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
		}

		private class PackageReference
		{
			public PackageReference(string name, string version)
			{
				Name = name;
				Version = version;
			}

			public string Name { get; }
			public string Version { get; }
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