namespace VsValidate.VisualStudio
{
	internal interface IPackageReference
	{
		string Name { get; }
		string Version { get; }
		ICondition? Condition { get; }
	}
}