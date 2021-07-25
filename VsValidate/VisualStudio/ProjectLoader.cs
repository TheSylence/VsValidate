using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VsValidate.Utilities;

namespace VsValidate.VisualStudio
{
	internal class ProjectLoader : IProjectLoader
	{
		public ProjectLoader(IOutput output)
		{
			_output = output;
		}

		public async IAsyncEnumerable<IProject> LoadProjectsFrom(FileInfo projectFile)
		{
			_output.Verbose($"Loading {projectFile}");

			string fileContent;
			try
			{
				fileContent = await File.ReadAllTextAsync(projectFile.FullName);
			}
			catch (Exception ex)
			{
				_output.Error($"Failed to load project from {projectFile}");
				_output.Error(ex.ToString());
				yield break;
			}

			if (IsSolution(fileContent))
			{
				await foreach (var project in LoadSolution(fileContent, projectFile))
				{
					yield return project;
				}
			}
			else
			{
				var project = LoadProject(fileContent, projectFile);
				if (project != null)
					yield return project;
			}
		}

		private static string? ExtractProjectFilePath(string line, FileInfo projectFile)
		{
			var kvp = line.Split('=', 2);
			if (kvp.Length < 2)
				return null;

			if (IsIgnoredProjectType(kvp[0]))
				return null;

			var parts = kvp[1].Split(',', 3);
			if (parts.Length < 2)
				return null;

			var filePath = parts[1].Trim('\"', ' ');

			return Path.Combine(projectFile.Directory?.FullName ?? string.Empty, filePath);
		}

		private static bool IsIgnoredProjectType(string projectTypePart)
		{
			var ignoredTypes = new[]
			{
				Guid.Parse("2150E333-8FDC-42A3-9474-1A3956D46DE8") // Visual Studio Solution Folder
			};

			var projectTypeStart = projectTypePart.IndexOf("{", StringComparison.Ordinal);
			var projectTypeEnd = projectTypePart.LastIndexOf("}", StringComparison.Ordinal);

			if (projectTypeEnd == -1 || projectTypeStart == -1)
				return true;

			var projectId = projectTypePart[new Range(projectTypeStart + 1, projectTypeEnd)];
			return !Guid.TryParse(projectId, out var id) || ignoredTypes.Contains(id);
		}

		private static bool IsSolution(string fileContent) => fileContent.Contains("Microsoft Visual Studio Solution File");

		private IProject? LoadProject(string content, FileInfo projectFile)
		{
			try
			{
				var doc = XDocument.Parse(content);
				return new Project(doc, projectFile.FullName);
			}
			catch (Exception exception)
			{
				_output.Error($"Failed to load project from {projectFile}");
				_output.Error(exception.ToString());

				return null;
			}
		}

		private async IAsyncEnumerable<IProject> LoadSolution(string content, FileInfo projectFile)
		{
			_output.Verbose($"Loading solution {projectFile}");
			var lines = content.Split(Environment.NewLine);

			foreach (var line in lines)
			{
				var projectPath = ExtractProjectFilePath(line, projectFile);
				if (!string.IsNullOrEmpty(projectPath))
				{
					await foreach (var project in LoadProjectsFrom(new FileInfo(projectPath)))
					{
						yield return project;
					}
				}
			}
		}

		private readonly IOutput _output;
	}
}