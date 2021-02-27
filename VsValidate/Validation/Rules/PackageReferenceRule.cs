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