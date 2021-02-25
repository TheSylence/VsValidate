using System.Collections.Generic;

namespace VsValidate.Validation.Rules
{
	internal class RulesList
	{
		public List<PropertyRuleData>? Properties { get; set; }
		public List<PackageReferenceRuleData>? Packages { get; set; }
	}
}