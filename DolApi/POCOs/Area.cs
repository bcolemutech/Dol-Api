namespace DolApi.POCOs
{
    using dol_sdk.Enums;
    using dol_sdk.POCOs;
    using Google.Cloud.Firestore;

    [FirestoreData]
    public class Area: IArea
    {
        [FirestoreProperty]
        public int X { get; set; }
        [FirestoreProperty]
        public int Y { get; set; }
        [FirestoreProperty]
        public string Region { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public bool IsCoastal { get; set; }
        [FirestoreProperty]
        public Ecosystem Ecosystem { get; set; }
        [FirestoreProperty]
        public Navigation Navigation { get; set; }
    }
}