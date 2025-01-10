using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Import;

namespace WSyncPro.Models.Settings
{
    [Serializable]
    public class AppSettingsModel
    {
        public ImportDefault ImportDefault { get; set; } = new ImportDefault();

        [Required]
        [StringLength(255)]
        public string AppSettingsBackupPath { get; set; }
    }
}
