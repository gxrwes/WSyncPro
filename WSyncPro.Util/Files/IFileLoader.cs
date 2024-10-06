using System.Collections.Generic;
using System.Threading.Tasks;

namespace WSyncPro.Util.Files
{
    public interface IFileLoader
    {
        Task<T> LoadFileAndParseAsync<T>(string filePath); // Asynchronous method for loading and parsing a file
        Task<string> LoadFileAsStringAsync(string filePath); // Asynchronous method for loading file content as string
        Task<List<string>> LoadFileAsListAsync(string filePath); // Asynchronous method for loading file content as list of strings
        Task SaveToFileAsStringAsync(string filePath, string content); // Asynchronous method for saving string content to file
        Task SaveToFileAsObjectAsync<T>(string filePath, T obj); // Asynchronous method for saving an object as JSON to file
        Task UpdateFileWithContentAsync(string filePath, string content); // Asynchronous method for appending content to file
        string Serialize(object obj); // Synchronous method for serializing an object to a JSON string
        T Deserialize<T>(string contentToDeserialize); // Synchronous method for deserializing a JSON string to an object
    }
}
