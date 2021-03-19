﻿using JetBrains.Annotations;

namespace VsValidate.Validation.Rules
{
	[UsedImplicitly]
	internal class ProjectReferenceRuleData
	{
		[UsedImplicitly] public bool Forbidden { get; set; }
		[UsedImplicitly] public string Name { get; set; } = string.Empty;
		[UsedImplicitly] public bool Required { get; set; }
	}
}