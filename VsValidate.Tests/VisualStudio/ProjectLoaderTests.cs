﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using VsValidate.Utilities;
using VsValidate.VisualStudio;
using Xunit;

namespace VsValidate.Tests.VisualStudio
{
	public class ProjectLoaderTests
	{
		private static DisposableFileInfo CreateTempFile(string content)
		{
			var fileName = Path.GetTempFileName();
			File.WriteAllText(fileName, content);

			return new DisposableFileInfo(fileName);
		}

		[Fact]
		public async Task LoadProjectsFromShouldReturnAllProjectsFromSolution()
		{
			// Arrange
			var logger = Substitute.For<IOutput>();
			var sut = new ProjectLoader(logger);

			var solutionFile = new FileInfo("TestData/Solution.txt");

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

			var solutionFile = new FileInfo("TestData/NonExistingProjectSolution.txt");

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

			var solutionFile = new FileInfo("TestData/EmptySolution.txt");

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

		[Fact]
		public async Task LoadProjectsShouldIgnoreSolutionFolders()
		{
			// Arrange
			var logger = Substitute.For<IOutput>();
			var sut = new ProjectLoader(logger);

			var solutionFile = new FileInfo("TestData/SolutionWithFolders.txt");

			// Act
			var actual = await sut.LoadProjectsFrom(solutionFile).ToListAsync();

			// Assert
			Assert.Equal(2, actual.Count);
		}
	}
}