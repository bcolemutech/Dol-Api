namespace DolApi.POCOs;

using dol_sdk.Enums;
using dol_sdk.POCOs;
using Google.Cloud.Firestore;

[FirestoreData]
public class Position : IPosition
{
    [FirestoreProperty]
    public int X { get; set; }
    [FirestoreProperty]
    public int Y { get; set; }
    [FirestoreProperty]
    public string Populace { get; set; }
    [FirestoreProperty]
    public string Location { get; set; }
    [FirestoreProperty]
    public Action Action { get; set; }
}
