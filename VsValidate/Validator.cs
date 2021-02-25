using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VsValidate
{
	internal class Validator
	{
		public Validator(IEnumerable<IRule> rules, IProjectLoader projectLoader, IOutput output)
		{
			_projectLoader = projectLoader;
			_output = output;
			_rules = rules.ToList();
		}

		public async Task<int> Validate(FileInfo[] projectFiles)
		{
			var results = new List<FileValidationResult>();

			foreach (var projectFile in projectFiles)
			{
				await foreach (var result in ValidateProject(projectFile))
				{
					results.Add(result);
				}
			}

			return Report(results);
		}

		private int Report(IEnumerable<FileValidationResult> results)
		{
			var failed = false;
			var groupedByFile = results.GroupBy(r => r.FileInfo);

			foreach (var fileResults in groupedByFile)
			{
				var errors = new List<string>();

				foreach (var result in fileResults.Select(r => r.Result))
				{
					failed = failed || result.IsError;

					if (result.IsError)
						errors.Add(result.Message);
				}

				if (errors.Any())
				{
					_output.Error($"Validation of {fileResults.Key} failed");
					foreach (var error in errors)
					{
						_output.Error(error);
					}
				}
			}

			return failed ? 1 : 0;
		}

		private async IAsyncEnumerable<FileValidationResult> ValidateProject(FileInfo projectFile)
		{
			var projects = _projectLoader.LoadProjectsFrom(projectFile);
			await foreach (var project in projects)
			{
				foreach (var rule in _rules)
				{
					var validationResult = await rule.Validate(project);
					yield return new FileValidationResult(validationResult, projectFile);
				}
			}
		}

		private readonly IProjectLoader _projectLoader;
		private readonly IOutput _output;
		private readonly List<IRule> _rules;

		private class FileValidationResult
		{
			public FileValidationResult(ValidationResult result, FileInfo fileInfo)
			{
				Result = result;
				FileInfo = fileInfo;
			}

			public FileInfo FileInfo { get; }
			public ValidationResult Result { get; }
		}
	}
}