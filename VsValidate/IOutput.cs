namespace VsValidate
{
	internal interface IOutput
	{
		void Error(string message);
		void Info(string message);
		void Verbose(string message);
		void Warning(string message);
	}
}