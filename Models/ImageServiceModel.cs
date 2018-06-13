using System.Collections.Generic;
using System.IO;
using System.Threading;
using ImageService.Communication.Adapters;
using ImageService.Web.Models.LogicObjects;

namespace ImageService.Web.Models
{
    public class ImageServiceModel
    {
        #region Members

        #endregion

        #region C'tor
        /// <summary>
        /// The constructor of the class.
        /// </summary>
        /// <param name="photosModel"></param>
        public ImageServiceModel(PhotosLogicModel photosModel)
        {
            Thread.Sleep(200);
            TcpAdapter = TcpClientAdapter.Instance;
            IsConnected = TcpAdapter.IsConnected;
            Thread.Sleep(200);
            NumberOfPhotos = photosModel.PhotosList.Count;
            StudentsList = new List<Student>();
            ParseDetailsFile();
        }
        #endregion

        #region Properties
        public bool IsConnected { get; set; }
        public int NumberOfPhotos { get; set; }
        public List<Student> StudentsList { get; set; }
        public TcpClientAdapter TcpAdapter { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Reads the details text file from the App_Data directory and parses each row to an object.
        /// </summary>
        private void ParseDetailsFile()
        {
            StreamReader file = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/studInfo.txt"));
            var detailsLines = file.ReadLine();

            //Loop through all the lines in the details file
            while (detailsLines != null)
            {
                var studentSplitedLine = detailsLines.Split(' ');
                StudentsList.Add(new Student
                {
                    FirstName = studentSplitedLine[0],
                    LastName = studentSplitedLine[1],
                    Id = long.Parse(studentSplitedLine[2])
                });
                detailsLines = file.ReadLine();
            }
            file.Close();
        }
        #endregion
    }
}