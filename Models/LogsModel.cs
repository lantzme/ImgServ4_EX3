using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mime;
using ImageService.Communication.Adapters;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Events;
using ImageService.Logging;
using Newtonsoft.Json;
using System.Threading;
using ImageService.Logging.Modal;
using ImageService.Web.Models.LogicObjects;

namespace ImageService.Web.Models
{
    public class LogsModel
    {
        #region Members
        public delegate void ChangeNotification();
        public event ChangeNotification NotifyUpdate;
        private static ImageServiceWebClient _tcpAdapter;
        #endregion

        #region C'tor

        /// <summary>
        /// The constructor of the class.
        /// </summary>
        public LogsModel()
        {

            LogList = new List<Log>();
            try
            {
                _tcpAdapter = ImageServiceWebClient.Instance;
                _tcpAdapter.HandleResp += HandleResponse;
                _tcpAdapter.RecieveResponseFromServer();
                FetchLogs();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion

        #region Properties
        public List<Log> LogList { get; set; }
        public string FilterParam { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Sends a GetLogsCommand request to the service.
        /// </summary>
        private void FetchLogs()
        {
            try
            {
                CommandRecievedEventArgs commandRecievedEventArgs = new CommandRecievedEventArgs((int)CommandEnum.GetLogsCommand, null, String.Empty);
                _tcpAdapter.SendCommandToServer(commandRecievedEventArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Handles the response received from the service according to the input CommandRecievedEventArgs object.
        /// </summary>
        /// <param name="args"></param>
        private void HandleResponse(CommandRecievedEventArgs args)
        {
            try
            {
                if (args == null) return;
                switch (args.CommandId)
                {
                    case (int) CommandEnum.GetLogsCommand:
                        InitializeLogsCollection(args);
                        break;
                    case (int) CommandEnum.NewLog:
                        UpdateLogsCollection(args);
                        break;
                }
                NotifyUpdate?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Initializes the LogList collection.
        /// </summary>
        /// <param name="args"></param>
        private void InitializeLogsCollection(CommandRecievedEventArgs args)
        {
            var logsJson = ParseJsonToLogsCollection(args);
            try
            {
                foreach (LogMessage log in logsJson)
                {
                    LogList.Add(new Log { LogType = log.Type, Message = log.Message });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Parses the input command to an ObservableCollection object.
        /// </summary>
        /// <param name="args">CommandRecievedEventArgs object</param>
        /// <returns>ObservableCollection of LogMessages</returns>
        private ObservableCollection<LogMessage> ParseJsonToLogsCollection(CommandRecievedEventArgs args)
        {
            return JsonConvert.DeserializeObject<ObservableCollection<LogMessage>>(args.Args[0]);
        }

        /// <summary>
        /// Updates the logs messages collection with the input CommandRecievedEventArgs parameter.
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateLogsCollection(CommandRecievedEventArgs obj)
        {
            try
            {
                LogMessage newLogMessage = new LogMessage { Type = obj.Args[0], Message = obj.Args[1] };
                LogList.Insert(0, new Log { LogType = newLogMessage.Type, Message = newLogMessage.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion
    }
}