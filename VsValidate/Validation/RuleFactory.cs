using VsValidate.Validation.Rules;

namespace VsValidate.Validation
{
	internal class RuleFactory : IRuleFactory
	{
		public IRule? Construct(PropertyRuleData data)
		{
			if (string.IsNullOrEmpty(data.Name))
				return null;

			return new PropertyRule(data);
		}

		public IRule? Construct(PackageReferenceRuleData data)
		{
			if (string.IsNullOrEmpty(data.Name))
				return null;

			return new PackageReferenceRule(data);
		}
	}
}