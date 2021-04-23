using System.Collections.Generic;
using System.Threading.Tasks;
using WebChatData.Models;
//using WebChat.Models;

namespace WebChat.Services.Contract
{
    public interface IUserService
    {
        ApplicationUser Authenticate(string username, string password);
        IEnumerable<ApplicationUser> GetAll();
        ApplicationUser GetById(int id);
        ApplicationUser Create(ApplicationUser user, string password);
        Task Update(ApplicationUser user, string password = null);
        Task Delete(int id);
    }
}
