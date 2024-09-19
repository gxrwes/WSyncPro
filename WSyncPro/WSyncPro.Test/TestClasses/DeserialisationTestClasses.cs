using System;
using System.Collections.Generic;

namespace WSyncPro.Test.TestClasses
{
    /// <summary>
    /// Represents a complex class for testing serialization and deserialization.
    /// Includes nested classes, lists, and inheritance.
    /// </summary>
    public class DeserialisationTestClasses
    {
        public string Title { get; set; }
        public int Count { get; set; }
        public List<ChildClass> Children { get; set; }
        public NestedClass Nested { get; set; }
    }

    /// <summary>
    /// Represents a child class with a list of tags.
    /// </summary>
    public class ChildClass
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
    }

    /// <summary>
    /// Represents a nested class containing a list of sub-nested items.
    /// </summary>
    public class NestedClass
    {
        public DateTime Timestamp { get; set; }
        public List<SubNestedClass> SubNestedItems { get; set; }
    }

    /// <summary>
    /// Represents a sub-nested class with additional details.
    /// </summary>
    public class SubNestedClass
    {
        public string Detail { get; set; }
        public int Value { get; set; }
    }
}
