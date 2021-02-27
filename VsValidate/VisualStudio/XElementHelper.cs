using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal static class XElementHelper
	{
		public static ICondition? ReadCondition(XElement element)
		{
			var attr = element.Attribute("Condition");
			return attr != null ? new Condition(attr.Value) : null;
		}
	}
}