using System.Threading;
using System.Threading.Tasks;
using WebChatData.Models;

namespace WebChatBotsWorkerService.Workers
{
    public interface IMessageBot
    {
        Task RunAsync(Message message, CancellationToken token);
    }
}
