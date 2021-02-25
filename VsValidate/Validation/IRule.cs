using System.Threading.Tasks;
using VsValidate.VisualStudio;

namespace VsValidate.Validation
{
	internal interface IRule
	{
		Task<ValidationResult> Validate(IProject project);
	}
}