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
using System.Diagnostics;
using System.IO.Compression;

namespace Test_task
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //string portablePostgresPath = @"C:\PostgreSQLPortable";
            //string zipFilePath = "postgresql-portable.zip"; // Укажите путь к архиву
            //string dataDir = Path.Combine(portablePostgresPath, "data");
            //string backupSqlFile = "backup.sql"; // Укажите путь к вашему SQL-скрипту

            //// Шаг 1: Распаковка архива, если папка не существует
            //if (!Directory.Exists(portablePostgresPath))
            //{
            //    ZipFile.ExtractToDirectory(zipFilePath, portablePostgresPath);
            //}

            //// Шаг 2: Проверка, инициализирована ли база данных
            //if (!Directory.Exists(dataDir) || !File.Exists(Path.Combine(dataDir, "PG_VERSION")))
            //{
            //    string initdbPath = Path.Combine(portablePostgresPath, "bin", "initdb.exe");
            //    Console.WriteLine($"Путь к initdb: {initdbPath}");
            //    RunCommand(Path.Combine(portablePostgresPath, "bin", "initdb.exe"), $"-D \"{dataDir}\"");
            //}
            //else
            //{
            //    Console.WriteLine("База данных уже инициализирована.");
            //}

            //// Шаг 3: Проверка, запущен ли сервер
            //if (!IsPostgresRunning(dataDir))
            //{
            //    RunCommand(Path.Combine(portablePostgresPath, "bin", "pg_ctl.exe"), $"-D \"{dataDir}\" start");
            //}
            //else
            //{
            //    Console.WriteLine("Сервер PostgreSQL уже запущен.");
            //}

            //// Шаг 4: Выполнение SQL-скрипта
            //RunCommand(Path.Combine(portablePostgresPath, "bin", "psql.exe"), $"-U postgres -d имя_вашей_базы -f \"{backupSqlFile}\"");


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
            
            using (ProjectManagementDbContext dbContext = new ProjectManagementDbContext())
            {
                var test = dbContext.Users.FirstOrDefault();
                if (test == null)
                {
                    RoleEntity role = new RoleEntity { Id = 1, Name = "admin" };
                    UserEntity admin = new UserEntity { login = "admin", password = "admin123", Role = role, Id = 1 };
                    ProjectEntity project = new ProjectEntity { Name = "testProject" };
                    dbContext.Roles.Add(role);
                    dbContext.SaveChanges();
                    dbContext.Users.Add(admin);
                    dbContext.SaveChanges();
                    dbContext.Projects.Add(project);
                    dbContext.SaveChanges();
                }
            }

            User user = new User();
            bool check = true;
            while (check)
            {
                Console.WriteLine("Введите логин:");
                var login = Console.ReadLine();
                Console.WriteLine("Введите пароль:");
                var password = Console.ReadLine();

                user = await userService.GetUserByLoginAsync(login);
                if (user == null || user.password != password)
                {
                    Console.WriteLine("Неверный логин или пароль.\n");
                }
                else check = false;

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
                    // Просмотр всех пользователей существующих
                    case "1":
                        var users = await userService.GetUsersAsync();
                        Console.WriteLine("Пользователи:\n");
                        foreach (var u in users)
                        {
                            Console.WriteLine($"Логин: {u.login}, Роль: {u.roleName}");
                        }
                        break; 

                    // Добавление пользователя
                    case "2":
                        Console.WriteLine("Введите логин нового пользователя:");
                        var newLogin = Console.ReadLine();
                        Console.WriteLine("Введите пароль:");
                        var newPassword = Console.ReadLine();
                        Console.WriteLine("Введите роль:");
                        var newRole = Console.ReadLine();
                        User createUser = new User { login = newLogin, password = newPassword, roleName = newRole };
                        try
                        {
                            if (await userService.CreateUserAsync(createUser, user.Id) == 0) 
                            {
                                Console.WriteLine("Пользователь не был добавлен так, как такой логин уже существует");
                                break;
                            };
                        }
                        catch(Exception ex) 
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                    // Создание задачи
                    case "3":
                        Console.WriteLine("Введите название задачи:");
                        var name = Console.ReadLine();
                        Project project = new Project { Name = name };
                        try
                        {
                            await taskService.CreateProjectAsync(project, user.Id);
                        }
                        catch (Exception ex) 
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }
                        Console.WriteLine("Задача добавлена.");
                        break;

                    // Назначение задачи на пользователей
                    case "4":
                        var allProjects = await taskService.GetAllProjectsAsync();

                        foreach(var pr in allProjects) 
                        {
                            Console.WriteLine($"Название задачи: {pr.Name}, её номер: {pr.Id}" + ((pr.UserId != 0) ? $", Владелец задачи: {pr.UserId}" : "" 
                                + ((pr.Status != null) ? $", Статус задачи: {pr.Status}" : "")));
                        }
                        Console.WriteLine("\nВыберите задачу, которую хотите изменить по её номеру: ");
                        string idChoice =Console.ReadLine();
                        int projectId = 0;
                        if (!int.TryParse(idChoice, out projectId))
                        {
                            Console.WriteLine("Вы ввели неверный символ, введите номер задачи\n");
                            break;
                        }
                        var choiceProject = await taskService.GetProjectByIdAsync(projectId);

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

        //static void RunCommand(string command, string arguments)
        //{
        //    ProcessStartInfo startInfo = new ProcessStartInfo
        //    {
        //        FileName = command,
        //        Arguments = arguments,
        //        RedirectStandardOutput = true,
        //        RedirectStandardError = true,
        //        UseShellExecute = false,
        //        CreateNoWindow = true
        //    };

        //    using (Process process = Process.Start(startInfo))
        //    {
        //        process.WaitForExit();
        //        string output = process.StandardOutput.ReadToEnd();
        //        string error = process.StandardError.ReadToEnd();

        //        if (process.ExitCode != 0)
        //        {
        //            Console.WriteLine($"Error: {error}");
        //        }
        //        else
        //        {
        //            Console.WriteLine(output);
        //        }
        //    }
        //}

        //static bool IsPostgresRunning(string dataDir)
        //{
        //    string pidFile = Path.Combine(dataDir, "postmaster.pid");
        //    return File.Exists(pidFile);
        //}
    }
}
