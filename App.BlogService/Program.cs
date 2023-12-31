using Microsoft.EntityFrameworkCore;
using AspNetCoreRateLimit;
using App.DataAccess.BlogDbContext;
using Polly;
using Polly.Extensions.Http;
using App.Entity.Interface;
using App.Entity.Database;
using App.Utility;
using App.BlogService.Repositories;
using App.BlogService.Commands;
using MediatR;
using App.BlogService.Handlers;
using App.Entity;

var builder = WebApplication.CreateBuilder(args);

// Configuration setup
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

// Initialize ConnectionHelper
ConnectionHelper.Initialize(configuration);

var DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<ICommentService, CommentService>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<BlogPost>());
builder.Services.AddTransient<IRequestHandler<GetPostByIDCommand, BlogPost>, GetPostByIDHandler>();
builder.Services.AddTransient<IRequestHandler<GetAllPostCommand, List<BlogPost>>, GetAllPostsHandler>();
builder.Services.AddTransient<IRequestHandler<CreatePostCommand, BlogPost>, CreatePostHandler>();
builder.Services.AddTransient<IRequestHandler<UpdatePostCommand, ApiResponse<string>>, UpdatePostHandler>();
builder.Services.AddTransient<IRequestHandler<DeletePostCommand, ApiResponse<string>>, DeletePostHandler>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddMemoryCache();
// Add services to the container.
builder.Services.AddControllers()
   .AddNewtonsoftJson(options =>
   options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BlogDbContext>(options =>
       options.UseSqlServer(DefaultConnection));
builder.Services.AddScoped<IRedisCache, RedisCache>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommentService, CommentService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
})
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
