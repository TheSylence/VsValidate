using VsValidate.Validation.Rules;

namespace VsValidate.Validation
{
	internal class RulesFactory : IRulesFactory
	{
		public IRule Construct(PropertyRuleData data) => new PropertyRule(data);

		public IRule Construct(PackageReferenceRuleData data) => new PackageReferenceRule(data);
	}
}