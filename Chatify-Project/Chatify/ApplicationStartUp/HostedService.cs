
using Microsoft.EntityFrameworkCore;
using RepositoryPatternUOW.EFcore;

namespace Chatify.ApplicationStartUp
{
    public class HostedService(IServiceScopeFactory scf) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = scf.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.UserConnections.ExecuteDeleteAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
