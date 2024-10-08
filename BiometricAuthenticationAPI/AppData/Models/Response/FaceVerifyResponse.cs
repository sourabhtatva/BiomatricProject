namespace BiometricAuthenticationAPI.Data.Models.Response
{
    public class FaceVerifyResponse : APIErrorResponse
    {
        public bool IsIdentical { get; set; }
        public float Confidence { get; set; }
        public float Similarity { get; set; }
    }
}
