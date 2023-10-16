using System.Text.Json;

namespace ProjectMuseum.Repositories
{
    public class JsonFileDatabase<T>
    {
        private readonly string _filePath;

        public JsonFileDatabase(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<List<T>?> ReadDataAsync()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    await using FileStream fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    // Configure the JsonSerializerOptions with your custom naming policy.
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = new PascalCaseNamingPolicy()
                    };

                    return await JsonSerializer.DeserializeAsync<List<T>>(fileStream, options);
                }
                return new List<T>();
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error.
                throw new ApplicationException("Error reading JSON file.", ex);
            }
        }

        public async Task WriteDataAsync(List<T> data)
        {
            try
            {
                await using FileStream fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                // Configure the JsonSerializerOptions with your custom naming policy.
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new PascalCaseNamingPolicy()
                };

                await JsonSerializer.SerializeAsync(fileStream, data, options);
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error.
                throw new ApplicationException("Error writing to JSON file.", ex);
            }
        }
    }
}
