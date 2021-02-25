namespace VsValidate.Validation
{
	internal class ValidationResult
	{
		private ValidationResult()
		{
			IsError = false;
			Message = string.Empty;
		}

		private ValidationResult(string message)
		{
			IsError = true;
			Message = message;
		}

		public bool IsError { get; }
		public string Message { get; }

		public static ValidationResult Error(string message) => new(message);
		public static ValidationResult Success() => new();
	}
}