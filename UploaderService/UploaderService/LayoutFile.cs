using System.ComponentModel.DataAnnotations;

namespace UploaderService
{
    public class LayoutFile
    {
        public int Id { get; set; } = 0;
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string? GoogleFileId { get; set; } = null;

        public LayoutFile() { }

        public LayoutFile(int orderId, string fullPath)
        {
            OrderId = orderId;
            Name = System.IO.Path.GetFileName(fullPath);
            Path = System.IO.Path.GetDirectoryName(fullPath) ?? "";
        }
    }
}
