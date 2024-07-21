namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entitytoAdd);
        public void RemoveEntity<T>(T entitytoRemove);
        public IEnumerable<User> GetUsers();
        public User GetSingleUser(int userId);
    }
}