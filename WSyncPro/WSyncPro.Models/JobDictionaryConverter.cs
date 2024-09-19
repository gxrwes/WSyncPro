using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WSyncPro.Models;
using WSyncPro.Models.State;

public class JobDictionaryConverter : JsonConverter<Dictionary<Job, List<JobProgress>>>
{
    public override void WriteJson(JsonWriter writer, Dictionary<Job, List<JobProgress>> value, JsonSerializer serializer)
    {
        var tempDict = value.ToDictionary(
            kvp => kvp.Key.Id, // Use Job.Id as the key
            kvp => kvp.Value
        );
        serializer.Serialize(writer, tempDict);
    }

    public override Dictionary<Job, List<JobProgress>> ReadJson(JsonReader reader, Type objectType, Dictionary<Job, List<JobProgress>> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var tempDict = serializer.Deserialize<Dictionary<string, List<JobProgress>>>(reader);
        var result = new Dictionary<Job, List<JobProgress>>();

        foreach (var kvp in tempDict)
        {
            // Reconstruct the Job object from the Job.Id
            // Assuming you have a method to retrieve a Job by its Id
            // For testing purposes, we'll create a new Job with only the Id set
            var job = new Job { Id = kvp.Key };
            result[job] = kvp.Value;
        }

        return result;
    }
}
