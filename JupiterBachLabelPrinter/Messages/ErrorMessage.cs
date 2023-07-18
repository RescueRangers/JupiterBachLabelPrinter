namespace JupiterBachLabelPrinter.Messages
{
	internal class ErrorMessage : CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage<string>
	{
		public ErrorMessage(string message) : base(message)
		{

		}
		public string Message { get; set; }
	}
}