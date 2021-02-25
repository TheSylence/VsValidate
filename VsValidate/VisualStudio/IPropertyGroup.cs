using System.Collections.Generic;

namespace VsValidate.VisualStudio
{
	internal interface IPropertyGroup
	{
		ICondition? Condition { get; }

		ICollection<IProperty> Properties { get; }
	}
}