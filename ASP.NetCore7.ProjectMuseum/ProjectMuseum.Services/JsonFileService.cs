using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class JsonFileService<T>
{
    private readonly string _filePath;

    public JsonFileService(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<List<T>> ReadDataAsync()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                using (FileStream fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return await JsonSerializer.DeserializeAsync<List<T>>(fileStream);
                }
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
            using (FileStream fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await JsonSerializer.SerializeAsync(fileStream, data);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions here, e.g., log the error.
            throw new ApplicationException("Error writing to JSON file.", ex);
        }
    }
}