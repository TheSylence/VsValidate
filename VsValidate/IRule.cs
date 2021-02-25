using System.Threading.Tasks;
using VsValidate.VisualStudio;

namespace VsValidate
{
	internal interface IRule
	{
		Task<ValidationResult> Validate(IProject project);
	}
}