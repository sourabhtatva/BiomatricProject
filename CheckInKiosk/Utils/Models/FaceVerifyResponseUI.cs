namespace CheckInKiosk.Utils.Models
{
    public class FaceVerifyResponseUI
    {
        public bool IsIdentical { get; set; }
        public float Confidence { get; set; }
        public float Similarity { get; set; }
        public bool ApiFailedStatus { get; set; }
    }
}
