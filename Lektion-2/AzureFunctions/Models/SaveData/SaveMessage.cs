using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctions.Models.SaveData
{
    public class SaveMessage
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Message { get; set; } = null!;
        public string SystemProperties { get; set; } = null!;
        public string PartitionKey { get; set; } = "Message";
    }
}
