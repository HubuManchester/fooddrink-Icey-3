using FoodieTracker.Models;
using Microsoft.Maui.Networking;
using System.Net.Http.Json;
using System.Text;
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

    // 内存缓存
    private static List<FoodEntry>? _cachedItems;
    private static DateTime _lastCacheTime = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    // 获取全部（带缓存）
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

    // 搜索（基于缓存）
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

    // 根据 ID 获取
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

    // 新增
    public static async Task<FoodEntry?> AddAsync(FoodEntry entry)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new Exception("无网络连接，无法新增");

        var response = await _httpClient.PostAsJsonAsync("", entry, _jsonOptions);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<FoodEntry>(_jsonOptions);
        if (created != null && _cachedItems != null)
        {
            _cachedItems.Add(created);
            _cachedItems = _cachedItems.OrderBy(x => x.Name).ToList();
        }
        return created;
    }

    // 更新
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

    // 删除
    public static async Task<bool> DeleteAsync(string id)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new Exception("无网络连接，无法删除");

        var response = await _httpClient.DeleteAsync($"/{id}");
        response.EnsureSuccessStatusCode();
        _cachedItems?.RemoveAll(x => x.Id == id);
        return true;
    }

    // 强制刷新缓存
    public static async Task RefreshCacheAsync() => await GetAllAsync(true);
}