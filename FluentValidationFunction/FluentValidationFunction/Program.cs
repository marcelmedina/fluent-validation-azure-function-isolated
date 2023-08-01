using FluentValidationFunction.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, svc) =>
    {
        svc.AddFluentValidation();
    })
    .Build();

host.Run();
