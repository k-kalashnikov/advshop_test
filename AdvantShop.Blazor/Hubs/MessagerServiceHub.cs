using AdvantShop.Blazor.Data;
using AdvantShop.Blazor.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AdvantShop.Blazor.Hubs
{
    public class MessagerServiceHub : Hub
    {
        private ApplicationDbContext DbContext { get; init; }

        public MessagerServiceHub(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<IEnumerable<ChatMessage>> GetRoomMessages(long roomId)
        {
            return await DbContext.ChatMessages.Where(m => m.ChatRoomId.Equals(roomId)).ToListAsync();
        }

        public async Task SendMessage(string message, long roomId)
        {
            var newMessage = new ChatMessage()
            {
                ChatRoomId = roomId,
                CreateAt = DateTime.Now,
                Message = message,
                SendBy = DbContext.ChatUsers.First(m => m.ConnectionId == Context.ConnectionId).Name
            };

            DbContext.ChatMessages.Add(newMessage);
            DbContext.SaveChanges();

            var userNames = DbContext.ChatRoomUsers
                .Where(ru => ru.RoomId.Equals(roomId))
                .Select(m => m.UserName)
                .ToList();

            var connectionIds = DbContext.ChatUsers
                .Where(m => userNames.Contains(m.Name))
                .Select(m => m.ConnectionId)
                .ToList();

            await Clients.Clients(connectionIds).SendAsync("Broadcast", newMessage);
        }

        public async Task<long> AddRoom(string roomName)
        {
            var result = DbContext.ChatRooms.Add(new ChatRoom()
            {
                Name = roomName,
            }).Entity;
            DbContext.SaveChanges();
            return result.Id;
        }

        public async Task<IEnumerable<ChatRoom>> GetRooms()
        {
            return await DbContext.ChatRooms.ToListAsync();
        }

        public async Task<bool> AddUser(string userName)
        {
            if (DbContext.ChatUsers.Any(m => m.ConnectionId.Equals(Context.ConnectionId)))
            {
                return false;
            }

            DbContext.ChatUsers.Add(new ChatUser()
            {
                ConnectionId = Context.ConnectionId,
                Name = userName
            });

            DbContext.SaveChanges();

            return true;
        }

        public void EnterToRoom(long roomId)
        {
            var user = DbContext
                .ChatUsers
                .First(m => m.ConnectionId.Equals(Context.ConnectionId));
            DbContext
                .ChatRoomUsers
                .RemoveRange(DbContext
                    .ChatRoomUsers
                    .Where(m => m.UserName.Equals(user.Name)));

            DbContext.ChatRoomUsers.Add(new ChatRoomUser()
            {
                RoomId = roomId,
                UserName = user.Name
            });

            DbContext.SaveChanges();
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            DbContext.ChatUsers.Remove(DbContext.ChatUsers.First(m => m.ConnectionId.Equals(Context.ConnectionId)));
            DbContext.SaveChanges();
            await base.OnDisconnectedAsync(e);
        }
    }
}
