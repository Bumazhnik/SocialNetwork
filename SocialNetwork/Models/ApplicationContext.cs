using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace SocialNetwork.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        private IPasswordHasher<User> passwordHasher = new PasswordHasher<User>();
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        private User MakeAdmin(Role adminRole)
        {
            string adminEmail = "admin@mail.ru";
            string adminPassword = "123456";
            var user = new User { Email = adminEmail, Name = "admin", RoleId = adminRole.Id };
            string hashPassword = passwordHasher.HashPassword(user, adminPassword);
            user.Password = hashPassword;
            return user;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";


            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = MakeAdmin(adminRole);
            adminUser.Id = 1;

            /*var converter = new ValueConverter<HashSet<int>, string>(
            v => string.Join(";", v),
            v => new HashSet<int>(v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => int.Parse(val))));*/
            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}
