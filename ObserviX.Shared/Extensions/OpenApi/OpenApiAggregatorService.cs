// using Microsoft.OpenApi.Models;
// using Microsoft.OpenApi.Readers;
//
// namespace ObserviX.Shared.Extensions.OpenApi;
//
// public class OpenApiAggregatorService
// {
//     private readonly HttpClient _httpClient;
//     public OpenApiAggregatorService(HttpClient httpClient) => _httpClient = httpClient;
//
//     public async Task<OpenApiDocument> AggregateAsync()
//     {
//         var aggregator = new OpenApiDocument
//         {
//             Info = new OpenApiInfo { Title = "Gateway API", Version = "v1" },
//             Paths = new OpenApiPaths(),
//             Components = new OpenApiComponents()
//         };
//
//         // Service1
//         var service1Doc = await FetchOpenApiDocAsync("http://service1-internal-url/swagger/v1/swagger.json");
//         PrependPaths(service1Doc, "/api/service1");
//         MergeDocuments(aggregator, service1Doc);
//
//         // Service2
//         var service2Doc = await FetchOpenApiDocAsync("http://service2-internal-url/swagger/v1/swagger.json");
//         PrependPaths(service2Doc, "/api/service2");
//         MergeDocuments(aggregator, service2Doc);
//
//         return aggregator;
//     }
//
//     private async Task<ReadResult> FetchOpenApiDocAsync(string url)
//     {
//         var response = await _httpClient.GetAsync(url);
//         var stream = await response.Content.ReadAsStreamAsync();
//         return await new OpenApiStreamReader().ReadAsync(stream);
//     }
//
//     private void PrependPaths(OpenApiDocument doc, string prefix)
//     {
//         var newPaths = new OpenApiPaths();
//         foreach (var path in doc.Paths)
//         {
//             newPaths[$"{prefix}{path.Key}"] = path.Value;
//         }
//         doc.Paths = newPaths;
//     }
//
//     private void MergeDocuments(OpenApiDocument target, OpenApiDocument source)
//     {
//         foreach (var path in source.Paths)
//         {
//             target.Paths.Add(path.Key, path.Value);
//         }
//         // Merge components as needed (schemas, parameters, etc.)
//     }
// }