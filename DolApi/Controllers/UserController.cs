using Microsoft.AspNetCore.Authorization;

namespace DolApi.Controllers
{
    public interface IUserController
    {
    }

    [Authorize(Policy = "Admins")]
    public class UserController : IUserController
    {
    }
}
