using System.Collections;
using System.Collections.Generic;
using DolApi.POCOs;

namespace DolApi.Repositories
{
    public interface IPopulaceRepo
    {
        IEnumerable<Populace> GetAllPops(int X, int Y);
    }

    public class PopulaceRepo : IPopulaceRepo
    {
        public IEnumerable<Populace> GetAllPops(int X, int Y)
        {
            throw new System.NotImplementedException();
        }
    }
}