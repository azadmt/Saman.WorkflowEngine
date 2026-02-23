using LiteDB;
using WorkflowBase;
using WrokflowDefinition.HealthInsuranceIssue;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient();
        //builder.Services.AddDbContext<WorkflowDbContext>(o =>
        //            o.UseSqlite("Data Source=workflow.db"));
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
            {
                policy
                      .WithOrigins("*") // آدرس فرانت
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });
        //builder.Services.AddScoped<IWorkflowHook>(sp =>
        //new ConsoleEventHook());


        builder.Services.AddSingleton<WorkflowBase.WorkflowEngine>();
        builder.Services.AddSingleton<IWorkflowRepository,LiteDbWorkflowRepository>();
        builder.Services.AddSingleton<WorkflowTaskService>();
       // builder.Services.AddSingleton<IJsonElementCleaner, JsonElementCleaner>();
        var workflowDb= builder.Configuration.GetValue<string>("DbName");
        builder.Services.AddSingleton<LiteDatabase>((sp)=> new LiteDatabase(workflowDb));

        
        builder.Services.AddControllers();
        var app = builder.Build();
        app.UseCors("AllowLocalhost");
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();
       var engin= app.Services.GetService<WorkflowBase.WorkflowEngine>();
        engin.RegisterWorkflow( GetAllWorkflowDefinitions());
        app.Run();
    }

    private static IEnumerable<WorkflowDefinition> GetAllWorkflowDefinitions()
    {
        var workflowDefiniotionType = typeof(IWorkflowDefinitionFactory);
        var workflowDefinitions = typeof(HealthInsuranceWorkflow).Assembly
    .GetTypes()
    .Where(type => workflowDefiniotionType.IsAssignableFrom(type) && !type.IsInterface);

        foreach (var definition in workflowDefinitions)
        {
            var instance = (IWorkflowDefinitionFactory)Activator.CreateInstance(definition);

            yield return instance.GetDefinition();
        }
    }
}

  

