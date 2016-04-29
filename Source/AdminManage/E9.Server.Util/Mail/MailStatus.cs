
namespace NM.Mail
{
    public enum MailStatus : int
    {
        Waitting = 0,
        SendSuccess = 1,
        FromAdressInvalid = 2,
        ToAddressInvalid = 3,
        SmtpInvalid = 4,
        DefaultSenderAddressInvalid = 5,
        InfoDocPathInvalid = 6,
        AttachmentMissing = 7,
        AttachmentError = 8,
        UnKnowError = 9
    }
}
