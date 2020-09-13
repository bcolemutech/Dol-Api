using Google.Cloud.Firestore;

namespace DolApi.POCOs
{
    [FirestoreData]
    public class Character
    {
        [FirestoreProperty]
        public string Name { get; set; }
    }
}
