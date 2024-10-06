using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WSyncPro.Util.Files
{
    public class FileLoader : IFileLoader
    {
        // Create a default JsonSerializerOptions instance with the enum converter
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() } // Add enum converter to handle string values for enums
        };

        public async Task<T> LoadFileAndParseAsync<T>(string filePath)
        {
            var fileContent = await LoadFileAsStringAsync(filePath);
            return Deserialize<T>(fileContent);
        }

        public async Task<string> LoadFileAsStringAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found at path: {filePath}");

            return await File.ReadAllTextAsync(filePath);
        }

        public async Task<List<string>> LoadFileAsListAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found at path: {filePath}");

            var lines = await File.ReadAllLinesAsync(filePath);
            return new List<string>(lines);
        }

        public async Task SaveToFileAsStringAsync(string filePath, string content)
        {
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task SaveToFileAsObjectAsync<T>(string filePath, T obj)
        {
            var serializedContent = Serialize(obj);
            await SaveToFileAsStringAsync(filePath, serializedContent);
        }

        public async Task UpdateFileWithContentAsync(string filePath, string content)
        {
            if (!File.Exists(filePath))
            {
                await SaveToFileAsStringAsync(filePath, content);
            }
            else
            {
                await File.AppendAllTextAsync(filePath, content);
            }
        }

        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, _jsonOptions); // Use the custom options with enum converter
        }

        public T Deserialize<T>(string contentToDeserialize)
        {
            return JsonSerializer.Deserialize<T>(contentToDeserialize, _jsonOptions); // Use the custom options with enum converter
        }
    }
}
