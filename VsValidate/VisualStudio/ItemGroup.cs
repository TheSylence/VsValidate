using System.Collections.Generic;
using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal class ItemGroup
	{
		public ItemGroup(XElement element)
		{
			_element = element;
			Condition = XElementHelper.ReadCondition(element);
		}

		public ICondition? Condition { get; }

		public IEnumerable<XElement> Descendants(XName? name = null) => _element.Descendants(name);

		private readonly XElement _element;
	}
}