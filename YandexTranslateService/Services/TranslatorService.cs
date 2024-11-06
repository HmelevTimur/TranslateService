using Grpc.Core;
using Newtonsoft.Json;
using RestSharp;
using YandexTranslateService.Cache;
using YandexTranslateService.Dto;
using YandexTranslateService.Services.Interfaces;

namespace YandexTranslateService.Services
{
    public class TranslatorService : Translator.TranslatorBase, ITranslationService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly ICache _cache;
        private readonly RestClient _client;

        public TranslatorService(string apiKey, string baseUrl, ICache cache, RestClient client)
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl;
            _cache = cache;
            _client = client;
        }

        public override Task<ServiceInfoResponse> GetServiceInfo(Empty request, ServerCallContext context)
        {
            return Task.FromResult(GetServiceInfoInternal());
        }

        private ServiceInfoResponse GetServiceInfoInternal()
        {
            try
            {
                var serviceInfo = new ServiceInfoResponse
                {
                    ExternalService = _baseUrl,
                    CacheType = _cache.GetType().Name,
                    CacheSize = _cache.GetAllValues().Count
                };

                return serviceInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServiceInfo: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal,
                    "Internal server error while getting service info."));
            }
        }

        public override async Task<TranslateResponse> Translate(TranslateRequest request, ServerCallContext context)
        {
            var translations = new List<string>();
            try
            {
                foreach (var text in request.Texts)
                {
                    var cachedTranslation = GetCachedTranslation(text, request.SourceLang);
                    if (cachedTranslation != null)
                    {
                        translations.Add(cachedTranslation);
                    }
                    else
                    {
                        var translation = await TranslateText(text, request.SourceLang, request.TargetLang);
                        translations.Add(translation);
                    }
                }

                return new TranslateResponse { Translations = { translations } };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Translate method: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error during translation."));
            }
        }

        private string GetCachedTranslation(string text, string sourceLang)
        {
            return _cache.GetValue(text, sourceLang) as string;
        }

        private async Task<string> TranslateText(string text, string sourceLang, string targetLang)
        {
            var apiRequest = new RestRequest("translate", Method.Post);
            apiRequest.AddHeader("Authorization", $"Api-Key {_apiKey}");
            apiRequest.AddJsonBody(new
            {
                targetLanguageCode = targetLang,
                sourceLanguageCode = sourceLang,
                texts = new[] { text }
            });

            try
            {
                var response = await _client.ExecuteAsync(apiRequest);

                return HandleApiResponse(response, text, sourceLang);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        private string HandleApiResponse(RestResponse response, string text, string sourceLang)
        {
            if (response.IsSuccessful)
            {
                var jsonResponse = JsonConvert.DeserializeObject<YandexTranslateResponse>(response.Content);
                foreach (var translation in jsonResponse.Translations)
                {
                    _cache.SetValue(text, sourceLang, translation.Text);
                    return translation.Text;
                }
            }
            else
            {
                return $"Error: {response.ErrorMessage} - {response.Content}";
            }

            return null;
        }
    }
}