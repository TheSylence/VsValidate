using System.Collections.Generic;

namespace VsValidate.VisualStudio
{
	internal interface IProject
	{
		ICollection<IPackageReference> PackageReferences { get; }
		ICollection<IPropertyGroup> PropertyGroups { get; }

		string Sdk { get; }

		/// <summary>
		///    Returns the value of the first found property with the given name.
		///    Subsequent occurrences of a property with the same name are ignored.
		/// </summary>
		string? PropertyValue(string propertyName);
	}
}