using System.Threading.Tasks;
using MongoCore.DbDriver.Documents;
using MongoDB.Bson;

namespace MongoCore.DbDriver
{
    public interface IUserManager
    {
        Task<UserDocument> FindByEmailAsync(string email);
        Task<UserDocument> FindByUsernameAsync(string username);
        Task<UserDocument> FindByIdAsync(ObjectId id);
    }
}