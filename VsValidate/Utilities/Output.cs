using System.CommandLine;
using System.CommandLine.IO;

namespace VsValidate.Utilities
{
	internal class Output : IOutput
	{
		public Output(IConsole console, bool silent, bool verbose)
		{
			_console = console;
			_silent = silent;
			_verbose = verbose;
		}

		public void Error(string message)
		{
			if (!_silent)
				_console.Error.WriteLine(message);
		}

		public void Info(string message)
		{
			if (!_silent)
				_console.Out.WriteLine(message);
		}

		public void Verbose(string message)
		{
			if (_verbose && !_silent)
				_console.Out.WriteLine(message);
		}

		public void Warning(string message)
		{
			if (!_silent)
				_console.Out.WriteLine(message);
		}

		private readonly IConsole _console;
		private readonly bool _silent;
		private readonly bool _verbose;
	}
}