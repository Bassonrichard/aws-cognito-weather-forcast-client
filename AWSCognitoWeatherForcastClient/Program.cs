using AWSCognitoWeatherForcastClient;
using AWSCognitoWeatherForcastClient.Configuration;
using AWSCognitoWeatherForcastClient.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<AWSCognitoConfig>(hostContext.Configuration.GetSection(key: AWSCognitoConfig.Name));

        services.AddHttpClient("CommonApplication", client =>
         {
             client.BaseAddress = new Uri("https://localhost:5001/");
         });

        services.AddTransient<ICommonApplicationService, CommonApplicationService>();

        services.AddHostedService<Worker>();
    }).Build();

await host.RunAsync();
