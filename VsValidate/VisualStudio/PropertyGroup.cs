﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace VsValidate.VisualStudio
{
	internal class PropertyGroup : IPropertyGroup
	{
		public PropertyGroup(XElement groupElement)
		{
			Condition = XElementHelper.ReadCondition(groupElement);
			Properties = ReadProperties(groupElement).ToList();
		}

		public ICondition? Condition { get; }
		public ICollection<IProperty> Properties { get; }

		private IEnumerable<IProperty> ReadProperties(XElement xml)
		{
			var propertyElements = xml.Descendants();
			return propertyElements.Select(e => new Property(e, this));
		}
	}
}