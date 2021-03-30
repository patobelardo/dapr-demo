using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public interface INotificationHub
{
    Task Notification(string message);
}
public class NotificationHub : Hub<INotificationHub>
{

}