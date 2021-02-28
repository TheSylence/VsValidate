using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using VsValidate.Utilities;
using VsValidate.Validation;
using VsValidate.Validation.Rules;
using VsValidate.VisualStudio;
using Xunit;

namespace VsValidate.Tests.Validation
{
	public class ValidatorTests
	{
		private static IProjectLoader ConstructProjectLoader(IProject? project = null)
		{
			var projectLoader = Substitute.For<IProjectLoader>();
			projectLoader.LoadProjectsFrom(Arg.Any<FileInfo>())
				.Returns(new[] {project ?? Substitute.For<IProject>()}.ToAsyncEnumerable());
			return projectLoader;
		}

		private static IRule ConstructSuccessRule()
		{
			var rule = Substitute.For<IRule>();
			rule.Validate(Arg.Any<IProject>()).Returns(ValidationResult.Success());
			return rule;
		}

		private static IRule ConstructErrorRule(string message)
		{
			var rule = Substitute.For<IRule>();
			rule.Validate(Arg.Any<IProject>()).Returns(ValidationResult.Error(message));
			return rule;
		}

		[Fact]
		public async Task ValidateShouldCallEveryRuleForEveryProject()
		{
			// Arrange
			var output = Substitute.For<IOutput>();
			var projectLoader = ConstructProjectLoader();
			var rule1 = ConstructSuccessRule();
			var rule2 = ConstructSuccessRule();
			var rule3 = ConstructSuccessRule();
			var rules = new[]
			{
				rule1, rule2, rule3
			};

			var sut = new Validator(rules, projectLoader, output);

			var projectFiles = new[]
			{
				new FileInfo("project1"),
				new FileInfo("project2"),
				new FileInfo("project3")
			};

			// Act
			var actual = await sut.Validate(projectFiles);

			// Assert
			Assert.Equal(0, actual);

			await rule1.Received(3).Validate(Arg.Any<IProject>());
			await rule2.Received(3).Validate(Arg.Any<IProject>());
			await rule3.Received(3).Validate(Arg.Any<IProject>());
		}

		[Fact]
		public async Task ValidateShouldNotPrintWhenRuleSucceeds()
		{
			// Arrange
			var output = Substitute.For<IOutput>();
			var project = Substitute.For<IProject>();
			var projectLoader = ConstructProjectLoader(project);

			var rules = new[]
			{
				ConstructSuccessRule()
			};

			var sut = new Validator(rules, projectLoader, output);

			// Act
			var actual = await sut.Validate(new[] {new FileInfo("file-name")});

			// Assert
			Assert.Equal(0, actual);

			output.DidNotReceiveWithAnyArgs().Error(null!);
		}

		[Fact]
		public async Task ValidateShouldPrintSingleErrorMessage()
		{
			// Arrange
			var output = Substitute.For<IOutput>();
			var project = Substitute.For<IProject>();
			var projectLoader = ConstructProjectLoader(project);

			var rules = new[]
			{
				ConstructErrorRule("error-message")
			};

			var sut = new Validator(rules, projectLoader, output);

			// Act
			var actual = await sut.Validate(new[] {new FileInfo("file-name")});

			// Assert
			Assert.Equal(1, actual);

			output.Received(1).Error(Arg.Is<string>(str => str.Contains("error-message")));
			output.Received(1).Error(Arg.Is<string>(str => str.Contains("file-name")));
		}

		[Fact]
		public async Task ValidateShouldPrintAllErrorMessages()
		{
			// Arrange
			var output = Substitute.For<IOutput>();
			var project = Substitute.For<IProject>();
			var projectLoader = ConstructProjectLoader(project);

			var rules = new[]
			{
				ConstructErrorRule("error-message-1"),
				ConstructErrorRule("error-message-2"),
				ConstructErrorRule("error-message-3")
			};

			var sut = new Validator(rules, projectLoader, output);


			// Act
			var actual = await sut.Validate(new[] {new FileInfo("file-name")});

			// Assert
			Assert.Equal(1, actual);

			output.Received(3).Error(Arg.Is<string>(str => str.Contains("error-message")));
			output.Received(1).Error(Arg.Is<string>(str => str.Contains("file-name")));
		}
	}
}