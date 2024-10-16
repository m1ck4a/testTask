using ProjectManagement.Core.Models;

namespace ProjectManagement.Core.Abstactions
{
    public interface IUsersRepository
    {
        Task<int> Create(User user, int createdByUserId);
        Task<int> Delete(int id);
        Task<List<User>> GetAll();
        Task<User> GetUserById(int id);
        Task<User> GetUserByLogin(string? login);
        Task<int> Update(User user, int Created);
    }
}