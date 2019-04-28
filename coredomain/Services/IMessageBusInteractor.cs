namespace Lockbase.CoreDomain.Services {

    public interface IMessageBusInteractor {

        void Receive(string topic, string session_id, string message);
    }
}