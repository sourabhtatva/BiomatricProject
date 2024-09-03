namespace BiometricAuthenticationAPI.Helpers.Enums
{
    public enum RejectReason
    {
        None,
        DocumentNotFound,
        InvalidDocumentType,
        PassengerIsBlackListed,
        GeneralError
    }
}
