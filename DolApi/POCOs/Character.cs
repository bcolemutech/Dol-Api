namespace DolApi.POCOs
{
    using Google.Cloud.Firestore;
    using dol_sdk.POCOs;
    
    [FirestoreData]
    public class Character: ICharacter 
    {
        [FirestoreProperty]
        public string Name { get; set; }
        
        [FirestoreProperty(ConverterType = typeof(PositionConverter))]
        public IPosition Position { get; set; }
    }

    public class PositionConverter : IFirestoreConverter<IPosition>
    {
        public object ToFirestore(IPosition value)
        {
            return new Position(value);
        }

        public IPosition FromFirestore(object value)
        {
            return value as Position;
        }
    }
}
