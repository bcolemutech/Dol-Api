namespace DolApi.POCOs;

using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;
using dol_sdk.POCOs;
using Action = dol_sdk.Enums.Action;
    
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
        if (value is IDictionary<string, object> map)
        {
            return new Position
            {
                Action = (Action)(long)map["Action"],
                Location = (string)map["Location"],
                Populace = (string)map["Populace"],
                X = (int)map["X"],
                Y = (int)map["Y"]
            };
        }
        throw new ArgumentException($"Unexpected data: {value.GetType()}");
    }
}
