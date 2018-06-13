using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageService.Web.Models;
using System.Threading;
using ImageService.Web.Models.LogicObjects;

namespace ImageService.Web.Controllers
{
    public class ImageServiceController : Controller
    {
        #region Members
        public static PhotosLogicModel PhotosModel = new PhotosLogicModel();
        private static Photo _chosenPhoto;
        private static ImageServiceModel _imageServiceModel = new ImageServiceModel(PhotosModel);
        private static LogsModel _logsModel = new LogsModel();
        private static bool _initialized = false;
        private static SettingsModel _settingsModel = new SettingsModel();
        private static string _handlerForDelete;
        #endregion

        #region C'tor
        public ImageServiceController()
        {
            if (_initialized)
            {
                SubscribeModelsNotifications();
            }
            else
            {
                _initialized = true;
            }
        }
        #endregion

        #region LogsMethods
        /// <summary>
        /// Handles logs model notification.
        /// </summary>
        public void NotifyUpdateLogs()
        {
            LogsView();
        }
        /// <summary>
        /// POST: LogsView/form
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogsView(FormCollection form)
        {
            string type = form["filterParam"];
            //No filtering is needed
            if (type == "")
            {
                return LogsView();
            }
            //Filter according to the requested log type
            var filteredLogs = _logsModel.LogList.Where(log => log.LogType == type).ToList();
            return View(filteredLogs);
        }

        /// <summary>
        /// GET: LogsView
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LogsView()
        {
            return View(_logsModel.LogList);
        }
        #endregion

        #region SettingsMethods
        /// <summary>
        /// Handles setting model notification.
        /// </summary>
        public void NotifyUpdateSettings()
        {
            SettingsView();
        }
        /// <summary>
        /// GET: SettingsView
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SettingsView()
        {
            return View(_settingsModel.ServiceSettings);
        }

        /// <summary>
        /// GET: CommitHandlerDeleteView
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CommitHandlerDeleteView()
        {
            return View(_settingsModel);
        }
        /// <summary>
        /// GET: DeleteHandler
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteHandler()
        {
            _settingsModel.StopWatchingHandler(_handlerForDelete);
            Thread.Sleep(500);
            return RedirectToAction("SettingsView");

        }
        /// <summary>
        /// GET: /ApplyDeleteHandler/handlerForDelete
        /// </summary>
        /// <param name="handlerForDelete"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ApplyDeleteHandler(string handlerForDelete)
        {
            _handlerForDelete = handlerForDelete;
            return RedirectToAction("CommitHandlerDeleteView");

        }

        /// <summary>
        /// GET: /DeleteCancel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteCancel()
        {
            return RedirectToAction("SettingsView");
        }
        #endregion

        #region PhotosMethods
        /// <summary>
        /// Handles photos model notification.
        /// </summary>
        void NotifyPhotos()
        {
            PhotosView();
        }
        /// <summary>
        /// GET: PhotosView
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PhotosView()
        {
            PhotosModel.PhotosList.Clear();
            PhotosModel.GetPhotosFromOutputDirectory();
            return View(PhotosModel.PhotosList);
        }

        /// <summary>
        /// GET: SinglePhotoView/photoPath
        /// </summary>
        /// <param name="relativePathToImage"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SinglePhotoView(string relativePathToImage)
        {
            foreach (Photo photo in PhotosModel.PhotosList.Where(photo => photo.RelativePathToPhoto == relativePathToImage))
            {
                _chosenPhoto = photo;

                // When found, stop:
                break;
            }
            return View(_chosenPhoto);
        }

        /// <summary>
        /// GET: DeletePhotoView/photoPath
        /// </summary>
        /// <param name="relativePathToImage"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeletePhotoView(string relativePathToImage)
        {
            foreach (Photo photo in PhotosModel.PhotosList.Where(photo => photo.RelativePathToPhoto == relativePathToImage))
            {
                _chosenPhoto = photo;

                // When found, stop:
                break;
            }
            return View(_chosenPhoto);
        }

        /// <summary>
        /// GET: CommitPhotoDelete/photoPath
        /// </summary>
        /// <param name="relativePathToImage"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CommitPhotoDelete(string relativePathToImage)
        {
            PhotosModel.DeletePhoto(_chosenPhoto);
            return RedirectToAction("PhotosView");
        }
        #endregion

        #region ImageServiceWebMethods
        /// <summary>
        /// GET: ImageServiceView
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ImageServiceView()
        {
            return View(_imageServiceModel);
        }
        #endregion

        #region InnerMethods
        /// <summary>
        /// Initializes the Notify event at every model.
        /// </summary>
        private void SubscribeModelsNotifications()
        {
            _logsModel.NotifyUpdate -= NotifyUpdateLogs;
            _logsModel.NotifyUpdate += NotifyUpdateLogs;
            PhotosModel.NotifyEvent -= NotifyPhotos;
            PhotosModel.NotifyEvent += NotifyPhotos;
            _settingsModel.NotifyUpdate += NotifyUpdateSettings;
        }
        #endregion
    }
}