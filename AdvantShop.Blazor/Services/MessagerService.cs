using AdvantShop.Blazor.Data;
using AdvantShop.Blazor.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AdvantShop.Blazor.Services
{
    public interface IMessengerService
    {
        ICollection<ChatUser> Users { get; }

        Task RemoveTrashMessages();
        void AddChatUser(string userName, string connectionId);
        Task<IEnumerable<ChatMessage>> GetChatMessages(long roomId);
        ChatRoom AddRoom(string roomName);
        void SaveMessage(ChatMessage chatMessage);
        IEnumerable<ChatRoom> GetRooms();
    }

    public class MessengerService : IMessengerService
    {
        public ICollection<ChatUser> Users { get; private set; }
        private ApplicationDbContext DbContext { get; init; }

        public MessengerService()
        {

        }

        public void AddChatUser(string userName, string connectionId)
        {
            DbContext.ChatUsers.Add(new ChatUser()
            {
                ConnectionId = connectionId,
                Name = userName,
            });

            DbContext.SaveChanges();
        }

        public void RemoveChatUser(string userName, string connectionId)
        {
            DbContext.ChatUsers.Remove(new ChatUser()
            {
                ConnectionId = connectionId,
                Name = userName,
            });

            DbContext.SaveChanges();

        }

        public async Task<IEnumerable<ChatMessage>> GetChatMessages(long roomId)
        {
            return await DbContext.ChatMessages.Where(m => m.ChatRoomId.Equals(roomId)).ToListAsync();
        }

        public void SaveMessage(ChatMessage chatMessage) 
        {
            DbContext.ChatMessages.Add(chatMessage);
            DbContext.SaveChanges();
        }

        public async Task RemoveTrashMessages()
        {
            DbContext.ChatMessages.RemoveRange(DbContext.ChatMessages.Where(m => m.CreateAt.Date < DateTime.Now.Date));
            await DbContext.SaveChangesAsync();
        }

        public ChatRoom AddRoom(string roomName)
        {
            var result = DbContext.ChatRooms.Add(new ChatRoom()
            {
                Name = roomName,
            }).Entity;
            DbContext.SaveChanges();
            return result;
        }

        public IEnumerable<ChatRoom> GetRooms()
        {
            return DbContext.ChatRooms.ToList();
        }
    }

    public class MessagerServiceHub : Hub
    {
        private IMessengerService Messager { get; set; }

        public MessagerServiceHub(IMessengerService messagerService) 
        {
            Messager = messagerService;
        }

        public async Task<IEnumerable<ChatMessage>> GetRoomMessages(long roomId)
        {
            return await Messager.GetChatMessages(roomId);
        }

        public async Task SendMessage(string message, long roomId)
        {
            var newMessage = new ChatMessage()
            {
                ChatRoomId = roomId,
                CreateAt = DateTime.Now,
                Message = message,
                SendBy = Messager.Users.First(m => m.ConnectionId == Context.ConnectionId).Name
            };
            Messager.SaveMessage(newMessage);

            await Clients.All.SendAsync("Broadcast", newMessage);
        }

        public async Task<long> AddRoom(string roomName)
        {
            return Messager.AddRoom(roomName).Id;
        }

        public async Task<IEnumerable<ChatRoom>> GetRooms()
        {
            return Messager.GetRooms();
        }

        public async Task<bool> AddUser(string userName)
        {
            if (Messager.Users.Any(m => m.ConnectionId.Equals(Context.ConnectionId)))
            {
                return false;
            }

            Messager.AddChatUser(userName, Context.ConnectionId);

            return true;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            await base.OnDisconnectedAsync(e);
        }
    }
}
