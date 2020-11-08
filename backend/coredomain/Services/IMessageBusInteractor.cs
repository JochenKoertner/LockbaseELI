namespace Lockbase.CoreDomain.Services {

	public class Message
	{
		public string text { get; set; }

		public int correlationId { get; set; }

		public string replyTo { get; set; }
	}


	public interface IMessageBusInteractor {

		void Receive(string replyTo, int sessionId, string message);
	}
}