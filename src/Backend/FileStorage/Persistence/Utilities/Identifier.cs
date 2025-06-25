namespace Persistence.Utilities;

public static class Identifier
{
    // Years past after 1970, may be set value as year when project will start work, any other changes doesn't recommended
    private const int YearsOffset = -55;

    public static Guid GenerateGuid() => Ulid.NewUlid(DateTimeOffset.UtcNow.AddYears(YearsOffset)).ToGuid();
    public static Ulid GenerateUlid() => Ulid.NewUlid(DateTimeOffset.UtcNow.AddYears(YearsOffset));
}