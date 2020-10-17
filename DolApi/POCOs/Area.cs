namespace DolApi.POCOs
{
    using Google.Cloud.Firestore;

    [FirestoreData]
    public class Area
    {
        [FirestoreProperty]
        public int X { get; set; }
        [FirestoreProperty]
        public int Y { get; set; }
    }
}