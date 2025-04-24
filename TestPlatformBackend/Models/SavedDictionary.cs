using System;

namespace TestPlatformBackend.Models
{
    public class SavedDictionary
    {
        public int Id { get; set; }
        public string DictionaryName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public Topic? Topic { get; set; }
    }
}