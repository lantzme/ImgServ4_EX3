using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ImageService.Web.Models.LogicObjects;

namespace ImageService.Web.Models
{
    public class PhotosLogicModel
    {
        #region Members
        public event SettingsModel.ChangeNotification NotifyEvent;
        private static SettingsModel _settings;
        private string _outputDir;
        private DirTraverser dirTraverser;
        #endregion

        #region C'tor
        /// <summary>
        /// The constructor of the class.
        /// </summary>
        public PhotosLogicModel()
        {
            _settings = new SettingsModel();
            _settings.NotifyUpdate += NotifyUpdateHandler;
            PhotosList = new List<Photo>();
            Thread.Sleep(400);
            dirTraverser = new DirTraverser(_outputDir, PhotosList);
        }
        #endregion

        #region Properties
        public List<Photo> PhotosList;

        #endregion

        #region Methods
        /// <summary>
        /// Handles the settings object notification.
        /// </summary>
        void NotifyUpdateHandler()
        {
            if (_settings.ServiceSettings.OutputDirectory != String.Empty)
            {
                _outputDir = _settings.ServiceSettings.OutputDirectory;
                GetPhotosFromOutputDirectory();
                NotifyEvent?.Invoke();
            }
        }

        /// <summary>
        /// Scan the output directory and creates a photo object for every valid file.
        /// </summary>
        public void GetPhotosFromOutputDirectory()
        {
            try
            {
                dirTraverser.GetPhotosFromOutputDirectory();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

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
        /// Deletes the input photo and its thumbnail from the output directory.
        /// </summary>
        /// <param name="photo"></param>
        public void DeletePhoto(Photo photo)
        {
            File.Delete(photo.ImagePath);
            File.Delete(photo.AbsPathToImage);
            PhotosList.Remove(photo);
        }
        #endregion
    }
}