using System;
using JetBrains.Annotations;

namespace VsValidate.Validation.Rules
{
	[UsedImplicitly]
	internal class PackageReferenceRuleData
	{
		[UsedImplicitly] public string[] ExcludeAssets { get; set; } = Array.Empty<string>();
		[UsedImplicitly] public bool Forbidden { get; set; }
		[UsedImplicitly] public string[] IncludeAssets { get; set; } = Array.Empty<string>();
		[UsedImplicitly] public string Name { get; set; } = string.Empty;
		[UsedImplicitly] public string[] PrivateAssets { get; set; } = Array.Empty<string>();
		[UsedImplicitly] public bool Required { get; set; }
		[UsedImplicitly] public string? Version { get; set; }
	}
}