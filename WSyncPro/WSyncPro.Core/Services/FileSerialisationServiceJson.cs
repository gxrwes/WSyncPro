using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WSyncPro.Core.Services
{
    /// <summary>
    /// Service for serializing and deserializing objects to and from JSON files.
    /// Supports nested classes, lists, and inheritance.
    /// </summary>
    public class FileSerialisationServiceJson
    {
        private readonly JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSerialisationServiceJson"/> class.
        /// Configures JSON serializer settings to handle inheritance and formatting.
        /// </summary>
        public FileSerialisationServiceJson()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto, // Handles inheritance
                Formatting = Formatting.Indented,         // Makes JSON readable
                NullValueHandling = NullValueHandling.Ignore, // Ignores null values
                // Additional settings can be configured here
            };
        }

        /// <summary>
        /// Asynchronously deserializes a JSON file into an object of type T.
        /// Supports nested classes, lists, and inheritance.
        /// </summary>
        /// <typeparam name="T">The type of the target class.</typeparam>
        /// <param name="filePath">The path to the JSON file (relative or absolute).</param>
        /// <returns>An instance of type T deserialized from the JSON file.</returns>
        /// <exception cref="ArgumentException">Thrown when filePath is null or whitespace.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when deserialization fails.</exception>
        public async Task<T> GetFileAsClassAsync<T>(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found at path: {filePath}");

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                using (StreamReader sr = new StreamReader(fs))
                using (JsonTextReader jtr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = JsonSerializer.Create(_serializerSettings);
                    T obj = serializer.Deserialize<T>(jtr);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                throw new InvalidOperationException($"Error deserializing JSON from file '{filePath}'.", ex);
            }
        }

        /// <summary>
        /// Asynchronously serializes an object of type T and saves it to a JSON file.
        /// Supports nested classes, lists, and inheritance.
        /// </summary>
        /// <typeparam name="T">The type of the input class.</typeparam>
        /// <param name="targetFilePath">The path where the JSON file will be saved (relative or absolute).</param>
        /// <param name="inputClass">The instance of type T to serialize.</param>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Thrown when targetFilePath is null or whitespace.</exception>
        public async Task<bool> SaveClassToFileAsync<T>(string targetFilePath, T inputClass)
        {
            if (string.IsNullOrWhiteSpace(targetFilePath))
                throw new ArgumentException("Target file path cannot be null or whitespace.", nameof(targetFilePath));

            try
            {
                // Ensure the directory exists
                string directory = Path.GetDirectoryName(targetFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (FileStream fs = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonTextWriter jtw = new JsonTextWriter(sw))
                {
                    jtw.Formatting = Formatting.Indented; // Makes JSON readable
                    JsonSerializer serializer = JsonSerializer.Create(_serializerSettings);
                    serializer.Serialize(jtw, inputClass);
                    await jtw.FlushAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                Console.WriteLine($"Error serializing object to file '{targetFilePath}': {ex.Message}");
                return false;
            }
        }
    }
}
