using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace SborkaDownloader.Model
{
    public partial class LayoutFile : ObservableObject
    {
        [ObservableProperty]
        private int id;
        [ObservableProperty]
        private int orderId;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string path;
        [ObservableProperty]
        private string? googleFileId;
        [ObservableProperty]
        private long progress = 0;
        [ObservableProperty]
        private long fileSize = 100;

        public LayoutFile() { }
    }
}
