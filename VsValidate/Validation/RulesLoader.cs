using System;
using System.Collections.Generic;
using System.IO;
using VsValidate.Utilities;
using VsValidate.Validation.Rules;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VsValidate.Validation
{
	internal class RulesLoader
	{
		public RulesLoader(IRuleFactory ruleFactory, IOutput output)
		{
			_ruleFactory = ruleFactory;
			_output = output;
			_deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();
		}

		public async IAsyncEnumerable<IRule> LoadRules(FileInfo configFile)
		{
			_output.Verbose($"Loading configuration from {configFile}");

			RulesList? rulesList;
			try
			{
				var yml = await File.ReadAllTextAsync(configFile.FullName);
				rulesList = _deserializer.Deserialize<RulesList?>(yml);
			}
			catch (Exception ex)
			{
				_output.Error($"Failed to load configuration from {configFile}");
				_output.Error(ex.ToString());
				yield break;
			}

			if (rulesList == null)
				yield break;

			if (rulesList.Packages != null)
			{
				foreach (var data in rulesList.Packages)
				{
					var rule = _ruleFactory.Construct(data);
					if (rule != null)
						yield return rule;
					else
						_output.Warning("Malformed package rule. Ignoring.");
				}
			}

			if (rulesList.Properties != null)
			{
				foreach (var data in rulesList.Properties)
				{
					var rule = _ruleFactory.Construct(data);
					if (rule != null)
						yield return rule;
					else
						_output.Warning("Malformed property rule. Ignoring.");
				}
			}

			if (rulesList.Projects != null)
			{
				foreach (var data in rulesList.Projects)
				{
					var rule = _ruleFactory.Construct(data);
					if (rule != null)
						yield return rule;
					else
						_output.Warning("Malformed project reference rule. Ignoring.");
				}
			}
		}

		private readonly IRuleFactory _ruleFactory;
		private readonly IOutput _output;
		private readonly IDeserializer _deserializer;
	}
}