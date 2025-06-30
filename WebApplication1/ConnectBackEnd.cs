using GlobalVariablesRP;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ConnectBackEnd
{
    /// <summary>
    /// Универсальный ответ API для всех запросов
    /// </summary>
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }

        public ApiResponse(bool isSuccess, T data, string errorMessage = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }
    }

    public class ConnectServer : IDisposable
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConnectServer()
        {
            _client = new HttpClient { BaseAddress = new Uri(GlobalVariables.GATEWAY) };
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private StringContent CreateJson<T>(T obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj, _jsonOptions), Encoding.UTF8, "application/json");
        }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        #region Основные методы API

        public async Task<ApiResponse<T>> PostAsync<T>(string path, object data, string jwtToken = null)
        {
            try
            {
                //SetAuthHeader(jwtToken);

                var response = await _client.PostAsync(path, CreateJson(data));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<T>(false, default, $"Error: {response.StatusCode} - {errorContent}");
                }

                var result = await DeserializeResponse<T>(response);
                return new ApiResponse<T>(true, result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(false, default, $"Exception: {ex.Message}");
            }
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string path, object data, string jwtToken = null)
        {
            try
            {
                SetAuthHeader(jwtToken);

                var response = await _client.PutAsync(path, CreateJson(data));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<T>(false, default, $"Error: {response.StatusCode} - {errorContent}");
                }

                var result = await DeserializeResponse<T>(response);
                return new ApiResponse<T>(true, result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(false, default, $"Exception: {ex.Message}");
            }
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string path, string jwtToken = null)
        {
            try
            {
                SetAuthHeader(jwtToken);
                var response = await _client.GetAsync(path);

                // Логирование для отладки
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response received: {content}");

                return await HandleResponse<T>(response);
            }
            catch (HttpRequestException ex)
            {
                return new ApiResponse<T>(false, default, $"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(false, default, $"Unexpected error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string path, string jwtToken = null)
        {
            try
            {
                SetAuthHeader(jwtToken);

                var response = await _client.DeleteAsync(path);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<bool>(false, false, $"Error: {response.StatusCode} - {errorContent}");
                }

                return new ApiResponse<bool>(true, true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, false, $"Exception: {ex.Message}");
            }
        }

        #endregion

        #region Вспомогательные методы

        private void SetAuthHeader(string jwtToken)
        {
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);
            }
            else
            {
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }

        #endregion


        private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<T>(false, default, content);
            }

            try
            {
                // Пытаемся десериализовать как обёрнутый ответ
                var wrapped = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                if (wrapped != null && (wrapped.Data != null || !string.IsNullOrEmpty(wrapped.ErrorMessage)))
                {
                    return wrapped;
                }

                // Если не получилось - десериализуем как прямой ответ
                var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                return new ApiResponse<T>(true, data);
            }
            catch (JsonException ex)
            {
                return new ApiResponse<T>(false, default, $"Invalid JSON format: {ex.Message}\nContent: {content}");
            }
        }
    }
}