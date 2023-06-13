﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dictionary_C_2
{
    
    /// <summary>
    /// Класс, содержащий точку входа в программу.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Асинхронно получает данные о погоде по указанному URL и возвращает объект WeatherData.
        /// </summary>
        /// <param name="url">URL для получения данных о погоде.</param>
        /// <returns>Объект WeatherData, содержащий данные о погоде.</returns>
        private static async Task<WeatherData> GetWeatherDataAsync(string url)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(responseBody);
                    return weatherData;
                }

                return null;
            }
        }

        /// <summary>
        /// Получает название города от пользователя.
        /// </summary>
        /// <returns>Название города.</returns>
        private static string GetCityName()
        {
            string cityName;
            
            try
            {
                Console.WriteLine("Введите, для какого города прогноз погоды: Minsk, London, Paris, NewYork, Warsaw");
                cityName = Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Такого города не существует в списке. Введите город вручную: ");
                cityName = Console.ReadLine();
            }

            return cityName;
        }

        /// <summary>
        /// Получает тип прогноза погоды от пользователя.
        /// </summary>
        /// <returns>Тип прогноза погоды (1 - на 1 день, 5 - на 5 дней).</returns>
        private static int GetWeatherType()
        {
            Console.WriteLine("На сколько дней Вы хотите знать прогноз погоды: на 1 день, на 5 дней?"); 
            var weatherType = int.Parse(Console.ReadLine());
            return weatherType;
        }
        
        /// <summary>
        /// Выводит на консоль данные о текущей погоде в указанном городе.
        /// </summary>
        /// <param name="weatherData">Объект WeatherData, содержащий данные о погоде.</param>
        /// <param name="cityName">Название города.</param>
        private static void PrintCurrentWeather(WeatherData weatherData, string cityName)
        {
            var result = "";
            
            var storage = new Storage();
            
            if (weatherData != null)
            {
                result += $"Прогноз погоды в городе {cityName} на сегодня: \n";
                result += $"Температура: {weatherData.Data.Temp}°C\n";
                result += $"Температура ощущается на: {weatherData.Data.FeelsLike}°C\n";
                result += $"Давление: {weatherData.Data.Pressure}Pa\n";
                result += $"Влажность: {weatherData.Data.Humidity}%\n";
                
                storage.WeatherData.Add(cityName, weatherData);
            }
            else
            {
                result += $"Ошибка получения данных о погоде в городе {cityName}\n";
            }

            Console.WriteLine(result);
        }

        /// <summary>
        /// Выводит на консоль данные о прогнозе погоды на 5 дней в указанном городе.
        /// </summary>
        /// <param name="weatherData">Объект WeatherData, содержащий данные о погоде.</param>
        /// <param name="cityName">Название города.</param>
        private static void PrintWeatherForecast(WeatherData weatherData, string cityName)
        {
            var result = "";
            
            var storage = new Storage();
            
            if (weatherData != null)
            {
                result += $"Прогноз погоды в городе {cityName} на 5 дней: \n";
                for (int i = 0; i < weatherData.ForecastList.Count; i++)
                {
                    Forecast forecast = weatherData.ForecastList[i];
                    result += $"День {i + 1}: \n";
                    result += $"Дата: {forecast.Date}\n";
                    result += $"Температура: {forecast.Temp}°C\n";
                    result += $"Температура ощущается на: {forecast.FeelsLike}°C\n";
                    result += $"Давление: {forecast.Pressure}Pa\n";
                    result += $"Влажность: {forecast.Humidity}%\n";
                    
                    storage.WeatherData.Add(cityName, weatherData);
                }
            }
            else
            {
                result += $"Ошибка получения данных о погоде в городе {cityName}\n";
            }

            Console.WriteLine(result);
        }

        /// <summary>
        /// Основной метод приложения
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        private static async Task Main(string[] args)
        {
            var cityName = GetCityName();
            var weatherType = GetWeatherType();
            
            if (weatherType == 1)
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid=d6bfd60ae10dc578300a860f105ed749&units=metric&lang=ru";
                WeatherData weatherData = await GetWeatherDataAsync(url);
                PrintCurrentWeather(weatherData, cityName);
            }
            else if (weatherType == 5)
            {
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid=d6bfd60ae10dc578300a860f105ed749&units=metric&lang=ru";
                WeatherData weatherData = await GetWeatherDataAsync(url);
                PrintWeatherForecast(weatherData, cityName);
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Пожалуйста, укажите, на сколько дней Вы хотите знать прогноз погоды: на 1 день или на 5 дней.");
            }

            Console.ReadLine();
        }


 
            
        // Метод сериализации данных
        void SerializeData(Dictionary<string, WeatherData> data, string path)
        {
            var formatter = new BinaryFormatter();
            using var stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, data.ToDictionary(x => x.Key, x => x.Value.Data));
        }

        // Метод десериализации данных
        Dictionary<string, WeatherData> DeserializeData(string path)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var data = (Dictionary<string, WeatherData.Data>)formatter.Deserialize(stream);
                return data.ToDictionary(x => x.Key, x => new WeatherData { Data = x.Value });
            }
        }
            
        // Использование методов сериализации и десериализации
        var data = storage.WeatherData; // Получаем данные из хранилища
        string path = "weatherdata.dat"; // Указываем путь к файлу для сохранения данных

        SerializeData(data, path); // Сериализуем данные и записываем их в файл

        var loadedData = DeserializeData(path); // Десериализуем данные из файла

        // Выводим результаты
        Console.WriteLine($"Загружено {loadedData.Count} записей из файла.");

        foreach (var item in loadedData)
        {
            Console.WriteLine($"Город: {item.Key}");
            Console.WriteLine($"Данные: {item.Value.Data}");
        }
            
        // Обработка событий при добавлении и удалении элементов из ObservableDictionary
        storage.WeatherData.ItemAdded += CacheItemAdded;
        storage.WeatherData.ItemRemoved += CacheItemRemoved;

        void CacheItemAdded(object sender, KeyValuePair<string, WeatherData> e)
        {
            Console.WriteLine($"Добавлен элемент с ключом {e.Key} и значением {e.Value.Data}");
        }

        void CacheItemRemoved(object sender, KeyValuePair<string, WeatherData> e)
        {
            Console.WriteLine($"Удален элемент с ключом {e.Key} и значением {e.Value.Data}");
        }
    }
}