namespace DolApi.POCOs
{
    using dol_sdk.Enums;
    using dol_sdk.POCOs;

    public class PlayerRequest : IPlayerRequest
    {
        public string Email { get; set; }
        public Authority Authority { get; set; }
    }
}