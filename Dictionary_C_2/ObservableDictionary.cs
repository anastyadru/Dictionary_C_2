﻿using System;
using System.Collections.Generic;

namespace Dictionary_C_2
{
    /// <summary>
    /// Класс, вызывающий события при добавлении и удалении объектов из коллекции.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа словаря.</typeparam>
    /// <typeparam name="TValue">Тип значения словаря.</typeparam>
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Событие, возникающее при добавлении элемента в словарь.
        /// </summary>
        public event EventHandler<KeyValuePair<TKey, TValue>> ItemAdded; 
        
        /// <summary>
        /// Событие, возникающее при удалении элемента из словаря.
        /// </summary>
        public event EventHandler<KeyValuePair<TKey, TValue>> ItemRemoved;
        
        /// <summary>
        /// Добавляет элемент с указанным ключом и значением в словарь.
        /// </summary>
        /// <param name="key">Ключ добавляемого элемента.</param>
        /// <param name="value">Значение добавляемого элемента.</param>
        public void Add(TKey key, TValue value)
        {
            cache.Add(key, value);
            ItemAdded?.Invoke(this, new KeyValuePair<TKey, TValue>(key, value)); // вызвано событие ItemAdded и передан в него добавленный элемент
        }
        
        /// <summary>
        /// Удаляет элемент с указанным ключом из словаря.
        /// </summary>
        /// <param name="key">Ключ удаляемого элемента.</param>
        /// <returns>Значение true, если элемент был успешно удален из словаря; в противном случае — значение false.</returns>
        public bool Remove(TKey key)
        {
            if (cache.ContainsKey(key))
            {
                TValue value = cache[key]; // получено значение элемента
                bool result = cache.Remove(key);
                if (result) // вызвано событие ItemRemoved и передан в него удаленный элемент
                {
                    ItemRemoved?.Invoke(this, new KeyValuePair<TKey, TValue>(key, value)); 
                    
                    // вызвано событие ItemAdded и передано в него пара ключ-значение, которая была удалена из словаря
                }
                
                return result;
            }
            
            return false;
        }
        
        /// <summary>
        /// Метод-обработчик события ItemAdded.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Cache_ItemAdded(object sender, KeyValuePair<TKey, TValue> e)
        {
            Console.WriteLine($"Добавлен элемент с ключом {e.Key} и значением {e.Value}");
        }
        
        /// <summary>
        /// Метод-обработчик события ItemRemoved.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void Cache_ItemRemoved(object sender, KeyValuePair<TKey, TValue> e)
        {
            Console.WriteLine($"Удален элемент с ключом {e.Key} и значением {e.Value}");
        }
        
        /// <summary>
        /// Конструктор класса ObservableDictionary.
        /// </summary>
        public ObservableDictionary()
        {
            ItemAdded += Cache_ItemAdded;
            ItemRemoved += Cache_ItemRemoved;
        }

    }
}
