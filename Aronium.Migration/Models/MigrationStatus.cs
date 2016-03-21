using System;

namespace Aronium.Migration.Models
{
    public class MigrationStatus
    {
        public int ID { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }
    }
}
