using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VsValidate.VisualStudio;

namespace VsValidate.Validation.Rules
{
	internal class ProjectReferenceRule : IRule
	{
		public ProjectReferenceRule(ProjectReferenceRuleData data)
		{
			_data = data;
		}

		public Task<ValidationResult> Validate(IProject project) => Task.FromResult(ValidateSync(project));

		private ValidationResult ValidateSync(IProject project)
		{
			var reference = project.ProjectReferences.FirstOrDefault(r => Path.GetFileName(r.Name) == _data.Name);
			if (reference == null)
			{
				return _data.Required
					? ValidationResult.Error($"Missing project reference {_data.Name}")
					: ValidationResult.Success();
			}

			if (_data.Forbidden)
				return ValidationResult.Error($"Forbidden project reference {_data.Name}");

			return ValidationResult.Success();
		}

		private readonly ProjectReferenceRuleData _data;
	}
}