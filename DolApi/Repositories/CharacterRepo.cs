using System.Collections.Generic;
using DolApi.POCOs;

namespace DolApi.Repositories
{
    public interface ICharacterRepo
    {
        Character Add(string user, string name);
        IEnumerable<Character> RetrieveAll(string user);
        Character Retrieve(string user,string name);
        void Remove(string user,string name);
    }

    public class CharacterRepo : ICharacterRepo
    {
        public Character Add(string user, string name)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Character> RetrieveAll(string user)
        {
            throw new System.NotImplementedException();
        }

        public Character Retrieve(string user,string name)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(string user,string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
