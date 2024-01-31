using System.Text;
using System.Text.Json;

namespace ProjectMuseum.Models;

public class GG
{
    private byte[] body;
    public T? GetScript<T>(string URL)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        var objectToReturn = JsonSerializer.Deserialize<T>(jsonStr);
        return objectToReturn;
    }
}