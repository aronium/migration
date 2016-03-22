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
        /// Gets or sets module for which this script was executed.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Gets or sets execution date.
        /// </summary>
        public DateTime? Date { get; set; }
    }
}
