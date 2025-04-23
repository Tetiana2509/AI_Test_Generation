using System;

namespace TestPlatformBackend.Models
{
    public class SavedDictionary
    {
        public int Id { get; set; }
        public string DictionaryName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}