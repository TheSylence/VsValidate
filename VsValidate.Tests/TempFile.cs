using System;
using System.IO;

namespace VsValidate.Tests
{
	internal class DisposableFileInfo : IDisposable
	{
		public DisposableFileInfo(string fileName)
		{
			_fileInfo = new FileInfo(fileName);
		}

		public void Dispose()
		{
			File.Delete(_fileInfo.FullName);
		}

		public static implicit operator FileInfo(DisposableFileInfo x) => x._fileInfo;
		public static implicit operator string(DisposableFileInfo x) => x._fileInfo.FullName;

		private readonly FileInfo _fileInfo;
	}

	internal class TempFile : DisposableFileInfo
	{
		public TempFile()
			: base(Path.GetTempFileName())
		{
		}
	}
}