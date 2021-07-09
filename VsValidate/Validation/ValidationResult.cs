using VsValidate.VisualStudio;

namespace VsValidate.Validation
{
	internal class ValidationResult
	{
		private ValidationResult()
		{
			IsError = false;
			_message = string.Empty;
			_project = null;
		}

		private ValidationResult(string message, IProject? project)
		{
			IsError = true;
			_message = message;
			_project = project;
		}

		private readonly string _message;
		private readonly IProject? _project;

		public bool IsError { get; }

		public string Message => _project != null ? $"[{_project.FileName}] {_message}" : _message;

		public static ValidationResult Error(IProject project, string message) => new(message, project);
		public static ValidationResult Success() => new();
	}
}