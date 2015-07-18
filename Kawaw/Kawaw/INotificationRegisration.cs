namespace Kawaw
{
    public interface INotificationRegisration
    {
        string Token { get; }
        void Register();
        void Unregister();
    }
}