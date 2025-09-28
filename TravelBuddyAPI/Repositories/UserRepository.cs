using BusinessObject.Models;
using Supabase.Postgrest;
using static Supabase.Postgrest.Constants;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Supabase.Client _supabase;

        public UserRepository(Supabase.Client supabaseClient)
        {
            _supabase = supabaseClient;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var response = await _supabase.From<User>().Get();
            return response.Models;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var response = await _supabase
                .From<User>()
                .Filter("user_id", Operator.Equals, userId)
                .Single();
            return response;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var response = await _supabase
                .From<User>()
                .Filter("email", Operator.Equals, email)
                .Single();
            return response;
        }

        public async Task<User> AddUserAsync(User user)
        {
            var response = await _supabase.From<User>().Insert(user);
            return response.Models.First();
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            // Supabase cần PrimaryKey để biết đối tượng nào cần cập nhật
            var response = await _supabase.From<User>().Update(user);
            return response.Models.First();
        }

        public async Task DeleteUserAsync(int userId)
        {
            await _supabase
                .From<User>()
                .Filter("user_id", Operator.Equals, userId)
                .Delete();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var response = await _supabase
                .From<User>()
                .Filter("email", Operator.Equals, email)
                .Single();
            return response;
        }
    }
}
