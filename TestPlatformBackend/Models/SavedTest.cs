using System;

namespace TestPlatformBackend.Models
{
    public class SavedTest
    {
        public int Id { get; set; } // первичный ключ
        public string TestName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}