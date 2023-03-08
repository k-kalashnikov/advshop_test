using AdvantShop.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvantShop.Blazor.Data
{
    public class ApplicationDbContext : DbContext
    {
        private string DbPath { get; set; }
        public ApplicationDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "adv_shop.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }

        public DbSet<ChatRoomUser> ChatRoomUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatRoomUser>().HasKey(vf => new { vf.RoomId, vf.UserName});
        }
    }
}
