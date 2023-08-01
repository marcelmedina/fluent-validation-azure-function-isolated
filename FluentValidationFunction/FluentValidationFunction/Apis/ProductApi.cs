using FluentValidation;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using FluentValidationFunction.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FluentValidationFunction.Apis
{
    public class ProductApi
    {
        private readonly ILogger<ProductApi> _logger;
        private readonly IValidator<ProductViewModel> _validator;

        public ProductApi(ILogger<ProductApi> log, IValidator<ProductViewModel> validator)
        {
            _logger = log;
            _validator = validator;
        }

        [Function(nameof(AddProductAsync))]
        [OpenApiOperation(operationId: nameof(AddProductAsync), tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ProductViewModel), Description = nameof(ProductViewModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json ", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> AddProductAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation($"{nameof(AddProductAsync)} has been triggered");

            // Deserialize object
            var productViewModel = await req.ReadFromJsonAsync<ProductViewModel>();

            // Validating
            var productValidationResult = await _validator.ValidateAsync(productViewModel);

            if (!productValidationResult.IsValid)
            {
                var responseWithErrors = req.CreateResponse();
                await responseWithErrors.WriteAsJsonAsync(productValidationResult.Errors.Select(e => new
                {
                    e.ErrorCode,
                    e.PropertyName,
                    e.ErrorMessage
                }));
                responseWithErrors.StatusCode = HttpStatusCode.BadRequest;
                return responseWithErrors;
            }

            // TODO: Perform add product

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
