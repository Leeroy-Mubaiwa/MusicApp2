using System.Text.Json;
using MusicApp2.Models;

namespace MusicApp2.Services
{
    public class DataService
    {
        private readonly string _dataPath;
        private readonly IWebHostEnvironment _environment;

        public DataService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _dataPath = Path.Combine(_environment.ContentRootPath, "Data");
            Directory.CreateDirectory(_dataPath);
        }

        private string GetFilePath<T>() where T : class
        {
            return Path.Combine(_dataPath, $"{typeof(T).Name}.json");
        }

        public async Task<List<T>> GetAll<T>() where T : class
        {
            var filePath = GetFilePath<T>();
            if (!File.Exists(filePath))
                return new List<T>();

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public async Task Save<T>(List<T> items) where T : class
        {
            var filePath = GetFilePath<T>();
            var json = JsonSerializer.Serialize(items);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task Add<T>(T item) where T : class
        {
            var items = await GetAll<T>();
            items.Add(item);
            await Save(items);
        }

        public async Task Update<T>(T item, Func<T, bool> predicate) where T : class
        {
            var items = await GetAll<T>();
            var index = items.FindIndex(new Predicate<T>(predicate));
            if (index != -1)
            {
                items[index] = item;
                await Save(items);
            }
        }
    }
}