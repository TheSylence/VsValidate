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
		private readonly IRulesFactory _rulesFactory;
		private readonly IOutput _output;
		private readonly IDeserializer _deserializer;

		public RulesLoader(IRulesFactory rulesFactory, IOutput output)
		{
			_rulesFactory = rulesFactory;
			_output = output;
			_deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();
		}

		public async IAsyncEnumerable<IRule> LoadRules(FileInfo configFile)
		{
			_output.Verbose($"Loading configuration from {configFile}");

			RulesList rulesList;
			try
			{
				var yml = await File.ReadAllTextAsync(configFile.FullName);
				rulesList = _deserializer.Deserialize<RulesList>(yml);
			}
			catch (Exception ex)
			{
				_output.Error($"Failed to load configuration from {configFile}");
				_output.Error(ex.ToString());
				yield break;
			}

			if (rulesList.Packages != null)
			{
				foreach (var data in rulesList.Packages)
				{
					yield return _rulesFactory.Construct(data);
				}
			}

			if (rulesList.Properties != null)
			{
				foreach (var data in rulesList.Properties)
				{
					yield return _rulesFactory.Construct(data);
				}
			}
		}
	}
}