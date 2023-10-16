using System.Text.Json;

namespace ProjectMuseum.Repositories;

public class PascalCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.Substring(0, 1).ToUpper() + name.Substring(1);
    }
}