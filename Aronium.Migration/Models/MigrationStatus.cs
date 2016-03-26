using System;

namespace Aronium.Migration.Models
{
    public class MigrationStatus
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// Gets or sets migration version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets script description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets full migration file path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets module for which this script was executed.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Gets or sets execution date.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets a value indicating whether migration is equal to specified migration status.
        /// </summary>
        /// <param name="migrationStatus">Status to compare.</param>
        /// <returns>True if is equal, otherwise false.</returns>
        public bool IsEqualTo(MigrationStatus migrationStatus)
        {
            return this.Version == migrationStatus.Version && this.Module == migrationStatus.Module;
        }

        /// <summary>
        /// Gets string representation of this instance.
        /// </summary>
        /// <returns>String representing this instance.</returns>
        public override string ToString()
        {
            return $"{Version} - {Description}";
        }
    }
}
