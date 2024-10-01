namespace CheckInKiosk.Utils.Models
{
    /// <summary>
    /// Model for face verification process response
    /// </summary>
    public class FaceVerifyResponseUI
    {
        public bool IsIdentical { get; set; }
        public float Confidence { get; set; }
        public float Similarity { get; set; }
        public bool ApiFailedStatus { get; set; }
    }
}
