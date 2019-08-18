namespace Lockbase.CoreDomain.Services {

	public interface IMessageBusInteractor {

		void Receive(string replyTo, int sessionId, string message);
	}
}