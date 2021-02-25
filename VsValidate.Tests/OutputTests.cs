using System.CommandLine;
using System.CommandLine.IO;
using NSubstitute;
using Xunit;

namespace VsValidate.Tests
{
	public class OutputTests
	{
		[Fact]
		public void ErrorShouldNotWriteToStderrWhenSilent()
		{
			// Arrange
			var stderr = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Error.Returns(stderr);
			var sut = new Output(console, true, false);

			// Act
			sut.Error("test-message");

			// Assert
			stderr.DidNotReceiveWithAnyArgs().Write(null!);
		}

		[Fact]
		public void ErrorShouldWriteToStderrWhenNotSilent()
		{
			// Arrange
			var stderr = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Error.Returns(stderr);
			var sut = new Output(console, false, false);

			// Act
			sut.Error("test-message");

			// Assert
			stderr.Received(1).Write(Arg.Is<string>(str => str.Contains("test-message")));
		}

		[Fact]
		public void InfoShouldShouldNotWriteToStdoutWhenSilentIsEnabled()
		{
			// Arrange
			var stdout = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Out.Returns(stdout);
			var sut = new Output(console, true, false);

			// Act
			sut.Info("test-message");

			// Assert
			stdout.DidNotReceiveWithAnyArgs().Write(null!);
		}

		[Fact]
		public void InfoShouldWriteToStdoutWhenSilentIsDisabled()
		{
			// Arrange
			var stdout = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Out.Returns(stdout);
			var sut = new Output(console, false, false);

			// Act
			sut.Info("test-message");

			// Assert
			stdout.Received(1).Write(Arg.Is<string>(str => str.Contains("test-message")));
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void VerboseShouldNotWriteToStdoutWhenSilentIsEnabled(bool verbose)
		{
			// Arrange
			var stdout = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Out.Returns(stdout);
			var sut = new Output(console, true, verbose);

			// Act
			sut.Verbose("test-message");

			// Assert
			stdout.DidNotReceiveWithAnyArgs().Write(null!);
		}

		[Fact]
		public void VerboseShouldShouldNotWriteToStdoutWhenVerboseIsDisabled()
		{
			// Arrange
			var stdout = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Out.Returns(stdout);
			var sut = new Output(console, false, false);

			// Act
			sut.Verbose("test-message");

			// Assert
			stdout.DidNotReceiveWithAnyArgs().Write(null!);
		}

		[Fact]
		public void VerboseShouldWriteToStdoutWhenVerboseIsEnabled()
		{
			// Arrange
			var stdout = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Out.Returns(stdout);
			var sut = new Output(console, false, true);

			// Act
			sut.Verbose("test-message");

			// Assert
			stdout.Received(1).Write(Arg.Is<string>(str => str.Contains("test-message")));
		}

		[Fact]
		public void WarningShouldShouldNotWriteToStdoutWhenSilentIsEnabled()
		{
			// Arrange
			var stdout = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Out.Returns(stdout);
			var sut = new Output(console, true, false);

			// Act
			sut.Warning("test-message");

			// Assert
			stdout.DidNotReceiveWithAnyArgs().Write(null!);
		}

		[Fact]
		public void WarningShouldWriteToStdoutWhenSilentIsDisabled()
		{
			// Arrange
			var stdout = Substitute.For<IStandardStreamWriter>();
			var console = Substitute.For<IConsole>();
			console.Out.Returns(stdout);
			var sut = new Output(console, false, false);

			// Act
			sut.Warning("test-message");

			// Assert
			stdout.Received(1).Write(Arg.Is<string>(str => str.Contains("test-message")));
		}
	}
}