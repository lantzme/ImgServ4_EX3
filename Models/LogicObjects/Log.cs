namespace ImageService.Web.Models.LogicObjects
{

    #region Enums
    public enum LogType
    {
        INFO,
        FAIL,
        WARNING
    }
    #endregion

    public class Log
    {
        #region Properties
        public string LogType { get; set; }
        public string Message { get; set; }
        #endregion
    }
}