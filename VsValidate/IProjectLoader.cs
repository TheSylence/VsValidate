using System.Collections.Generic;
using System.IO;
using VsValidate.VisualStudio;

namespace VsValidate
{
	internal interface IProjectLoader
	{
		IAsyncEnumerable<IProject> LoadProjectsFrom(FileInfo projectFile);
	}
}