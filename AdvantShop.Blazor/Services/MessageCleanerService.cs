using AdvantShop.Blazor.Data;
using Microsoft.EntityFrameworkCore;

namespace AdvantShop.Blazor.Services
{
    public class MessageCleanerService : IHostedService, IDisposable
    {
        private Timer? Timer { get; set; }

        private IServiceProvider ServiceProvider { get; set; }

        public MessageCleanerService(IServiceProvider serviceProvider) 
        {
            ServiceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            using (var dbContext = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var oldMessages = dbContext.ChatMessages
                .Where(m => m.CreateAt.Date < DateTime.Now.Date)
                            .ToList();

                dbContext.RemoveRange(oldMessages);
                dbContext.SaveChanges();
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
