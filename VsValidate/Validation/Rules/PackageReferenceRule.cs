using System;
using System.Linq;
using System.Threading.Tasks;
using McSherry.SemanticVersioning;
using McSherry.SemanticVersioning.Ranges;
using VsValidate.VisualStudio;

namespace VsValidate.Validation.Rules
{
	internal class PackageReferenceRule : IRule
	{
		public PackageReferenceRule(PackageReferenceRuleData data)
		{
			_data = data;
		}

		public Task<ValidationResult> Validate(IProject project) => Task.FromResult(ValidateSync(project));

		private ValidationResult ValidateAssets(IPackageReference reference, AssetsProperty property)
		{
			var inProject = Array.Empty<string>();
			switch (property)
			{
				case AssetsProperty.Include:
					inProject = reference.IncludeAssets;
					break;
				case AssetsProperty.Exclude:
					inProject = reference.ExcludeAssets;
					break;
				case AssetsProperty.Private:
					inProject = reference.PrivateAssets;
					break;
			}

			var inConfig = Array.Empty<string>();
			switch (property)
			{
				case AssetsProperty.Include:
					inConfig = _data.IncludeAssets;
					break;
				case AssetsProperty.Exclude:
					inConfig = _data.ExcludeAssets;
					break;
				case AssetsProperty.Private:
					inConfig = _data.PrivateAssets;
					break;
			}

			if (inProject.Any(x => x == "none") && inConfig.Length == 0 ||
			    inProject.Length == 0 && inConfig.Any(x => x == "none"))
				return ValidationResult.Success();

			var missingInProject = inConfig.Except(inProject).ToList();
			if (missingInProject.Any())
			{
				return ValidationResult.Error(
					$"Missing {property}Asset entry in project: {string.Join(",", missingInProject)}");
			}

			var extraInProject = inProject.Except(inConfig).ToList();
			if (extraInProject.Any())
			{
				return ValidationResult.Error(
					$"Extra {property}Asset entry in project: {string.Join(",", extraInProject)}");
			}

			return ValidationResult.Success();
		}

		private ValidationResult ValidateAssets(IPackageReference reference)
		{
			var validation = ValidateAssets(reference, AssetsProperty.Include);
			if (validation.IsError)
				return validation;

			validation = ValidateAssets(reference, AssetsProperty.Exclude);
			if (validation.IsError)
				return validation;

			validation = ValidateAssets(reference, AssetsProperty.Private);
			if (validation.IsError)
				return validation;

			return ValidationResult.Success();
		}

		private ValidationResult ValidateSync(IProject project)
		{
			var reference = project.PackageReferences.FirstOrDefault(r => r.Name == _data.Name);
			if (reference == null)
			{
				return _data.Required
					? ValidationResult.Error($"Missing package {_data.Name}")
					: ValidationResult.Success();
			}

			if (_data.Forbidden)
				return ValidationResult.Error($"Forbidding package {_data.Name}");

			var validation = ValidateVersion(reference);
			if (validation.IsError)
				return validation;

			validation = ValidateAssets(reference);
			if (validation.IsError)
				return validation;

			return ValidationResult.Success();
		}

		private ValidationResult ValidateVersion(IPackageReference reference)
		{
			if (string.IsNullOrEmpty(_data.Version))
				return ValidationResult.Success();

			if (!VersionRange.TryParse(_data.Version, out var range))
				return ValidationResult.Error($"Invalid semantic version range given: {_data.Version}");

			if (!range!.SatisfiedBy(SemanticVersion.Parse(reference.Version)))
			{
				return ValidationResult.Error(
					$"Version of {_data.Name} ({reference.Version}) das not match required version ({_data.Version})");
			}

			return ValidationResult.Success();
		}

		private readonly PackageReferenceRuleData _data;
	}
}