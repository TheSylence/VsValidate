using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("VsValidate.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace VsValidate
{
	[ExcludeFromCodeCoverage]
	internal static class Program
	{
		private static Task Main(string[] args)
		{
			var rootCommand = new RootCommand
			{
				new Option<FileInfo>("--config", "Path to a configuration that defines how validation will occur.")
				{
					IsRequired = true,
					AllowMultipleArgumentsPerToken = false,
					Argument = new Argument
					{
						Arity = ArgumentArity.ExactlyOne
					}
				}.ExistingOnly(),
				new Option<FileInfo[]>("--project",
					"Path to a Visual Studio solution or project files that should be validated. Can be specified multiple times")
				{
					IsRequired = true,
					AllowMultipleArgumentsPerToken = true,
					Argument = new Argument
					{
						Arity = ArgumentArity.OneOrMore
					}
				}.ExistingOnly(),
				new Option<bool>("--verbose", description: "Verbose output of progress", getDefaultValue: () => false),
				new Option<bool>("--silent", description: "Don't output anything on stdout", getDefaultValue: () => false)
			};

			rootCommand.Description = "Validator for Visual Studio projects";
			rootCommand.Handler = CommandHandler.Create(
				(FileInfo config, FileInfo[] project, bool verbose, bool silent, IConsole console) =>
				{
					var logger = new Output(console, silent, verbose);

					var rulesLoader = new RulesLoader(logger);
					var rules = rulesLoader.LoadRules(config).ToList();

					var projectLoader = new ProjectLoader(logger);
					var validator = new Validator(rules, projectLoader, logger);

					return validator.Validate(project);
				});

			return rootCommand.InvokeAsync(args);
		}
	}
}