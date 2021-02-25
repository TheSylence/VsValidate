using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace VsValidate.Tests
{
	public class ProjectLoaderTests
	{
		private static DisposableFileInfo CreateTempFile(string content)
		{
			var fileName = Path.GetTempFileName();
			File.WriteAllText(fileName, content);

			return new DisposableFileInfo(fileName);
		}

		private class DisposableFileInfo : IDisposable
		{
			public DisposableFileInfo(string fileName)
			{
				_fileInfo = new FileInfo(fileName);
			}

			public void Dispose()
			{
				File.Delete(_fileInfo.FullName);
			}

			public static implicit operator FileInfo(DisposableFileInfo x) => x._fileInfo;

			private readonly FileInfo _fileInfo;
		}

		[Fact]
		public async Task LoadProjectsFromShouldReturnAllProjectsFromSolution()
		{
			// Arrange
			var logger = Substitute.For<IOutput>();
			var sut = new ProjectLoader(logger);

			var solutionFile = new FileInfo("_TestData/TestSolutions/Solution.txt");

			// Act
			var actual = await sut.LoadProjectsFrom(solutionFile).ToListAsync();

			// Assert
			Assert.Equal(2, actual.Count);
		}

		[Fact]
		public async Task LoadProjectsFromShouldReturnEmptyWhenProjectFromSolutionDoesNotExist()
		{
			// Arrange
			var logger = Substitute.For<IOutput>();
			var sut = new ProjectLoader(logger);

			var solutionFile = new FileInfo("_TestData/TestSolutions/NonExistingProjectSolution.txt");

			// Act
			var actual = await sut.LoadProjectsFrom(solutionFile).ToListAsync();

			// Assert
			Assert.Empty(actual);
		}

		[Fact]
		public async Task LoadProjectsFromShouldReturnEmptyWhenProjectIsInvalidXml()
		{
			// Arrange
			using var tmpFile = CreateTempFile("<project><invalid-tag></project>");

			var logger = Substitute.For<IOutput>();
			var sut = new ProjectLoader(logger);

			// Act
			var actual = await sut.LoadProjectsFrom(tmpFile).ToListAsync();

			// Assert
			Assert.Empty(actual);
		}

		[Fact]
		public async Task LoadProjectsFromShouldReturnEmptyWhenSolutionIsEmpty()
		{
			// Arrange
			var logger = Substitute.For<IOutput>();
			var sut = new ProjectLoader(logger);

			var solutionFile = new FileInfo("_TestData/TestSolutions/EmptySolution.txt");

			// Act
			var actual = await sut.LoadProjectsFrom(solutionFile).ToListAsync();

			// Assert
			Assert.Empty(actual);
		}

		[Fact]
		public async Task LoadProjectsFromShouldReturnSingleProjectWhenCsProjFileIsGiven()
		{
			// Arrange
			using var tmpFile = CreateTempFile("<Project Sdk=\"Microsoft.NET.Sdk\"></Project>");

			var logger = Substitute.For<IOutput>();
			var sut = new ProjectLoader(logger);

			// Act
			var actual = await sut.LoadProjectsFrom(tmpFile).ToListAsync();

			// Assert
			Assert.Single(actual);
		}
	}
}