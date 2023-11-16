# Архивы погодных условий в Москве

## Установка

### Шаг 1: Установка .NET Core SDK

Убедитесь, что у вас установлен .NET Core SDK. Если нет, скачайте его с [официального сайта](https://dotnet.microsoft.com/download).

### Шаг 2: Клонирование репозитория

```sh
bash
git clone https://github.com/ваш-локальный-путь/ваш-репозиторий.git
cd ваш-проект
```

### Шаг 3: Установка зависимостей
```sh
dotnet restore
```

### Шаг 4: Настройка строки подключения к базе данных
В файле appsettings.json укажите строку подключения к вашей MS SQL Server базе данных.
```sh
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=YourDatabaseName;Trusted_Connection=True;"
}
```
### Шаг 5: Применение миграций
```sh
dotnet ef database update
```

### Шаг 6: Запуск приложения
```sh
dotnet run
```

### Примечание:
- Вам нужно заменить ```YourDatabaseName``` на фактическое имя вашей базы данных.
- Уточните строку подключения в зависимости от вашей конфигурации MS SQL Server.
- Убедитесь, что инструменты управления миграциями ```dotnet ef``` установлены. Если нет, установите их с помощью ```dotnet tool install --global dotnet-ef```

## Использование

### 1. Главная страница
На главной странице вашего приложения есть меню с двумя пунктами:

- Просмотр архивов 
Перейдите на эту страницу для просмотра архивов погодных условий в городе Москве. Здесь вы найдете постраничную навигацию и сможете просматривать погодные условия по месяцам и годам.

- Загрузка архивов 
На этой странице вы можете загружать архивы погодных условий в городе Москве. Архивы представляют собой файлы Excel. Вы можете загружать как один файл, так и несколько за раз. Приложение разберет файл Excel и загрузит данные в базу данных.

### 2. Страница просмотра архивов погодных условий
На этой странице присутствует постраничная навигация, позволяющая вам просматривать погодные условия по месяцам и годам.

### 3. Страница загрузки архивов погодных условий
На данной странице вы можете загружать архивы погодных условий в городе Москве. Следуйте инструкциям на странице для загрузки файлов Excel.

## Документация к коду

### Импорты используемых библиотек и пространств имен
```c
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using System.Globalization;
using WebApp.Data;
using WebApp.Models;
using PagedList;
```

### Метод для получения даты из ячейки Excel
```c
        private DateTime GetDateValue(ICell cell)
        {
            // ... (код метода)
        }
```
### Метод для получения числового значения из ячейки Excel
```c
        private double GetNumericCellValue(ICell cell)
        {
            // ... (код метода)
        }
```
### Метод для получения целочисленного значения из ячейки Excel
```c
        private int GetIntCellValue(ICell cell)
        {
            // ... (код метода)
        }
```
### Добавляем метод для отображения главной страницы
```c
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
```
###  Добавляем метод для отображения страницы загрузки погоды
```c
        [HttpGet]
        public IActionResult UploadWeather(IFormFile file, int? year, int? month, int? page)
        {
            // ... (код метода)
        }
```
###  Метод для преобразования строки времени
```c
        private static string TryParseTime(string timeString)
        {
            // ... (код метода)
        }
```
###  Метод для просмотра погодных условий с постраничной навигацией
```c
        public IActionResult ViewWeather(int? year, int? month, int page = 1)
        {
            // ... (код метода)
        }
```
###  Метод для загрузки погодных условий из файлов Excel
```c
        [HttpPost]
        public IActionResult UploadWeather(List<IFormFile> files, int? page)
        {
            // ... (код метода)
        }
```
###   Метод для очистки данных о погоде
```c
        [HttpPost]
        public IActionResult ClearWeatherData()
        {
            // ... (код метода)
        }
```
