namespace BiometricAuthenticationAPI.Data.Models
{
    public class MatchFacesRequest
    {
        public required byte[] ScannedImage { get; set; }
        public required byte[] ClickedImage { get; set; }
    }
}
