using JetBrains.Annotations;

namespace VsValidate.Validation.Rules
{
	[UsedImplicitly]
	internal class PropertyRuleData
	{
		[UsedImplicitly] public bool Forbidden { get; set; }
		[UsedImplicitly] public int? MaximumOccurrences { get; set; }
		[UsedImplicitly] public int? MinimumOccurrences { get; set; }
		[UsedImplicitly] public string Name { get; set; } = string.Empty;
		[UsedImplicitly] public bool Required { get; set; }
		[UsedImplicitly] public string? Value { get; set; }
	}
}