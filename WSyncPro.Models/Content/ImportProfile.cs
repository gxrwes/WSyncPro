using WSyncPro.Models.Enum;

namespace WSyncPro.Models.Content
{
    public class ImportProfile
    {
        public Guid Id { get; set; } = new Guid();
        public string Name { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; } = Status.Undefined;
        public string DstDirectory { get; set; }
        public List<string> FilterInclude { get; set; } = new List<string>();
        public List<string> FilterExclude { get; set; } = new List<string>();
        public bool includeDirectories { get; set; } = true;
        public bool Enabled { get; set; } = false;// Indicates whether the job is selected in the UI

        public string SubPathBuilder { get; set; } = string.Empty; // For building target directory, eg: "/%YEAR%/%INTID%_%IMPORTNAME%/src/" where an example would parse out to be "/2024/0002_myfirstimport/src/" 
    }
}
