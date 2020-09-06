using DolApi.Controllers;
using Xunit;

namespace DolApiTest.Controllers
{
    public class UserControllerTest
    {
        private readonly IUserController _sut;

        public UserControllerTest()
        {
            _sut = new UserController();
        }

        [Fact]
        public void Test1()
        {

        }
    }
}
