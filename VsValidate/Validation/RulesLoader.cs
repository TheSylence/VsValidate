using System;
using System.Collections.Generic;
using System.IO;
using VsValidate.Utilities;

namespace VsValidate.Validation
{
	internal class RulesLoader
	{
		public RulesLoader(IOutput output)
		{
		}

		public IEnumerable<IRule> LoadRules(FileInfo configFile)
		{
			Console.WriteLine($"Configfile: {configFile}");
			yield break;
		}
	}
}