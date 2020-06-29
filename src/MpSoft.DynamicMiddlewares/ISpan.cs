namespace MpSoft.DynamicMiddlewares
{
	public interface ISpan
	{
		int Begin { get; }
		string Name { get; }
		void End();
	}
}
