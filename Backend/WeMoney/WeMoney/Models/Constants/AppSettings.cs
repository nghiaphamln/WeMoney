namespace WeMoney.Models.Constants;

public class AppSettings
{
    public string ConnectionString { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
    public string UserCollectionName { get; init; } = "User";
}