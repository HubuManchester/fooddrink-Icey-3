using FoodieTracker.Models;
using Microsoft.Maui.Networking;
using System.Net.Http.Json;
using System.Text.Json;

namespace FoodieTracker.Services;

public static class FoodDataService
{

    private const string BaseApiUrl = "https://6a1c60a58858a003817bd4da.mockapi.io/foodentries";

    private static readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri(BaseApiUrl),
        Timeout = TimeSpan.FromSeconds(15)
    };

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static List<FoodEntry>? _cachedItems;
    private static DateTime _lastCacheTime = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private static Page? GetCurrentPage()
        => Application.Current?.MainPage ?? Shell.Current?.CurrentPage;

    private static async Task SafeDisplayAlert(string title, string message, string cancel = "OK")
    {
        var page = GetCurrentPage();
        if (page != null)
            await page.DisplayAlert(title, message, cancel);
        else
            System.Diagnostics.Debug.WriteLine($"Alert not shown: {title} - {message}");
    }

    private static async Task EnsureCacheAsync()
    {
        if (_cachedItems == null)
            await GetAllAsync();
    }

    public static async Task<IReadOnlyList<FoodEntry>> GetAllAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedItems != null && DateTime.UtcNow - _lastCacheTime < CacheDuration)
            return _cachedItems;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return _cachedItems ?? new List<FoodEntry>();

        try
        {
            var items = await _httpClient.GetFromJsonAsync<List<FoodEntry>>("", _jsonOptions);
            if (items != null)
            {
                _cachedItems = items.OrderBy(i => i.Name).ToList();
                _lastCacheTime = DateTime.UtcNow;
                return _cachedItems;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetAllAsync error: {ex.Message}");
        }

        return _cachedItems ?? new List<FoodEntry>();
    }

    public static async Task<IReadOnlyList<FoodEntry>> SearchAsync(string? query)
    {
        var all = await GetAllAsync();
        if (string.IsNullOrWhiteSpace(query)) return all;
        var q = query.Trim().ToLowerInvariant();
        return all.Where(x =>
            x.Name.ToLowerInvariant().Contains(q) ||
            x.Category.ToLowerInvariant().Contains(q) ||
            x.Description.ToLowerInvariant().Contains(q)
        ).ToList();
    }

    public static async Task<FoodEntry?> GetByIdAsync(string id)
    {
        var cached = _cachedItems?.FirstOrDefault(x => x.Id == id);
        if (cached != null) return cached;

        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                return await _httpClient.GetFromJsonAsync<FoodEntry>($"/{id}", _jsonOptions);
        }
        catch { /* 忽略 */ }
        return null;
    }

    public static async Task<FoodEntry?> AddAsync(FoodEntry entry)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new Exception("无网络连接，无法新增");

        await EnsureCacheAsync();

        var response = await _httpClient.PostAsJsonAsync("", entry, _jsonOptions);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<FoodEntry>(_jsonOptions);
        if (created != null)
        {
            _cachedItems!.Add(created);
            _cachedItems = _cachedItems.OrderBy(x => x.Name).ToList();
            _lastCacheTime = DateTime.UtcNow; // 可选：刷新缓存时间
        }
        return created;
    }

    public static async Task<FoodEntry?> UpdateAsync(FoodEntry entry)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new Exception("无网络连接，无法更新");

        var response = await _httpClient.PutAsJsonAsync($"/{entry.Id}", entry, _jsonOptions);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<FoodEntry>(_jsonOptions);
        if (updated != null && _cachedItems != null)
        {
            var idx = _cachedItems.FindIndex(x => x.Id == entry.Id);
            if (idx >= 0) _cachedItems[idx] = updated;
        }
        return updated;
    }

    public static async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            await SafeDisplayAlert("Delete Failed", "Invalid ID.");
            return false;
        }

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await SafeDisplayAlert("No Internet", "Cannot delete without network connection.");
            return false;
        }

        try
        {
            var url = $"{BaseApiUrl.TrimEnd('/')}/{id}";
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                _cachedItems?.RemoveAll(x => x.Id == id);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Delete failed: {response.StatusCode}, Content: {errorContent}");
                await SafeDisplayAlert("Delete Failed", $"Status: {response.StatusCode}\n{errorContent}");
                return false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete exception: {ex.Message}");
            await SafeDisplayAlert("Error", ex.Message);
            return false;
        }
    }

    public static async Task RefreshCacheAsync() => await GetAllAsync(true);
}