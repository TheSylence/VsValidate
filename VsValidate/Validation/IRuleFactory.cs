using VsValidate.Validation.Rules;

namespace VsValidate.Validation
{
	internal interface IRuleFactory
	{
		IRule? Construct(PropertyRuleData data);
		IRule? Construct(PackageReferenceRuleData data);
	}
}