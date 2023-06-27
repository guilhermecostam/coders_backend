namespace Coders_Back.Domain.Enums;

public enum LoginErrorsOutput
{
    AccountBlocked = 1,
    AccountNotAllowed = 2,
    TwoFactorAuthenticationRequired = 3,
    InvalidUsernameOrPassword = 4,
    EmailNotConfirmed = 5
}