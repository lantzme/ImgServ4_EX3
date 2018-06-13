using System.Collections.ObjectModel;

namespace ImageService.Web.Models.LogicObjects
{
    public class Settings
    {
        #region Properties
        public string LogName { get; set; }
        public string OutputDirectory { get; set; }
        public string SourceName { get; set; }
        public int ThumbnailSize { get; set; }
        public bool isOn { get; set; }
        public ObservableCollection<string> Handlers { get; set; }
        #endregion
    }
}