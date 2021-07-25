using System.Threading.Tasks;
using VsValidate.Validation.Definitions;
using VsValidate.Validation.Rules;
using Xunit;

namespace VsValidate.Tests.Validation.Rules
{
	public class PackageReferenceRuleTests
	{
		[Theory]
		[InlineData(AssetsProperty.Include)]
		[InlineData(AssetsProperty.Exclude)]
		[InlineData(AssetsProperty.Private)]
		public async Task ValidateShouldFailWhenAssetsDoesNotMatch(AssetsProperty property)
		{
			// Arrange
			var projectBuilder = new ProjectBuilder()
				.WithItemGroup();

			var data = new PackageReferenceRuleData
			{
				Name = "Package"
			};

			if (property == AssetsProperty.Include)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, "build");
				data.IncludeAssets = new[] {"compile"};
			}
			else if (property == AssetsProperty.Exclude)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, "build");
				data.ExcludeAssets = new[] {"compile"};
			}
			else if (property == AssetsProperty.Private)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, null, "build");
				data.PrivateAssets = new[] {"compile"};
			}

			var project = projectBuilder.Build();
			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Theory]
		[InlineData(AssetsProperty.Include)]
		[InlineData(AssetsProperty.Exclude)]
		[InlineData(AssetsProperty.Private)]
		public async Task ValidateShouldFailWhenAssetsInConfigDoesNotContainAllValues(AssetsProperty property)
		{
			// Arrange
			var projectBuilder = new ProjectBuilder()
				.WithItemGroup();

			var data = new PackageReferenceRuleData
			{
				Name = "Package"
			};

			if (property == AssetsProperty.Include)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, "build;compile");
				data.IncludeAssets = new[] {"compile"};
			}
			else if (property == AssetsProperty.Exclude)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, "build;compile");
				data.ExcludeAssets = new[] {"compile"};
			}
			else if (property == AssetsProperty.Private)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, null, "build;compile");
				data.PrivateAssets = new[] {"compile"};
			}

			var project = projectBuilder.Build();
			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Theory]
		[InlineData(AssetsProperty.Include)]
		[InlineData(AssetsProperty.Exclude)]
		[InlineData(AssetsProperty.Private)]
		public async Task ValidateShouldFailWhenAssetsInProjectDoesNotContainAllValues(AssetsProperty property)
		{
			// Arrange
			var projectBuilder = new ProjectBuilder()
				.WithItemGroup();

			var data = new PackageReferenceRuleData
			{
				Name = "Package"
			};

			if (property == AssetsProperty.Include)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, "build");
				data.IncludeAssets = new[] {"compile", "build"};
			}
			else if (property == AssetsProperty.Exclude)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, "build");
				data.ExcludeAssets = new[] {"compile", "build"};
			}
			else if (property == AssetsProperty.Private)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, null, "build");
				data.PrivateAssets = new[] {"compile", "build"};
			}

			var project = projectBuilder.Build();
			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenForbiddenReferenceExists()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.0")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Forbidden = true,
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenPackageVersionIsWrong()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.2.3")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Version = "^2.0.0",
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenRequiredReferenceIsMissing()
		{
			// Arrange
			var project = new ProjectBuilder().Build();

			var data = new PackageReferenceRuleData
			{
				Required = true,
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldFailWhenVersionIsInvalidSemanticVersion()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.2.3")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Version = "^1.0",
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.True(actual.IsError);
		}

		[Theory]
		[InlineData(AssetsProperty.Include)]
		[InlineData(AssetsProperty.Exclude)]
		[InlineData(AssetsProperty.Private)]
		public async Task ValidateShouldSucceedWhenAssetValueNoneIsSpecifiedInConfig(AssetsProperty property)
		{
			// Arrange
			var projectBuilder = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.0");

			var data = new PackageReferenceRuleData
			{
				Name = "Package"
			};

			if (property == AssetsProperty.Include)
				data.IncludeAssets = new[] {"none"};
			else if (property == AssetsProperty.Exclude)
				data.ExcludeAssets = new[] {"none"};
			else if (property == AssetsProperty.Private)
				data.PrivateAssets = new[] {"none"};

			var project = projectBuilder.Build();
			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}

		[Theory]
		[InlineData(AssetsProperty.Include)]
		[InlineData(AssetsProperty.Exclude)]
		[InlineData(AssetsProperty.Private)]
		public async Task ValidateShouldSucceedWhenMultipleAssetsMatch(AssetsProperty property)
		{
			// Arrange
			var projectBuilder = new ProjectBuilder()
				.WithItemGroup();

			var data = new PackageReferenceRuleData
			{
				Name = "Package"
			};

			if (property == AssetsProperty.Include)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, "build;compile");
				data.IncludeAssets = new[] {"compile", "build"};
			}
			else if (property == AssetsProperty.Exclude)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, "build;compile");
				data.ExcludeAssets = new[] {"compile", "build"};
			}
			else if (property == AssetsProperty.Private)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, null, "build;compile");
				data.PrivateAssets = new[] {"compile", "build"};
			}

			var project = projectBuilder.Build();
			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenOptionalPackageIsReferenced()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.2.3")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Name = "Package",
				Required = false
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenOptionalReferenceIsMissing()
		{
			// Arrange
			var project = new ProjectBuilder().Build();

			var data = new PackageReferenceRuleData
			{
				Required = false,
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}

		[Fact]
		public async Task ValidateShouldSucceedWhenPackageHasCorrectVersion()
		{
			// Arrange
			var project = new ProjectBuilder()
				.WithItemGroup()
				.WithPackageReference("Package", "1.2.3")
				.Build();

			var data = new PackageReferenceRuleData
			{
				Version = "^1.0.0",
				Name = "Package"
			};

			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}

		[Theory]
		[InlineData(AssetsProperty.Include)]
		[InlineData(AssetsProperty.Exclude)]
		[InlineData(AssetsProperty.Private)]
		public async Task ValidateShouldSucceedWhenSingleAssetMatches(AssetsProperty property)
		{
			// Arrange
			var projectBuilder = new ProjectBuilder()
				.WithItemGroup();

			var data = new PackageReferenceRuleData
			{
				Name = "Package"
			};

			if (property == AssetsProperty.Include)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, "build");
				data.IncludeAssets = new[] {"build"};
			}
			else if (property == AssetsProperty.Exclude)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, "build");
				data.ExcludeAssets = new[] {"build"};
			}
			else if (property == AssetsProperty.Private)
			{
				projectBuilder = projectBuilder.WithPackageReference("Package", "1.0", false, null, null, "build");
				data.PrivateAssets = new[] {"build"};
			}

			var project = projectBuilder.Build();
			var sut = new PackageReferenceRule(data);

			// Act
			var actual = await sut.Validate(project);

			// Assert
			Assert.False(actual.IsError);
		}
	}
}