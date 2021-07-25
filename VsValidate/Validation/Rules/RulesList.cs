using System.Collections.Generic;
using JetBrains.Annotations;
using VsValidate.Validation.Definitions;

namespace VsValidate.Validation.Rules
{
	[UsedImplicitly]
	internal class RulesList
	{
		[UsedImplicitly] public List<PackageReferenceRuleData>? Packages { get; set; }
		[UsedImplicitly] public List<ProjectReferenceRuleData>? Projects { get; set; }
		[UsedImplicitly] public List<PropertyRuleData>? Properties { get; set; }
	}
}