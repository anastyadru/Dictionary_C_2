﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

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
        private async Task<WeatherData> PrintAsync(string url)
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
        /// Основной метод приложения
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        static async Task Main(string[] args)
        {

            string url;
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

            string weatherType;

            Console.WriteLine("На сколько дней Вы хотите знать прогноз погоды: на 1 день, на 5 дней?");
            weatherType = Console.ReadLine();

            var program = new Program();
            var storage = new Storage();

            if (weatherType != null && weatherType.ToLower() == "на 1 день")
            {
                url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid=d6bfd60ae10dc578300a860f105ed749&units=metric&lang=ru";

                WeatherData weatherData = await program.PrintAsync(url);
                var result = "";

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
                Console.ReadLine();
            }

            else if (weatherType != null && weatherType.ToLower() == "на 5 дней")
            {
                url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid=d6bfd60ae10dc578300a860f105ed749&units=metric&lang=ru";

                WeatherData weatherData = await program.PrintAsync(url);
                var result = "";

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
                Console.ReadLine();
            }

            else
            {
                Console.WriteLine("Некорректный ввод. Пожалуйста, укажите, на сколько дней Вы хотите знать прогноз погоды: на 1 день или на 5 дней.");
            }
            
            var data = storage.WeatherData;
            var path = "weatherdata.dat";
            var bytes = data.GetBytes(); // получаем массив байтов, представляющий сериализованные данные объекта
            File.WriteAllBytes(path, bytes); // записываем массив байтов в файл по указанному пути
            
            
        }
    }
}