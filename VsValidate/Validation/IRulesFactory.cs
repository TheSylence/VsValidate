using VsValidate.Validation.Rules;

namespace VsValidate.Validation
{
	internal interface IRulesFactory
	{
		IRule Construct(PropertyRuleData data);
		IRule Construct(PackageReferenceRuleData data);
	}
}