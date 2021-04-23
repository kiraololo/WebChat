using System;
using System.Collections.Generic;
using System.Linq;
using WebChat.Services.Contract;
using WebChatData.Models;
using WebChatDataData.Models.Context;

namespace WebChat.Services.Implementation
{
    public class UserService: IUserService
    {
        private AppIdentityDBContext context;
        private AppDbContext dataContext;

        public UserService(AppIdentityDBContext context, AppDbContext dataContext)
        {
            this.context = context;
            this.dataContext = dataContext;
        }

        public ApplicationUser Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return context.Users;
        }

        public ApplicationUser GetById(int id)
        {
            return context.Users.Find(id);
        }

        public ApplicationUser Create(ApplicationUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");
            
            if (context.Users.Any(x => x.Username == user.Username))
                throw new Exception($"Username \"{user.Username}\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            context.Users.Add(user);
            context.SaveChanges();

            dataContext.ChatUsers.Add(new ChatUser { 
                LoginName = user.Username,
                NikName = $"{user.LastName} {user.FirstName}"
            });
            dataContext.SaveChanges();

            return user;
        }

        public void Update(ApplicationUser userParam, string password = null)
        {
            var user = context.Users.Find(userParam.Id);

            if (user == null)
                throw new Exception("User not found");            

            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                if (context.Users.Any(x => x.Username == userParam.Username))
                    throw new Exception($"Username {userParam.Username} is already taken");
                user.Username = userParam.Username;
            }

            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            context.Users.Update(user);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = context.Users.Find(id);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
