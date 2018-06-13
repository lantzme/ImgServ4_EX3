using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;
using ImageService.Web.Models.LogicObjects;

namespace ImageService.Web.Models
{
    public class DirTraverser
    {
        #region Members
        string dirToTraverse;
        List<Photo> ListOfPhotos;
        #endregion

        #region C'tor
        public DirTraverser(string dirToTraverse, List<Photo> ListOfPhotos)
        {
            this.dirToTraverse = dirToTraverse;
            this.ListOfPhotos = ListOfPhotos;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns true if input directory exists, otherwise false.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool IsDirectoryExists(string dir)
        {
            return Directory.Exists(dir);
        }

        /// <summary>
        /// Fetches the photos from the output dir.
        /// </summary>
        /// <returns></returns>
        public void GetPhotosFromOutputDirectory()
        {
            try
            {
                string thumbnailDirectory = dirToTraverse + "\\Thumbnails";
                if (IsDirectoryExists(thumbnailDirectory))
                {
                    DirectoryInfo thumbnailDirectoryInfo = new DirectoryInfo(thumbnailDirectory);
                    List<string> fileTypesToHandle = new List<string> { ".jpg", ".bmp", ".gif", ".png" };
                    foreach (DirectoryInfo year in thumbnailDirectoryInfo.GetDirectories())
                    {
                        ScanAllYearDirectory(year, fileTypesToHandle);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Loop through all the subdirectories of 'year' and make a photo object for every file.
        /// </summary>
        /// <param name="yearDirectoryInfo"></param>
        /// <param name="fileTypesToHandle"></param>
        private void ScanAllYearDirectory(DirectoryInfo yearDirectoryInfo, ICollection<string> fileTypesToHandle)
        {
            var directoryName = Path.GetDirectoryName(yearDirectoryInfo.FullName);
            if (directoryName == null || !directoryName.EndsWith("Thumbnails")) return;
            foreach (FileInfo i in yearDirectoryInfo.GetDirectories().SelectMany(monthDirInfo => monthDirInfo.GetFiles()))
            {
                CreateNewPhoto(i, fileTypesToHandle);
            }
        }

        /// <summary>
        /// Creates a photo object for the input file if it is valid.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileTypesToHandle"></param>
        private void CreateNewPhoto(FileSystemInfo fileInfo, ICollection<string> fileTypesToHandle)
        {
            // If the photo isn't valid in terms of extension, don't add:
            if (!fileTypesToHandle.Contains(fileInfo.Extension.ToLower())) return;
            ListOfPhotos.Add(new Photo(fileInfo.FullName));
        }

        #endregion
    }
}