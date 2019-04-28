namespace Lockbase.CoreDomain.Services {

    public interface IMessageBusInteractor {

        void Receive(string replyTo, string session_id, string message);
    }
}