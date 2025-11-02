using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(string baseUrl, string token = null)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
    public void SetToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        //response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseData);
    }

    public async Task<string> GetAsync(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
      //  response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
      //  response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseData);
    }

    public async Task<string> PostAsync(string endpoint, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
       // response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    //public async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
    //{
    //    var json = JsonConvert.SerializeObject(data);
    //    var content = new StringContent(json, Encoding.UTF8, "application/json");
    //    var response = await _httpClient.PostAsync(endpoint, content);
    //    response.EnsureSuccessStatusCode();
    //    return response;
    //}
    public async Task<T> PutAsync<T>(string endpoint, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(endpoint, content);
      //  response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseData);
    }

    public async Task<string> PutAsync(string endpoint, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
       // response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
