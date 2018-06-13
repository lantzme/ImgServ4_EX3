using System;
using System.IO;

namespace ImageService.Web.Models.LogicObjects
{
    public class Photo
    {
        #region Properties
        public string PhotoName { get; set; }
        public string YearTaken { get; set; }
        public string MonthTaken { get; set; }
        public string ImagePath { get; set; }
        public string AbsPathToImage { get; set; }
        public string PathToThumbnail { get; set; }
        public string RelativePathToPhoto { get; set; }
        #endregion

        #region C'tor
        /// <summary>
        /// The constructor of the class.
        /// </summary>
        /// <param name="path"></param>
        public Photo(string path)
        {
            try
            {
                InitializePhotoData(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the properties
        /// </summary>
        /// <param name="path"></param>
        private void InitializePhotoData(string path)
        {
            ImagePath = path;
            int relativePathStartIndex = CalculterRelativePathStart(path);
            AbsPathToImage = path.Replace(@"Thumbnails\", "");
            PathToThumbnail = @"\" + path.Substring(relativePathStartIndex, path.Length - relativePathStartIndex);
            RelativePathToPhoto = PathToThumbnail.Replace(@"Thumbnails\", "");
            InitializeDirectories();
        }

        private int CalculterRelativePathStart(string path)
        {
            return path.IndexOf("ImageService.Web", StringComparison.Ordinal) + "ImageService.Web".Length + 1;
        }

        /// <summary>
        /// Initialize the directory properties according to the current directory location.
        /// </summary>
        private void InitializeDirectories()
        {
            PhotoName = Path.GetFileNameWithoutExtension(ImagePath);
            MonthTaken = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(ImagePath));
            YearTaken = Path.GetFileNameWithoutExtension(Path.GetDirectoryName((Path.GetDirectoryName(ImagePath))));
        }
        #endregion
    }
}