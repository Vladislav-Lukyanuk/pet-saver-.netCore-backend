namespace animalFinder.Enum
{
    public enum ApiError
    {
        NotFound = 1,
        EmailOrPasswordIncorrect,
        IsNotConfirmedUser,
        IncorrectData,
        ConfirmationCodeIsWrong,
        ResetPasswordCodeIsWrong,
        AccessForbidden,
        MailServerDeclinedEmail
    }
}
