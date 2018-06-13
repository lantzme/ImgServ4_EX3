using System;
using System.Collections.ObjectModel;
using System.Linq;
using ImageService.Communication.Adapters;
using ImageService.Infrastructure.Enums;
using ImageService.Infrastructure.Events;
using ImageService.Web.Models.LogicObjects;

namespace ImageService.Web.Models
{
    public class SettingsModel
    {
        #region Members
        public delegate void ChangeNotification();
        public event ChangeNotification NotifyUpdate;
        #endregion

        #region C'tor
        /// <summary>
        /// The constructor of the class.
        /// </summary>
        public SettingsModel()
        {
            TcpAdapter = ImageServiceWebClient.Instance;
            try
            {
                TcpAdapter.RecieveResponseFromServer();
                TcpAdapter.HandleResp += HandleResponse;
                InitializeSettings();
                ServiceSettings.isOn = false;
                FetchConfigFromServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion

        #region Properties
        public Settings ServiceSettings { get; set; }
        private static ImageServiceWebClient TcpAdapter { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Sends a request to the service
        /// </summary>
        private void FetchConfigFromServer()
        {
            string[] arr = new string[5];
            CommandRecievedEventArgs argsToSend = new CommandRecievedEventArgs((int)CommandEnum.GetConfigCommand, arr, String.Empty);
            TcpAdapter.SendCommandToServer(argsToSend);
        }

        /// <summary>
        /// Initializes the config parameters.
        /// </summary>
        private void InitializeSettings()
        {
            ServiceSettings = new Settings()
            {
                SourceName = string.Empty,
                LogName = string.Empty,
                ThumbnailSize = 0,
                OutputDirectory = string.Empty,
                Handlers = new ObservableCollection<string>(),
            };
        }

        /// <summary>
        /// Deletes the handler listener.
        /// </summary>
        /// <param name="handler"></param>
        public void StopWatchingHandler(string handler)
        {
            try
            {
                string[] arr = { handler };
                CommandRecievedEventArgs eventArgs = new CommandRecievedEventArgs((int)CommandEnum.CloseDirectoryHandlerCommand,
                                arr, String.Empty);
                TcpAdapter.SendCommandToServer(eventArgs);
                ServiceSettings.Handlers.Remove(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Receives the response from the service.
        /// </summary>
        /// <param name="args"></param>
        private void HandleResponse(CommandRecievedEventArgs args)
        {
            try
            {
                    switch (args.CommandId)
                    {
                        case (int) CommandEnum.GetConfigCommand:
                            ParseConfigResponse(args);
                            break;
                        case (int) CommandEnum.CloseDirectoryHandlerCommand:
                            CloseHandler(args);
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
        /// Closes the handler and removes from the list.
        /// </summary>
        /// <param name="args"></param>
        private void CloseHandler(CommandRecievedEventArgs args)
        {
            if (ServiceSettings.Handlers == null || ServiceSettings.Handlers.Count <= 0 || args?.Args == null ||
                !ServiceSettings.Handlers.Contains(args.Args[0])) return;
            ServiceSettings.Handlers.Remove(args.Args[0]);
            NotifyUpdate?.Invoke();
        }

        /// <summary>
        /// Parses the CommandRecievedEventArgs to a Settings object.
        /// </summary>
        /// <param name="args"></param>
        private void ParseConfigResponse(CommandRecievedEventArgs args)
        {
            try
            {
                ServiceSettings.OutputDirectory = args.Args[0];
                ServiceSettings.SourceName = args.Args[1];
                ServiceSettings.LogName = args.Args[2];
                ServiceSettings.ThumbnailSize = int.Parse(args.Args[3]);
                string[] handlers = args.Args[4].Split(';');

                // Adding the handlers:
                foreach (string handler in handlers.Where(handler => !ServiceSettings.Handlers.Contains(handler)).Where(handler => !handler.Equals("")))
                {
                    ServiceSettings.Handlers.Add(handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion     
    }
}