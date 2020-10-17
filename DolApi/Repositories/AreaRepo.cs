namespace DolApi.Repositories
{
    using System.Threading.Tasks;
    using POCOs;

    public interface IAreaRepo
    {
        Task<Area> Retrieve(int x, int y);
        Task Replace(int x, int y, Area area);
    }

    public class AreaRepo : IAreaRepo
    {
        public Task<Area> Retrieve(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public Task Replace(int x, int y, Area area)
        {
            throw new System.NotImplementedException();
        }
    }
}