using System.Collections.Generic;
using System.IO;

namespace VsValidate.VisualStudio
{
	internal interface IProjectLoader
	{
		IAsyncEnumerable<IProject> LoadProjectsFrom(FileInfo projectFile);
	}
}