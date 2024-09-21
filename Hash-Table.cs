using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography; //Для SHA256
using System.Text;
using System.Diagnostics;
using System.IO;

namespace hash_table
{
    public class Item //Объект HashTable
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public Item(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) //Проверка на пустое
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            Key = key;
            Value = value;
        }
    }

    public class HashTable
    {
        private readonly byte _maxSize = 255; //Максимальный размер таблицы
        private Dictionary<string, List<Item>> _items = null; //Список ключ значение
        public IReadOnlyCollection<KeyValuePair<string, List<Item>>> Items => _items.ToList().AsReadOnly(); //Для вывода всей таблицы
        public HashTable()
        {
            _items = new Dictionary<string, List<Item>>(_maxSize);
        }
        public void Insert(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            var item = new Item(key, value);
            var hash = GetHash(item.Key); //Создаём Хэш ключа

            List<Item> hashTableItem = null;
            if (_items.ContainsKey(hash))
            {
                hashTableItem = _items[hash];
                _items[hash].Add(item);

            }
            else {
                hashTableItem = new List<Item> { item };
                _items.Add(hash, hashTableItem);
            }
        }
        public void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var hash = GetHash(key);

            if (!_items.ContainsKey(hash))
            {
                return;
            }

            var hashTableItem = _items[hash];//получаем ключ-значение

            var item = hashTableItem.SingleOrDefault(i => i.Key == key);//получаем значение
            if (item != null)
            {
                hashTableItem.Remove(item);
            }

        }
        public string Search(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var hash = GetHash(key);

            if (!_items.ContainsKey(hash))
            {
                return null;
            }

            var hashTableItem = _items[hash];

            if (hashTableItem != null)
            {
                var item = hashTableItem.SingleOrDefault(i => i.Key == key);

                if (item != null)
                {
                     return item.Value;
                }
            }
             return null;
        }
        private string GetHash(string value) //Для создания хеша ключа
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(value);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var all_time = Stopwatch.StartNew();//Время
            var hashTable = new HashTable(); //Данные из файла в HashTable
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            using (var reader = new StreamReader(@"random_data.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
            }
            for (int i = 0; i < listA.Count; i++) 
            {
                hashTable.Insert(listA[i], listB[i]);
            }

            ShowHashTable(hashTable, "================================Hashtable========================================");
            Console.ReadLine();
            var watch = Stopwatch.StartNew();
            hashTable.Delete("cfzwqhaexs");
            watch.Stop();
            using (StreamWriter writer = new StreamWriter("HashTable_time.txt", false))
            {
                writer.WriteLineAsync($"Время удаления из таблицы: {watch.ElapsedMilliseconds}ms");
            }
            ShowHashTable(hashTable, "================================Deleted!=========================================================");
            Console.ReadLine();

            Console.WriteLine("srqcifgtoq");
            watch = Stopwatch.StartNew();
            var text = hashTable.Search("srqcifgtoq");
            watch.Stop();
            using (StreamWriter writer = new StreamWriter("HashTable_time.txt", true))
            {
                writer.WriteLineAsync($"Время Поиска в таблице: {watch.ElapsedMilliseconds}ms");
            }
            Console.WriteLine(text);
            all_time.Stop();
            using (StreamWriter writer = new StreamWriter("HashTable_time.txt", true))
            {
                writer.WriteLineAsync($"Общее время работы программы: {all_time.ElapsedMilliseconds}ms");
            }
            Console.ReadLine();
        }
        private static void ShowHashTable(HashTable hashTable, string title) //Выводит весь словарь
        {
            if (hashTable == null)
            {
                throw new ArgumentNullException(nameof(hashTable));
            }
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }
            
            Console.WriteLine(title);

            foreach (var item in hashTable.Items)
            {
                Console.WriteLine(item.Key);
                foreach (var value in item.Value)
                {
                    Console.WriteLine($"\t{value.Key} - {value.Value}");
                }
            }
            Console.WriteLine();
        }
    }
}
