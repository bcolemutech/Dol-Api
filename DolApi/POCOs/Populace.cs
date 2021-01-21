using dol_sdk.POCOs;
using Google.Cloud.Firestore;

namespace DolApi.POCOs
{
    public class Populace : IPopulace
    {
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public int Size { get; set; }
        [FirestoreProperty]
        public bool HasPort { get; set; }
    }
}