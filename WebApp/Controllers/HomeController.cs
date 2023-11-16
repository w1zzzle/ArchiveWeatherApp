using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using System.Globalization;
using WebApp.Data;
using WebApp.Models;
using PagedList;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private List<WeatherRecord> weatherRecords;


        public HomeController(AppDbContext context)
        {
            _context = context;
        }


        // Получение значения даты из ячейки Excel
        private DateTime GetDateValue(ICell cell)
        {
            if (cell != null)
            {
                if (cell.CellType == CellType.Numeric)
                {
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return DateTime.ParseExact(cell.DateCellValue.ToString("dd.MM.yyyy"), "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    }
                }
                else if (cell.CellType == CellType.String && DateTime.TryParseExact(cell.StringCellValue, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            return DateTime.MinValue;
        }


        // Получение числового значения из ячейки Excel
        private double GetNumericCellValue(ICell cell)
        {
            return cell != null && cell.CellType == CellType.Numeric ? cell.NumericCellValue : 0.0;
        }

        // Получение целочисленного значения из ячейки Excel
        private int GetIntCellValue(ICell cell)
        {
            return cell != null && cell.CellType == CellType.Numeric ? (int)cell.NumericCellValue : 0;
        }


        // Добавляем метод для отображения главной страницы
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // Добавляем метод для отображения страницы загрузки погоды
        [HttpGet]
        public IActionResult UploadWeather(IFormFile file, int? year, int? month, int? page)
        {
            if (!_context.WeatherRecord.Any())
            {
                _context.Database.ExecuteSqlRaw("DELETE FROM WeatherRecord");
            }

            var weatherRecords = _context.WeatherRecord.AsNoTracking().ToList();

            if (year.HasValue)
            {
                weatherRecords = weatherRecords.Where(w => w.Date.Year == year.Value).ToList();
            }

            if (month.HasValue)
            {
                weatherRecords = weatherRecords.Where(w => w.Date.Month == month.Value).ToList();
            }

            int pageNumber = page ?? 1;
            int pageSize = 50;
            var pagedWeatherRecords = weatherRecords.ToPagedList(pageNumber, pageSize);

            return View(pagedWeatherRecords);
        }


        // Парсинг времени с дополнительной проверкой
        private static string TryParseTime(string timeString)
        {
            if (timeString == "00:00")
            {
                return "00:00";
            }

            if (TimeSpan.TryParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture, out var timeSpan))
            {
                return timeSpan.ToString(@"hh\:mm");
            }

            return timeString;
        }


        // Отображение данных о погоде на странице
        public IActionResult ViewWeather(int? year, int? month, int page = 1)
        {
            int defaultPageSize = 30;
            var allWeatherRecords = _context.WeatherRecord.AsNoTracking();

            if (year.HasValue)
            {
                allWeatherRecords = allWeatherRecords.Where(w => w.Date.Year == year.Value);
            }

            if (month.HasValue)
            {
                allWeatherRecords = allWeatherRecords.Where(w => w.Date.Month == month.Value);
            }

            int pageNumber = page;
            int pageSize = defaultPageSize;
            var paginatedWeatherRecords = allWeatherRecords.ToPagedList(pageNumber, pageSize);

            ViewData["SelectedYear"] = year;
            ViewData["SelectedMonth"] = month;

            return View(paginatedWeatherRecords);
        }

        [HttpPost]
        public async Task<IActionResult> UploadWeather(List<IFormFile> files, int? page)
        {
            var weatherRecords = new List<WeatherRecord>();
            ProcessUploadedFiles(files);
            TempData.Keep("ViewWeatherLink");

            // Если есть обработанные записи о погоде, добавляем их в базу данных и отображаем результат
            if (weatherRecords.Any())
            {
                _context.WeatherRecord.AddRange(weatherRecords);
                await _context.SaveChangesAsync();

                int pageNumber1 = (page ?? 1);
                int pageSize1 = 50;
                var pagedWeatherRecords1 = weatherRecords.ToPagedList(pageNumber1, pageSize1);

                TempData["ViewWeatherLink"] = Url.Action("ViewWeather");

                return View(pagedWeatherRecords1);
            }

            return View();
        }


        // Метод для обработки данных о погоде из листа Excel
        // sheet: Лист Excel с данными о погоде
        // weatherRecords: Список для добавления обработанных записей о погоде
        private void ProcessWeatherData(ISheet sheet, List<WeatherRecord> weatherRecords)
        {

            // Итерация по строкам листа Excel, начиная с 4-й строки
            for (int i = 4; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);


                // Создание объекта WeatherRecord и заполнение его данными из текущей строки
                var record = new WeatherRecord
                {
                    Date = GetDateValue(row.GetCell(0)),
                    Time = TryParseTime(row.GetCell(1).StringCellValue),
                    Temperature = GetNumericCellValue(row.GetCell(2)),
                    RelativeHumidity = row.GetCell(3) != null ? GetIntCellValue(row.GetCell(3)) : 0,
                    DewPoint = GetNumericCellValue(row.GetCell(4)),
                    AtmosphericPressure = GetNumericCellValue(row.GetCell(5)),
                    WindDirection = row.GetCell(6) != null ? row.GetCell(6).StringCellValue : null,
                    WindSpeed = GetNumericCellValue(row.GetCell(7)),
                    Cloudiness = row.GetCell(8) != null ? GetIntCellValue(row.GetCell(8)) : 0,
                    CloudHeight = row.GetCell(9) != null ? GetIntCellValue(row.GetCell(9)) : 0,
                    Visibility = row.GetCell(10) != null ? GetNumericCellValue(row.GetCell(10)) : (double?)null,
                    WeatherPhenomenon = !string.IsNullOrEmpty(row.GetCell(11)?.StringCellValue) ? row.GetCell(11).StringCellValue : " ",
                };
                weatherRecords.Add(record);
            }
        }

        private void ProcessUploadedFiles(List<IFormFile> files)
        {
            var validFiles = new List<string>();
            var invalidFiles = new List<string>();

            foreach (var file in files)
            {
            var weatherRecords = new List<WeatherRecord>();

                bool isValidFile = true;

                using (var stream = file.OpenReadStream())
                {
                    IWorkbook workbook;
                    try
                    {
                        workbook = WorkbookFactory.Create(stream);
                    }
                    catch (Exception ex)
                    {
                        invalidFiles.Add($"Ошибка при открытии файла {file.FileName}: {ex.Message}");
                        isValidFile = false;
                        continue;
                    }

                    foreach (ISheet sheet in workbook)
                    {
                        IRow headerRow = sheet.GetRow(2);
                        if (!IsHeaderValid(headerRow))
                        {
                            isValidFile = false;
                            invalidFiles.Add($"Неверный Excel файл ({file.FileName}): неверный заголовок на листе {sheet.SheetName}.");
                            break;
                        }

                        ProcessWeatherData(sheet, weatherRecords);
                    }
                }

                if (isValidFile)
                {
                    validFiles.Add(file.FileName);
                }


                if (validFiles.Count > 0)
                {
                    TempData["SuccessMessage"] = $"Верные файлы ({string.Join(", ", validFiles)}) загружены.";
                }

                if (invalidFiles.Count > 0)
                {
                    TempData["InvalidFileMessage"] = $"Некоторые файлы не были загружены: {string.Join(", ", invalidFiles)}";
                }

                _context.WeatherRecord.AddRange(weatherRecords);
                _context.SaveChanges();

                TempData["FilesUploaded"] = weatherRecords.Any();
                TempData.Keep("ViewWeatherLink"); // добавьте эту строку
            }
        }


        // Проверка валидности заголовка файла Excel
        public bool IsHeaderValid(IRow headerRow)
        {
            string[] expectedHeaders = { "Дата", "Время", "Т", "Отн. влажность", "Td", "Атм. давление,", "Направление", "Скорость", "Облачность,", "h", "VV", "Погодные явления" };

            if (headerRow == null || headerRow.Cells.Count != expectedHeaders.Length)
            {
                return false;
            }

            for (int i = 0; i < expectedHeaders.Length; i++)
            {
                if (headerRow.GetCell(i)?.StringCellValue != expectedHeaders[i])
                {
                    return false;
                }
            }

            return true;
        }


        // Очистка данных о погоде
        [HttpPost]
        public IActionResult ClearWeatherData()
        {
          //_context.Database.ExecuteSqlRaw("DELETE FROM WeatherRecord");
            _context.RemoveRange(_context.WeatherRecord);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
