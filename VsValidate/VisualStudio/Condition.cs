namespace VsValidate.VisualStudio
{
	internal class Condition : ICondition
	{
		public Condition(string expression)
		{
			Expression = expression;
		}

		public string Expression { get; }
	}
}