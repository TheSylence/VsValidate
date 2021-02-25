namespace VsValidate.Validation.Rules
{
	internal class PackageReferenceRuleData
	{
		public string Name { get; set; }
		public string? Version { get; set; }
		public bool Required { get; set; }
		public bool Forbidden { get; set; }
	}
}