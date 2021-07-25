using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using VsValidate.Utilities;
using VsValidate.Validation;
using VsValidate.Validation.Definitions;
using VsValidate.Validation.Rules;
using Xunit;

namespace VsValidate.Tests.Validation
{
	public class RulesLoaderTests
	{
		[Fact]
		public async Task LoadRulesShouldBeEmptyForEmptyFile()
		{
			// Arrange
			using var file = new TempFile();
			var rulesFactory = Substitute.For<IRuleFactory>();
			var output = Substitute.For<IOutput>();
			var sut = new RulesLoader(rulesFactory, output);

			// Act
			var actual = await sut.LoadRules(file).ToListAsync();

			// Assert
			Assert.Empty(actual);
		}

		[Fact]
		public async Task LoadRulesShouldNotLoadUnconstructableRules()
		{
			// Arrange
			var config = new ConfigBuilder()
				.WithProperty(new PropertyRuleData())
				.WithPackage(new PackageReferenceRuleData())
				.WithProjectReference(new ProjectReferenceRuleData())
				.ToString();

			using var file = new TempFile();
			await File.WriteAllTextAsync(file, config);

			var rulesFactory = Substitute.For<IRuleFactory>();
			rulesFactory.Construct(Arg.Any<PackageReferenceRuleData>()).Returns((IRule?) null);
			rulesFactory.Construct(Arg.Any<PropertyRuleData>()).Returns((IRule?) null);
			rulesFactory.Construct(Arg.Any<ProjectReferenceRuleData>()).Returns((IRule?) null);
			var output = Substitute.For<IOutput>();
			var sut = new RulesLoader(rulesFactory, output);

			// Act
			var actual = await sut.LoadRules(file).ToListAsync();

			// Assert
			Assert.Empty(actual);
			rulesFactory.Received(1).Construct(Arg.Any<PropertyRuleData>());
			rulesFactory.Received(1).Construct(Arg.Any<PackageReferenceRuleData>());
			rulesFactory.Received(1).Construct(Arg.Any<ProjectReferenceRuleData>());
		}

		[Fact]
		public async Task LoadRulesShouldNotThrowWhenFileDoesNotExist()
		{
			// Arrange
			var rulesFactory = Substitute.For<IRuleFactory>();
			var output = Substitute.For<IOutput>();
			var sut = new RulesLoader(rulesFactory, output);

			// Act
			var ex = await Record.ExceptionAsync(async () =>
				await sut.LoadRules(new FileInfo("non.existing")).ToListAsync());

			// Assert
			Assert.Null(ex);
			output.ReceivedWithAnyArgs(2).Error(null!);
		}

		[Fact]
		public async Task LoadRulesShouldOnlyReturnPackagesWhenOnlyGivenPackages()
		{
			// Arrange
			var config = new ConfigBuilder()
				.WithPackage("package1", "1.2")
				.WithPackage("package2", "0.3")
				.ToString();

			using var file = new TempFile();
			await File.WriteAllTextAsync(file, config);

			var rulesFactory = Substitute.For<IRuleFactory>();
			rulesFactory.Construct(Arg.Any<PackageReferenceRuleData>()).Returns(Substitute.For<IRule>());
			var output = Substitute.For<IOutput>();
			var sut = new RulesLoader(rulesFactory, output);

			// Act
			var actual = await sut.LoadRules(file).ToListAsync();

			// Assert
			Assert.Equal(2, actual.Count);
			rulesFactory.Received(2).Construct(Arg.Any<PackageReferenceRuleData>());
			rulesFactory.DidNotReceive().Construct(Arg.Any<PropertyRuleData>());
			rulesFactory.DidNotReceive().Construct(Arg.Any<ProjectReferenceRuleData>());
		}

		[Fact]
		public async Task LoadRulesShouldOnlyReturnProjectReferencesWhenOnlyGivenProjectReferences()
		{
			// Arrange
			var config = new ConfigBuilder()
				.WithProjectReference(new ProjectReferenceRuleData {Name = "project1"})
				.WithProjectReference(new ProjectReferenceRuleData {Name = "project2"})
				.ToString();

			using var file = new TempFile();
			await File.WriteAllTextAsync(file, config);

			var rulesFactory = Substitute.For<IRuleFactory>();
			rulesFactory.Construct(Arg.Any<ProjectReferenceRuleData>()).Returns(Substitute.For<IRule>());
			var output = Substitute.For<IOutput>();
			var sut = new RulesLoader(rulesFactory, output);

			// Act
			var actual = await sut.LoadRules(file).ToListAsync();

			// Assert
			Assert.Equal(2, actual.Count);
			rulesFactory.Received(2).Construct(Arg.Any<ProjectReferenceRuleData>());
			rulesFactory.DidNotReceive().Construct(Arg.Any<PackageReferenceRuleData>());
			rulesFactory.DidNotReceive().Construct(Arg.Any<PropertyRuleData>());
		}

		[Fact]
		public async Task LoadRulesShouldOnlyReturnPropertiesWhenOnlyGivenPackages()
		{
			// Arrange
			var config = new ConfigBuilder()
				.WithProperty("prop1", "value1")
				.WithProperty("prop2")
				.ToString();

			using var file = new TempFile();
			await File.WriteAllTextAsync(file, config);

			var rulesFactory = Substitute.For<IRuleFactory>();
			rulesFactory.Construct(Arg.Any<PropertyRuleData>()).Returns(Substitute.For<IRule>());
			var output = Substitute.For<IOutput>();
			var sut = new RulesLoader(rulesFactory, output);

			// Act
			var actual = await sut.LoadRules(file).ToListAsync();

			// Assert
			Assert.Equal(2, actual.Count);
			rulesFactory.Received(2).Construct(Arg.Any<PropertyRuleData>());
			rulesFactory.DidNotReceive().Construct(Arg.Any<PackageReferenceRuleData>());
			rulesFactory.DidNotReceive().Construct(Arg.Any<ProjectReferenceRuleData>());
		}
	}
}