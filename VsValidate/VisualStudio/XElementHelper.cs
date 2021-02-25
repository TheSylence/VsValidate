using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal static class XElementHelper
	{
		public static ICondition? ReadCondition(XElement element)
		{
			var attr = element.Attribute("Condition");
			if (attr != null)
				return new Condition(attr.Value);

			return null;
		}
	}
}