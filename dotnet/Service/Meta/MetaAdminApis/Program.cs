using Colorverse.Apis.Auth.Extensions;
using Colorverse.Apis.Extensions;
using Colorverse.Apis.Logger.Extensions;
using Colorverse.Apis.Swagger;
using Colorverse.Apis.Swagger.Extensions;
using Colorverse.MetaAdmin;
using Colorverse.MetaAdmin.Config;
using CvFramework.Bson;
using CvFramework.Apis;

/// <summary>
/// 
/// </summary>
public class Program : ProgramBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static async Task Main(string[] args)
    {
        if (SwaggerConfig.RUN_CLI)
        {
            var appConfig = new AppConfig();
            SwaggerCliApp.Run(appConfig, args);
            return;
        }

        var program = new Program();
        await program.RunAsync(args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected override Task OnRunAsync(string[] args)
    {
        //------------------------------
        //- Default
        //------------------------------
        BsonInitializer.Init();

        //------------------------------
        //- App Builder
        //------------------------------
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration<AppConfig>();
        
        //------------------------------
        //- Logger
        //------------------------------
        builder.AddDeafultSerilogWithSentry(o => {
            o.Dsn = config.Current.SentryUrl;
            o.Release = $"{config.ApiPath}@{config.Version}";
        });

        //------------------------------
        //- Framework
        //------------------------------
        builder.AddDefaultFramework(this.RunAssembly);

        //------------------------------
        // Authorization and Authentication
        //------------------------------
        builder.Services.AddDefaultAuthorization();
        builder.Services.AddDefaultAuthentication(builder.Configuration);
        builder.Services.AddDefaultHttpContextUser();

        //------------------------------
        //- Debug only
        //------------------------------
        #if DEBUG
        DebugModeExtensions.DebugRuntime(builder);
        #endif

        //------------------------------
        //- Swagger
        //------------------------------
        if(!builder.Environment.IsProduction())
        {
            builder.Services.AddSwagger(builder.Configuration, config, this.RunAssembly);
        }

        //------------------------------
        //- Apis setup
        //------------------------------
        AppSetup.Setup(builder);

        //------------------------------
        //- Controllers
        //------------------------------
        builder.Services.ConfigureRouteOptions();
        builder.Services.AddDefaultController();

        //------------------------------
        //- App build
        //------------------------------
        var app = builder.Build();
        SetLoggerFactory(app.Services.GetRequiredService<ILoggerFactory>());

        //------------------------------
        //- Framework
        //------------------------------
        app.UseDefaultFramework();

        //------------------------------
        //- Swagger
        //------------------------------
        if(!app.Environment.IsProduction())
        {
            app.UseDefaultSwagger();
        }

        //------------------------------
        //- Endpoints
        //------------------------------
        app.UseDefaultEndpoints();

        //------------------------------
        //- Run
        //------------------------------
        AppBoot.Before(app);

        app.Run();

        return Task.CompletedTask;
    }
}

