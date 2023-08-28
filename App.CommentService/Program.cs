using Microsoft.EntityFrameworkCore;
using AspNetCoreRateLimit;
using App.DataAccess.BlogDbContext;
using App.Utility;
using App.Entity.Database;
using App.Entity;
using MediatR;
using App.CommentService.Commands;
using App.CommentService.Handlers;
using App.CommentService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuration setup
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

// Initialize ConnectionHelper
ConnectionHelper.Initialize(configuration);

var DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddControllers();
   
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddMemoryCache();
builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<BlogPost>());
builder.Services.AddTransient<IRequestHandler<GetCommentByPostIdCommand, List<Comment>>, GetCommentByPostIdHandler>();
builder.Services.AddTransient<IRequestHandler<GetCommentByIdCommand, Comment>, GetCommentByIdHandler>();
builder.Services.AddTransient<IRequestHandler<CreateCommentCommand, Comment>, CreateCommentHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateCommentCommand, ApiResponse<string>>, UpdateCommentHandler>();
builder.Services.AddTransient<IRequestHandler<DeleteCommentCommand, ApiResponse<string>>, DeleteCommentHandler>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BlogDbContext>(options =>
       options.UseSqlServer(DefaultConnection));
builder.Services.AddScoped<IRedisCache, RedisCache>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
