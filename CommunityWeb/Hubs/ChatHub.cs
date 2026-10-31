using Microsoft.AspNetCore.SignalR;

namespace CommunityWeb.Hubs
{
    public class ChatHub : Hub
    {
        // Этот метод могут вызывать клиенты
        public async Task SendMessage(string user, string message)
        {
            // Отправляем сообщение ВСЕМ подключенным клиентам
            // "ReceiveMessage" - это имя JS-функции, которую вызовет браузер
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
