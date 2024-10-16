using Project_Job.DataAccess;
using ProjectManagement.Core.Models;
using ProjectManagement.Architecture.Entities;
using Microsoft.Extensions.DependencyInjection;
using Test_task.Services;
using ProjectManagement.Core.Abstactions;
using ProjectManagement.Core.Abstractions;
using ProjectManagement.Architecture.UserRepository;
using Microsoft.EntityFrameworkCore;
using Test_task.Interfaces;
using ProjectManagement.Architecture.Mappers;

namespace Test_task
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ProjectManagementDbContext>()
                .AddScoped<IRolesRepository, RolesRepository>()
                .AddScoped<IProjectsRepository, ProjectsRepository>()
                .AddScoped<IUsersRepository, UsersRepository>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IProjectService, ProjectService>()
                .AddScoped<UserMapper>()
                .AddScoped<RoleMapper>()
                .AddScoped<ProjectMapper>()
                .BuildServiceProvider();
            serviceProvider.GetRequiredService<IUserService>();
            var userService = serviceProvider.GetService<IUserService>();
            var projectService = serviceProvider.GetService<IProjectService>();


            // Комментарий ниже нужен для создания локальной базы данных, внесения первого юзера (админа), роли админа и просто тестового проекта

            //using (ProjectManagementDbContext dbContext = new ProjectManagementDbContext())
            //{
            //    RoleEntity role = new RoleEntity { Id = 1, Name = "admin" };
            //    UserEntity admin = new UserEntity { login = "admin", password = "admin123", Role = role, Id = 1};
            //    ProjectEntity project = new ProjectEntity { Name = "testProject"};
            //    dbContext.Roles.Add(role);
            //    dbContext.SaveChanges();
            //    dbContext.Users.Add(admin);
            //    dbContext.SaveChanges();
            //    dbContext.Projects.Add(project);
            //    dbContext.SaveChanges();
            //}

            User user = new User();

            Console.WriteLine("Введите логин:");
            var login = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            var password = Console.ReadLine();

            user = await userService.GetUserByLoginAsync(login);
            if (user == null || user.password != password)
            {
                Console.WriteLine("Неверный логин или пароль.");
                return;
            }

            Console.WriteLine($"Добро пожаловать, {user.login}!");

            if (user.roleName == "admin")
            {
                await AdminMenu(userService, projectService, user);
            }
            else
            {
                await UserMenu(projectService, user);
            }
        }

        private static async Task AdminMenu(IUserService userService, IProjectService taskService, User user)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Меню администратора:");
                Console.WriteLine("1. Просмотреть всех пользователей");
                Console.WriteLine("2. Добавить пользователя");
                Console.WriteLine("3. Создать задачу");
                Console.WriteLine("4. Назначить задачу");
                Console.WriteLine("5. Выйти");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var users = await userService.GetUsersAsync();
                        Console.WriteLine("Пользователи:\n");
                        foreach (var u in users)
                        {
                            Console.WriteLine($"Логин: {u.login}, Роль: {u.roleName}");
                        }
                        break;

                    case "2":
                        Console.WriteLine("Введите логин нового пользователя:");
                        var newLogin = Console.ReadLine();
                        Console.WriteLine("Введите пароль:");
                        var newPassword = Console.ReadLine();
                        Console.WriteLine("Введите роль:");
                        var newRole = Console.ReadLine();
                        User createUser = new User { login = newLogin, password = newPassword, roleName = newRole };
                        if (await userService.CreateUserAsync(createUser, user.Id) == null)
                        {
                            Console.WriteLine("Пользователь не был добавлен");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Пользователь добавлен.");
                            break;
                        };

                    case "3":
                        Console.WriteLine("Введите название задачи:");
                        var name = Console.ReadLine();
                        Project project = new Project { Name = name };

                        await taskService.CreateProjectAsync(project, user.Id);
                        Console.WriteLine("Задача добавлена.");
                        break;

                    case "4":
                        var allProjects = await taskService.GetAllProjectsAsync();

                        foreach(var pr in allProjects) 
                        {
                            Console.WriteLine($"Название задачи: {pr.Name}, её номер: {pr.Id}" + ((pr.UserId != 0) ? $", Владелец задачи: {pr.UserId}" : "" 
                                + ((pr.Status != null) ? $", Статус задачи: {pr.Status}" : "")));
                        }
                        Console.WriteLine("\nВыберите задачу, которую хотите изменить по её номеру: ");
                        int idChoice = Convert.ToInt32(Console.ReadLine());
                        var choiceProject = await taskService.GetProjectByIdAsync(idChoice);

                        if (choiceProject == null) 
                        {
                            Console.WriteLine("Такой задачи не существует");
                            break;
                        }
                        users = await userService.GetUsersAsync();
                        Console.WriteLine("\nПользователи:\n");
                        foreach (var u in users)
                        {
                            Console.WriteLine($"Логин: {u.login}, Роль: {u.roleName}, Номер: {u.Id}");
                        }
                        Console.WriteLine("\nВведите номер пользователя на кого хотите назначить: ");

                        int idUser = Convert.ToInt32(Console.ReadLine());

                        choiceProject.UserId = idUser;
                        if (await taskService.UpdateProjectAsync(choiceProject) != -1)
                        {
                            Console.WriteLine("Задача успешно добавлена\n");
                        }
                        else 
                        {
                            Console.WriteLine("Задача не была добавлена, проверьте корректность данных\n");
                        };
                        break;

                    case "5":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.\n");
                        break;
                }
            }
        }

        private static async Task UserMenu(IProjectService taskService, User user)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Меню пользователя:");
                Console.WriteLine("1. Просмотреть задачи");
                Console.WriteLine("2. Изменить статус задачи");
                Console.WriteLine("3. Выйти");

                var choice = Console.ReadLine();
                var projects = await taskService.GetProjectsAsync(user);

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Ваши задачи:\n");
                        foreach (var pr in projects)
                        {
                            Console.WriteLine($"Задача: {pr.Name}, Её номер: {pr.Id}" + ((pr.Status != string.Empty) ? $", Статус задачи: {pr.Status}" : ""));
                        }
                        break;

                    case "2":
                        Console.WriteLine("Ваши задачи:\n");
                        foreach (var pr in projects)
                        {
                            Console.WriteLine($"Задача: {pr.Name}, Её номер: {pr.Id}");
                        }
                        Console.WriteLine("\nВведите номер задачи: ");
                        var idProject = Convert.ToInt32(Console.ReadLine());
                        int idChoice = -1;
                        foreach (var pr in projects)
                        {
                            if (pr.Id == idProject)
                                idChoice = idProject;
                        }
                        if (idChoice == -1) 
                        {
                            Console.WriteLine("Этой задачи нет в Вашем списке, выберите из списка");
                            break;
                        }
                        Console.WriteLine("Введите статус задачи:");
                        var status = Console.ReadLine();
                        if (await taskService.UpdateProjectStatus(status, idChoice) == true)
                        {
                            Console.WriteLine("Статус успешно сменён\n");
                        }
                        else 
                        {
                            Console.WriteLine("Статус неизменён\n");
                        };
                        break;

                    case "3":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }
    }
}
