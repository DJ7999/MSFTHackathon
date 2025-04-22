using ChatbotBackend;
using ChatbotBackend.Agent;
using ChatbotBackend.AgentRouter;
using ChatbotBackend.EntityFramework;
using ChatbotBackend.Helper;
using ChatbotBackend.Manager;
using ChatbotBackend.MarkDown;
using ChatbotBackend.Plugins;
using ChatbotBackend.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var builder = WebApplication.CreateBuilder(args);

// Enable SignalR
builder.Services.AddSignalR();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=chatbot.db"));  // File-based SQLite DB
// Register Semantic Kernel
builder.Services.AddScoped(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: configuration["AzureOpenAI:DeploymentName"],
        apiKey: configuration["AzureOpenAI:ApiKey"],
        endpoint: configuration["AzureOpenAI:Endpoint"]
    );
    kernelBuilder.AddGoogleTextSearch(configuration["GoogleSearch:SearchEngineId"], configuration["GoogleSearch:APIKey"]);

    return kernelBuilder.Build();
});

// Register services and plugins
builder.Services.AddScoped<IAgentRouter, AgentRouter>();
builder.Services.AddScoped<UserContext>();
builder.Services.AddScoped<ISessionManager, SessionManager>();
builder.Services.AddScoped<IMarkDownGenerator, MarkDownGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRetirementRepository, RetirementRepository>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<TextSearchHelper>();
builder.Services.AddScoped<MarketReaserchPlugin>();
builder.Services.AddScoped<FinancialMetricsPlugin>();
builder.Services.AddScoped<SessionManagerPlugin>();
builder.Services.AddScoped<RetirementPlanningAgent>();
builder.Services.AddScoped<RetirementPlugin>();
builder.Services.AddScoped<UserInfoPlugin>();
builder.Services.AddScoped<GoalPlugin>();  
builder.Services.AddScoped<PortfolioPlugin>();
builder.Services.AddScoped<GoalPlanningAgent>();
builder.Services.AddScoped<FinanceAgent>();
builder.Services.AddScoped<MarketReaserchAnalystAgent>();
builder.Services.AddScoped<PortfolioManagerAgent>();

// Factory to resolve IAgent from name
builder.Services.AddTransient<Func<string, IAgent?>>(sp =>
{
    return className =>
    {
        var type = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(x => x.GetTypes())
                            .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase)
                                              && typeof(IAgent).IsAssignableFrom(t));

        if (type == null)
            return null;

        return (IAgent)sp.GetRequiredService(type);
    };
});

var app = builder.Build();

app.UseCors();

app.MapHub<ChatHub>("/chat");

app.Run();
