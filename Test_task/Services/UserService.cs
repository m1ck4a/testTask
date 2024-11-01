﻿using ProjectManagement.Core.Models;
using Test_task.Interfaces;
using ProjectManagement.Core.Abstactions;

namespace Test_task.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;

        public UserService(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> CreateUserAsync(User user, int createdByUserId)
        {
            if (string.IsNullOrWhiteSpace(user.login) || string.IsNullOrEmpty(user.login) || user.login.Length < 3) 
            {
                throw new ArgumentException("Логин не может быть пустым и меньше 3 символов ");
            }
            return await _userRepository.Create(user, createdByUserId);
        }

        public async Task UpdateUserAsync(User user, int updatedByUserId)
        {
            await _userRepository.Update(user, updatedByUserId);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User> GetUserByLoginAsync(string? login)
        {
            return await _userRepository.GetUserByLogin(login);
        }
    }
}
