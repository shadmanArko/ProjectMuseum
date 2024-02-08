using System.Text.Json;

namespace ProjectMuseum.Models;

public static class LanguageSelector
{
    
    public static string GetCurrentLanguage()
    {
        string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Dummy Data Folder", "Language.json");

        // Check if the file exists
        if (File.Exists(jsonFilePath))
        {
            try
            {
                // Read the entire JSON file
                string jsonString = File.ReadAllText(jsonFilePath);

                // Deserialize the JSON string into an object
                var language = JsonSerializer.Deserialize<Language>(jsonString);
                if (language != null)  return language.GameLanguage;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading JSON file: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("JSON file not found.");
        }

        return "EN";
    }
}