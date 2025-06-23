using System.Text.Json;
using FleetManager.Models;

namespace FleetManager.Services;

public class FileDataService<T> where T : IEntity
{
    private readonly string _filePath;

    public FileDataService(string fileName, IWebHostEnvironment environment)
    {
        string dataDirectory;

        //Development mode
        if (environment.IsDevelopment())
            dataDirectory = Path.Combine(environment.ContentRootPath, "data");

        //Produciton mode
        else
            dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

        Directory.CreateDirectory(dataDirectory);

        _filePath = Path.Combine(dataDirectory, $"{fileName}.json");
    }

    public async Task<List<T>> GetAllAsync()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured while trying to read data: {ex.Message}");
            return new List<T>();
        }
    }
    public async Task<IEnumerable<T>> GetSelectedAsync(Func<T, bool> pred)
    {
        var data = await GetAllAsync();

        var result = data.Where(pred);

        return result;
    }
    public async Task<T?> GetByPredictateAsync(Func<T, bool> pred)
    {
        var data = await GetAllAsync();

        return data.FirstOrDefault(pred);
    }

    public async Task<int> SaveAsync(List<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);

        return 0;
    }

    public async Task<int> AddAsync(T item)
    {
        var data = await GetAllAsync();

        var lastId = data.Count > 0 ? data.Max(x => x.Id) : 0;

        item.Id = lastId + 1;

        data.Add(item);

        return await SaveAsync(data);
    }


    public async Task<int> RemoveByPredictateAsync(Func<T, bool> pred)
    {
        var data = await GetAllAsync();
        var itemToRemove = data.FirstOrDefault(pred);

        if (itemToRemove == null)
            return -1;

        data.Remove(itemToRemove);

        return await SaveAsync(data);
    }

    public async Task<int> UpdateAsync(Func<T, bool> pred, T updatedItem)
    {
        var data = await GetAllAsync();
        var index = data.FindIndex(x => pred(x));

        if (index == -1)
            return -1;

        data[index] = updatedItem;

        return await SaveAsync(data);
    }
}