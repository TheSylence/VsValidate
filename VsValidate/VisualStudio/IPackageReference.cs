namespace VsValidate.VisualStudio
{
	internal interface IPackageReference
	{
		ICondition? Condition { get; }
		string[] ExcludeAssets { get; }
		string[] IncludeAssets { get; }
		string Name { get; }
		string[] PrivateAssets { get; }
		string Version { get; }
	}
}