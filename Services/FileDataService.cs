using System.Text.Json;

namespace FleetManager.Services;

public class FileDataService<T> where T : class
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

    public List<T> GetAll()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            var json = File.ReadAllTextAsync(_filePath).Result;
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
    public T? GetByPredictateAsync(Func<T, bool> pred)
    {
        var data = GetAll();

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
        var data = GetAll();
        data.Add(item);
        return await SaveAsync(data);
    }


    public async Task<int> RemoveByPredictateAsync(Func<T, bool> pred)
    {
        var data = GetAll();
        var itemToRemove = data.FirstOrDefault(pred);

        if (itemToRemove == null)
            return -1;

        data.Remove(itemToRemove);

        return await SaveAsync(data);
    }

    public async Task<int> UpdateAsync(Func<T, bool> pred, T updatedItem)
    {
        var data = GetAll();
        var index = data.FindIndex(x => pred(x));

        if (index == -1)
            return -1;

        data[index] = updatedItem;

        return await SaveAsync(data);
    }
}