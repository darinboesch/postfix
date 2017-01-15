using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace postfix.Models.Identity
{
    public class IdentityRole
    {
        public IdentityRole() {
            Id = ObjectId.GenerateNewId().ToString();
        }
        
        public IdentityRole(string roleName) : this() {
            Name = roleName;
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; }

        public string Name { get; set; }
    }
}