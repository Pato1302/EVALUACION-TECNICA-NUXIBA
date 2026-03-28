using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using NuxibaApi.Data;
using NuxibaApi.Models;
using System.Globalization;

namespace NuxibaApi.Services
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            // Si ya hay datos, no volver a insertar
            if (await context.Users.AnyAsync() || await context.Logins.AnyAsync() || await context.Areas.AnyAsync())
                return;

            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));

            var usersPath = Path.Combine(basePath, "ccUsers.csv");
            var areasPath = Path.Combine(basePath, "ccRIACat_Areas.csv");
            var loginsPath = Path.Combine(basePath, "ccloglogin.csv");

            // 1. Áreas
            if (File.Exists(areasPath))
            {
                var areas = ReadCsv<AreaCsv>(areasPath)
                    .Select(a => new Area
                    {
                        IDArea = TryInt(a.IDArea),
                        AreaName = a.AreaName,
                        StatusArea = TryIntNullable(a.StatusArea),
                        CreateDate = TryDateNullable(a.CreateDate)
                    })
                    .Where(a => a.IDArea != 0)
                    .GroupBy(a => a.IDArea)
                    .Select(g => g.First())
                    .ToList();

                context.Areas.AddRange(areas);
                await context.SaveChangesAsync();
            }

            // 2. Usuarios
            if (File.Exists(usersPath))
            {
                var users = ReadCsv<UserCsv>(usersPath)
                    .Select(u => new User
                    {
                        User_id = TryInt(u.User_id),
                        Login = u.Login ?? "",
                        Nombres = u.Nombre,
                        ApellidoPaterno = u.ApellidoPaterno,
                        ApellidoMaterno = u.ApellidoMaterno,
                        Password = u.Password,
                        TipoUser_id = TryIntNullable(u.TipoUser_id),
                        Status = TryIntNullable(u.Status),
                        fCreate = TryDateNullable(u.fCreate),
                        IDArea = TryIntNullable(u.IDArea),
                        LastLoginAttempt = TryDateNullable(u.LastLoginAttempt)
                    })
                    .Where(u => u.User_id != 0 && !string.IsNullOrWhiteSpace(u.Login))
                    .ToList();

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // 3. Logins
            if (File.Exists(loginsPath))
            {
                var existingUserIds = await context.Users.Select(u => u.User_id).ToListAsync();

                var logins = ReadCsv<LoginCsv>(loginsPath)
                    .Select(l => new Login
                    {
                        User_id = TryInt(l.User_id),
                        Extension = TryInt(l.Extension),
                        TipoMov = TryInt(l.TipoMov),
                        fecha = TryDate(l.fecha)
                    })
                    .Where(l => l.User_id != 0 && existingUserIds.Contains(l.User_id))
                    .ToList();

                context.Logins.AddRange(logins);
                await context.SaveChangesAsync();
            }
        }

        private static List<T> ReadCsv<T>(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.Trim()
            });

            return csv.GetRecords<T>().ToList();
        }

        private static int TryInt(string? value)
            => int.TryParse(value, out var result) ? result : 0;

        private static int? TryIntNullable(string? value)
            => int.TryParse(value, out var result) ? result : null;

        private static DateTime TryDate(string? value)
            => DateTime.TryParse(value, out var result) ? result : DateTime.MinValue;

        private static DateTime? TryDateNullable(string? value)
            => DateTime.TryParse(value, out var result) ? result : null;
    }

    public class AreaCsv
    {
        public string? IDArea { get; set; }
        public string? AreaName { get; set; }
        public string? StatusArea { get; set; }
        public string? CreateDate { get; set; }
    }

    public class UserCsv
    {
        public string? User_id { get; set; }
        public string? Login { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Password { get; set; }
        public string? TipoUser_id { get; set; }
        public string? Status { get; set; }
        public string? fCreate { get; set; }
        public string? IDArea { get; set; }
        public string? LastLoginAttempt { get; set; }
    }

    public class LoginCsv
    {
        public string? User_id { get; set; }
        public string? Extension { get; set; }
        public string? TipoMov { get; set; }
        public string? fecha { get; set; }
    }
}