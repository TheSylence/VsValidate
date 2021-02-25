namespace VsValidate.Validation.Rules
{
	internal class PropertyRuleData
	{
		public bool Forbidden { get; set; }
		public int? MaximumOccurrences { get; set; }
		public int? MinimumOccurrences { get; set; }
		public string Name { get; set; }
		public bool Optional { get; set; }
		public string? Value { get; set; }
	}
}