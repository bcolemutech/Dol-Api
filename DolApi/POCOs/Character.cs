namespace DolApi.POCOs
{
    using Google.Cloud.Firestore;
    using dol_sdk.POCOs;
    
    [FirestoreData]
    public class Character: ICharacter 
    {
        [FirestoreProperty]
        public string Name { get; set; }
        
        [FirestoreProperty]
        public IPosition Position { get; set; }
        
        [FirestoreProperty]
        public IPosition Move { get; set; }
    }
}
