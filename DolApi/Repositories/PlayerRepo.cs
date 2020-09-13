namespace DolApi.Repositories
{
    public interface IPlayerRepo
    {
        void Add(string userName);
    }

    public class PlayerRepo : IPlayerRepo
    {
        public void Add(string userName)
        {
            throw new System.NotImplementedException();
        }
    }
}
