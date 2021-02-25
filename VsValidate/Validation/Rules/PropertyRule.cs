using System.Linq;
using System.Threading.Tasks;
using VsValidate.VisualStudio;

namespace VsValidate.Validation.Rules
{
	internal class PropertyRule : IRule
	{
		public PropertyRule(PropertyRuleData data)
		{
			_data = data;
		}

		public Task<ValidationResult> Validate(IProject project) => Task.FromResult(ValidateSync(project));

		private ValidationResult ValidateSync(IProject project)
		{
			var properties = project.FindPropertyByName(_data.Name).ToList();
			if (properties.Count > _data.MaximumOccurrences)
			{
				return ValidationResult.Error(
					$"Property {_data.Name} was specified {properties.Count} times but only {_data.MaximumOccurrences} occurrences are allowed.");
			}

			if (properties.Count < _data.MinimumOccurrences)
			{
				return ValidationResult.Error(
					$"Property {_data.Name} was specified {properties.Count} times but is required {_data.MaximumOccurrences} times.");
			}

			if (!properties.Any() && !_data.Optional)
				return ValidationResult.Error($"Property {_data.Name} not specified");

			if (properties.Any() && _data.Forbidden)
				return ValidationResult.Error($"Forbidden property {_data.Name} specified");

			foreach (var property in properties)
			{
				if (_data.Value != property.Value)
				{
					return ValidationResult.Error(
						$"Property {_data.Name} has invalid value ({property.Value} instead of {_data.Value})");
				}
			}

			return ValidationResult.Success();
		}

		private readonly PropertyRuleData _data;
	}
}