using System;

namespace Dictionary_C_2
{
    /// <summary>
    /// Представляет данные.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Получает или задает температуру.
        /// </summary>
        public double Temp { get; set; }
        
        /// <summary>
        /// Получает или задает температуру "как ощущается".
        /// </summary>
        public double FeelsLike { get; set; }
        
        /// <summary>
        /// Получает или задает давление.
        /// </summary>
        public int Pressure { get; set; }
        
        /// <summary>
        /// Получает или задает влажность.
        /// </summary>
        public int Humidity { get; set; }
    }
}