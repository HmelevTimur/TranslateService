using Grpc.Core;

namespace YandexTranslateService.Services.Interfaces;

public interface ITranslationService
{
    Task<TranslateResponse> Translate(TranslateRequest request, ServerCallContext context);
    Task<ServiceInfoResponse> GetServiceInfo(Empty request, ServerCallContext context);
}