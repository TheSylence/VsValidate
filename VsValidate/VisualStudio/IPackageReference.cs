﻿namespace VsValidate.VisualStudio
{
	internal interface IPackageReference
	{
		ICondition? Condition { get; }
		string Name { get; }
		string Version { get; }
	}
}