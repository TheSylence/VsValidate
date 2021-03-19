namespace VsValidate.VisualStudio
{
	internal interface IProjectReference
	{
		ICondition? Condition { get; }
		string Name { get; }
	}
}