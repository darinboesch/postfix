using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using postfix.Models.Identity;
using postfix.Models.User;

namespace postfix.Shared.DataAccess
{
    public class PostfixContext
    {
        private IConfigurationRoot _config;
        private ILogger<UserManager<PostfixUser>> _userManagerLogger;

        public IMongoDatabase Database { get; set; }
        public UserManager<PostfixUser> UserManager { get; set; }
        public RoleManager<IdentityRole> RoleManager { get; set; }

        public PostfixContext(IConfigurationRoot config, ILogger<UserManager<PostfixUser>> userManagerLogger) {
            _config = config;
            _userManagerLogger = userManagerLogger;

            var client = new MongoClient(_config["ConnectionStrings:PostfixConnection"]);
            Database = client.GetDatabase(_config["Databases:PostfixDatabaseName"]);

            var users = Database.GetCollection<PostfixUser>("users");
            var userStore = new UserStore<PostfixUser>(users);
            UserManager = new UserManager<PostfixUser>(userStore, null, new PasswordHasher<PostfixUser>(),
                                                        null, null, null, null, null, _userManagerLogger);

            var roles = Database.GetCollection<IdentityRole>("roles");
            var roleStore = new RoleStore<IdentityRole>(roles);
            RoleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null, null);
        }
    }
}