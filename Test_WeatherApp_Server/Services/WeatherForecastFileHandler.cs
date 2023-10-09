using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Test_WeatherApp_Server.Context;
using Test_WeatherApp_Server.Interfaces;
using Test_WeatherApp_Server.Models;

namespace Test_WeatherApp_Server.Services
{
    // This service works with files
    // Reading and forming Weather forecast objects to put them in DB
    // And retrieving all current files in server directory
    public class WeatherForecastFileHandler : IWeatherForecastFileHandler
    {
        private readonly WeatherForecastDbContext _dbContext;
        private readonly ILogger<WeatherForecastFileHandler> _logger;

        public WeatherForecastFileHandler(WeatherForecastDbContext dbContext, ILogger<WeatherForecastFileHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Read content from uploaded file
        public async Task<StatusUploadFile> ReadWeatherForecast(IFormFile file)
        {
            // Check if file is right type
            // Currently works only with files from excel 2007+
            if (!(file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                _logger.LogInformation($"Wrong format of a file: {file.Name}");
                return new StatusUploadFile(file.FileName, "WrongFormatFile");
            }

            string filePath = Directory.GetCurrentDirectory() + $"\\wwwroot\\Upload\\{file.FileName}";

            // Check if file was uploaded previously
            if (File.Exists(filePath))
            {
                _logger.LogInformation($"File {file.Name} is already uploaded");
                return new StatusUploadFile(file.FileName, "FileAlreadyUploaded");
            }

            try
            {
                // Save file to directory
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Read its content
                await ReadContent(filePath);

                _logger.LogInformation($"File uploaded: {file.Name} and its content read");
                return new StatusUploadFile(file.FileName, "OK");
            }

            catch
            {
                _logger.LogInformation($"File {file.Name} has wrong content");
                return new StatusUploadFile(file.FileName, "WrongFileContent");
            } 
        }

        // Function to form object from row content
        private WeatherForecast FormWeatherForecast(IRow rowContent, Guid monthId)
        {
            return new WeatherForecast
            {
                Day = rowContent.GetCell(0).ToString() is null
                                ? "нет данных" : rowContent.GetCell(0, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                Time = rowContent.GetCell(1).ToString() is null
                                ? "нет данных" : rowContent.GetCell(1, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                Temprature = rowContent.GetCell(2).ToString() is null
                                ? "нет данных" : rowContent.GetCell(2, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                Humidity = rowContent.GetCell(3).ToString() is null
                                ? "нет данных" : rowContent.GetCell(3, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                DewPoint = rowContent.GetCell(4).ToString() is null
                                ? "нет данных" : rowContent.GetCell(4, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                Pressure = rowContent.GetCell(5).ToString() is null
                                ? "нет данных" : rowContent.GetCell(5, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                WindDirection = rowContent.GetCell(6, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString() is null
                                ? "нет данных" : rowContent.GetCell(6, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                WindSpeed = rowContent.GetCell(7, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString() is null
                                ? "нет данных" : rowContent.GetCell(7, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                Cloudy = rowContent.GetCell(8, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString() is null
                                ? "нет данных" : rowContent.GetCell(8, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                CloudBase = rowContent.GetCell(9, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString() is null
                                ? "нет данных" : rowContent.GetCell(9, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                HorizontalVisability = rowContent.GetCell(10, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString() is null
                                ? "нет данных" : rowContent.GetCell(10, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                WeatherConditions = rowContent.GetCell(11, MissingCellPolicy.RETURN_BLANK_AS_NULL) is null
                                ? "нет данных" : rowContent.GetCell(11, MissingCellPolicy.RETURN_BLANK_AS_NULL).ToString(),
                MonthId = monthId
            };
        }

        // Function to read contetn of a file an form weather forecasts
        private async Task ReadContent(string filePath)
        {
            XSSFWorkbook xssfwb;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                xssfwb = new XSSFWorkbook(file);
            }

            for (int i = 0; i < xssfwb.Count; i++)
            {
                var sheet = xssfwb[i];

                var sheetName = sheet.SheetName;

                var monthAndYear = sheetName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var month = monthAndYear[0];
                var year = Convert.ToUInt32(monthAndYear[1]);

                // Creating month for a specific year
                var monthId = Guid.NewGuid();
                _dbContext.Set<Month>().Add(new Month
                {
                    Id = monthId,
                    Name = month,
                    YearNum = year
                });

                // Read main content of sheets
                for (int row = 0; row <= sheet.LastRowNum; row++)
                {
                    var rowContent = sheet.GetRow(row);

                    if (rowContent != null)
                    {
                        if (DateTime.TryParse(rowContent.GetCell(0).ToString(), out var exactDate))
                        {
                            _dbContext.Set<WeatherForecast>().Add(FormWeatherForecast(rowContent, monthId));
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
