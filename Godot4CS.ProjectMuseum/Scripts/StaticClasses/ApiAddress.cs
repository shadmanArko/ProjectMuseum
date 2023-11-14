namespace Godot4CS.ProjectMuseum.Scripts.StaticClasses;

public static class ApiAddress
{
    public const string DomainName = "localhost";
    public const string Port = "5178";
    public const string UrlPrefix = $"http://{DomainName}:{Port}/api/";
    public const string MuseumApiPath = $"{UrlPrefix}Museum/";
    public const string PlayerApiPath = $"{UrlPrefix}Player/";
    public const string StoryApiPath = $"{UrlPrefix}Story/";
    public const string MineApiPath = $"{UrlPrefix}Mine/";
    
}