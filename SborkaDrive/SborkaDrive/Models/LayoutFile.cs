using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace SborkaDrive.Models
{
    [Index(nameof(OrderId), IsUnique = true)]
    public class LayoutFile
    {
        [Key]
        public int Id { get; set; } = 0;
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string? GoogleFileId { get; set; } = null;

        public LayoutFile() {}

        public LayoutFile(int orderId, string fullPath)
        {
            OrderId = orderId;
            Name = System.IO.Path.GetFileName(fullPath);
            Path = System.IO.Path.GetDirectoryName(fullPath) ?? "";
        }
    }
}
