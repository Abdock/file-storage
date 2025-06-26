namespace Application.DTO.Enums;

public enum CustomStatusCodes
{
    Ok = 200,

    FileNameAlreadyUsing = 400_001,
    WeakPassword = 400_002,
    InvalidUserCredentials = 400_003,
    UsernameAlreadyUsing = 400_004,

    DoesNotHavePermission = 403_001,
    ApiKeyRevoked = 403_002,
    
    UserWasNotFound = 404_001,
    FileWasNotFound = 404_002,
    ApiKeyWasNotFound = 404_003,
}