namespace DolApi.Wrappers
{
    using Google.Cloud.Firestore;

    public interface IFirestoreFactory
    {
        IFirestoreDb Create(string projectId);
    }

    public class FirestoreFactory : IFirestoreFactory
    {
        public IFirestoreDb Create(string projectId)
        {
            return new DolDb(FirestoreDb.Create(projectId)); 
        }
    }
}