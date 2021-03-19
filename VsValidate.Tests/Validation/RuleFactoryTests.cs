using System;
using VsValidate.Validation;
using VsValidate.Validation.Rules;
using Xunit;

namespace VsValidate.Tests.Validation
{
	public class RuleFactoryTests
	{
		[Fact]
		public void PackageReferenceRuleShouldBeConstructable()
		{
			// Arrange
			var data = new PackageReferenceRuleData
			{
				Name = Guid.NewGuid().ToString()
			};
			var sut = new RuleFactory();

			// Act
			var actual = sut.Construct(data);

			// Assert
			Assert.NotNull(actual);
		}

		[Fact]
		public void PackageReferenceRuleShouldNotBeConstructableWhenNameIsMissing()
		{
			// Arrange
			var data = new PackageReferenceRuleData();
			var sut = new RuleFactory();

			// Act
			var actual = sut.Construct(data);

			// Assert
			Assert.Null(actual);
		}

		[Fact]
		public void ProjectReferenceRuleShouldBeConstructable()
		{
			// Arrange
			var data = new ProjectReferenceRuleData
			{
				Name = Guid.NewGuid().ToString()
			};
			var sut = new RuleFactory();

			// Act
			var actual = sut.Construct(data);

			// Assert
			Assert.NotNull(actual);
		}

		[Fact]
		public void ProjectReferenceShouldNotBeConstructableWhenNameIsMissing()
		{
			// Arrange
			var data = new ProjectReferenceRuleData();
			var sut = new RuleFactory();

			// Act
			var actual = sut.Construct(data);

			// Assert
			Assert.Null(actual);
		}

		[Fact]
		public void PropertyRuleShouldBeConstructable()
		{
			// Arrange
			var data = new PropertyRuleData
			{
				Name = Guid.NewGuid().ToString()
			};
			var sut = new RuleFactory();

			// Act
			var actual = sut.Construct(data);

			// Assert
			Assert.NotNull(actual);
		}

		[Fact]
		public void PropertyRuleShouldNotBeConstructableWhenNameIsMissing()
		{
			// Arrange
			var data = new PropertyRuleData();
			var sut = new RuleFactory();

			// Act
			var actual = sut.Construct(data);

			// Assert
			Assert.Null(actual);
		}
	}
}