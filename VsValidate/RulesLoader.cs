using System;
using System.Collections.Generic;
using System.IO;

namespace VsValidate
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