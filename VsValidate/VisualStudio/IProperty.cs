namespace VsValidate.VisualStudio
{
	internal interface IProperty
	{
		IPropertyGroup Group { get; }
		string Name { get; }
		string? Value { get; }
	}
}