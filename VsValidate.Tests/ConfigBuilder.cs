using System.Collections.Generic;
using System.Linq;
using System.Text;
using VsValidate.Validation.Definitions;

namespace VsValidate.Tests
{
	internal class ConfigBuilder
	{
		public override string ToString()
		{
			var sb = new StringBuilder();

			if (_properties.Any())
			{
				sb.AppendLine("properties:");

				foreach (var property in _properties)
				{
					var required = property.Required ? "true" : "false";
					var forbidden = property.Forbidden ? "true" : "false";

					sb.AppendLine($"  - name: {property.Name}");
					sb.AppendLine($"    required: {required}");
					sb.AppendLine($"    forbidden: {forbidden}");
					if (!string.IsNullOrEmpty(property.Value))
						sb.AppendLine($"    value: {property.Value}");

					sb.AppendLine();
				}
			}

			if (_packageReferences.Any())
			{
				sb.AppendLine("packages:");

				foreach (var package in _packageReferences)
				{
					var forbidden = package.Forbidden ? "true" : "false";
					var required = package.Required ? "true" : "false";

					sb.AppendLine($"  - name: {package.Name}");

					sb.AppendLine($"    required: {required}");
					sb.AppendLine($"    forbidden: {forbidden}");
					if (!string.IsNullOrEmpty(package.Version))
						sb.AppendLine($"    version: {package.Version}");

					if (!package.IncludeAssets.Any())
						sb.AppendLine($"    includeAssets: ['{string.Join("','", package.IncludeAssets)}']");
					if (!package.ExcludeAssets.Any())
						sb.AppendLine($"    excludeAssets: ['{string.Join("','", package.ExcludeAssets)}']");
					if (!package.PrivateAssets.Any())
						sb.AppendLine($"    privateAssets: ['{string.Join("','", package.PrivateAssets)}']");

					sb.AppendLine();
				}
			}

			if (_projectReferences.Any())
			{
				sb.AppendLine("projects:");

				foreach (var reference in _projectReferences)
				{
					var forbidden = reference.Forbidden ? "true" : "false";
					var required = reference.Required ? "true" : "false";

					sb.AppendLine($"  - name: {reference.Name}");
					sb.AppendLine($"    required: {required}");
					sb.AppendLine($"    forbidden: {forbidden}");

					sb.AppendLine();
				}
			}

			return sb.ToString();
		}

		public ConfigBuilder WithPackage(PackageReferenceRuleData data)
		{
			_packageReferences.Add(data);
			return this;
		}

		public ConfigBuilder WithPackage(string name, string? version = null, bool forbidden = false,
			bool required = false) => WithPackage(new PackageReferenceRuleData
		{
			Name = name,
			Version = version,
			Forbidden = forbidden,
			Required = required
		});

		public ConfigBuilder WithProjectReference(ProjectReferenceRuleData data)
		{
			_projectReferences.Add(data);
			return this;
		}

		public ConfigBuilder WithProperty(PropertyRuleData data)
		{
			_properties.Add(data);
			return this;
		}

		public ConfigBuilder WithProperty(string name, string? value = null, bool required = true, bool forbidden = false,
			int? minimumOccurrences = null, int? maximumOccurrences = null) => WithProperty(new PropertyRuleData
		{
			Name = name,
			Value = value,
			Required = required,
			Forbidden = forbidden,
			MaximumOccurrences = maximumOccurrences,
			MinimumOccurrences = minimumOccurrences
		});

		private readonly List<PropertyRuleData> _properties = new();
		private readonly List<PackageReferenceRuleData> _packageReferences = new();
		private readonly List<ProjectReferenceRuleData> _projectReferences = new();
	}
}