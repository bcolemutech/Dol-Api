using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;

namespace DolApi.Services
{
    public interface IAdminService
    {
        string GetUserByEmailAsync(string email);
        string CreateUserAsync(UserRecordArgs user);
        Task SetCustomUserClaimsAsync(string uid, Dictionary<string,object> claims);
    }

    [ExcludeFromCodeCoverage]
    public class AdminService : IAdminService
    {
        private static FirebaseAuth Auth => FirebaseAuth.DefaultInstance;
        public string GetUserByEmailAsync(string email)
        {
            return Auth.GetUserByEmailAsync(email).Result.Uid;
        }

        public string CreateUserAsync(UserRecordArgs user)
        {
            return Auth.CreateUserAsync(user).Result.Uid;
        }

        public Task SetCustomUserClaimsAsync(string uid, Dictionary<string, object> claims)
        {
            return Auth.SetCustomUserClaimsAsync(uid, claims);
        }
    }
}
