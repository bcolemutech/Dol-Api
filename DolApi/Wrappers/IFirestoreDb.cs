namespace DolApi.Wrappers
{
    using Google.Cloud.Firestore;

    public interface IFirestoreDb
    {
        CollectionReference Collection(string path);
    }

    public class DolDb : IFirestoreDb
    {
        private FirestoreDb db { get; }
        public DolDb(FirestoreDb firestoreDb)
        {
            db = firestoreDb;
        }

        public CollectionReference Collection(string path)
        {
            return db.Collection(path);
        }
    }
}