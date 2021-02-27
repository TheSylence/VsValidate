using System.Collections.Generic;
using JetBrains.Annotations;

namespace VsValidate.Validation.Rules
{
	[UsedImplicitly]
	internal class RulesList
	{
		[UsedImplicitly] public List<PackageReferenceRuleData>? Packages { get; set; }
		[UsedImplicitly] public List<PropertyRuleData>? Properties { get; set; }
	}
}