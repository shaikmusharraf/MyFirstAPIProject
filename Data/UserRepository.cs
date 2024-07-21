namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityframework;
        public UserRepository(IConfiguration config)
        {
            _entityframework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityframework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entitytoAdd)
        {
            if (entitytoAdd != null)
            {
                _entityframework.Add(entitytoAdd);
            }
            // SaveChanges();
        }

        public void RemoveEntity<T>(T entitytoRemove)
        {
            if (entitytoRemove != null)
            {
                _entityframework.Remove(entitytoRemove);
            }
            // SaveChanges();
        }

        public IEnumerable<User> GetUsers()
        {
            // string sql = @"SELECT * FROM [TutorialAppSchema].[Users]";
            IEnumerable<User> users = _entityframework.Users.ToList<User>();
            return users;
        }
        public User GetSingleUser(int userId)
        {
            // string sql = @"SELECT * FROM [TutorialAppSchema].[Users] WHERE UserId = " + userId.ToString();
            User? user = _entityframework.Users.Where(o => o.UserId == userId).FirstOrDefault<User>();
            if (user != null)
            {
                return user;
            }
            else
            {
                throw new Exception("Failed to return user");
            }
        }


    }
}