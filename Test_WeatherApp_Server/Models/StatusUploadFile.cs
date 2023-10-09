namespace Test_WeatherApp_Server.Models
{
    public class StatusUploadFile
    {
        public string? FileName { get; }
        public string? Status { get; }

        public StatusUploadFile(string? fileName, string status)
        {
            FileName = fileName;
            Status = status;
        }
    }
}
