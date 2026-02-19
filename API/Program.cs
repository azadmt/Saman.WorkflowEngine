using LiteDB;
using Microsoft.EntityFrameworkCore;
using WorkflowBase;

namespace API
{
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


            //builder.Services.AddDbContext<WorkflowDbContext>(o =>
            //            o.UseSqlite("Data Source=workflow.db"));


            builder.Services.AddHttpClient();
            //builder.Services.AddScoped<IWorkflowHook>(sp =>
            //new ConsoleEventHook());


            builder.Services.AddScoped<WorkflowBase.WorkflowEngine>();
            builder.Services.AddScoped<IWorkflowRepository,LiteDbWorkflowRepository>();
            builder.Services.AddScoped<WorkflowTaskService>();
           var workflowDb= builder.Configuration.GetValue<string>("DbName");
            builder.Services.AddScoped<LiteDatabase>((sp)=> new LiteDatabase(workflowDb));

            
            builder.Services.AddControllers();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
