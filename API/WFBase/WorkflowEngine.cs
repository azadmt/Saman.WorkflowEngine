using API.WFBase;
using API.WFBase.Common;
using API.WFBase.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
public class WorkflowEngine
{
    private readonly WorkflowDbContext _db;
    private readonly HttpClient _http;
    private readonly IEventBus _bus;


    public WorkflowEngine(WorkflowDbContext db, HttpClient http, IEventBus bus)
    {
        _db = db; _http = http; _bus = bus;
    }

    public async Task<Guid> StartAsync(string code, object input)
    {
        var defEntity = await _db.Definitions
        .OrderByDescending(x => x.Version)
        .FirstAsync(x => x.Code == code);


        var instance = new WorkflowInstanceEntity
        {
            Id = Guid.NewGuid(),
            DefinitionId = defEntity.Id,
            CurrentStepIndex = 0,
            Status = WorkflowStatus.Running,
            ContextJson = JsonConvert.SerializeObject(input)
        };


        _db.Instances.Add(instance);
        await _db.SaveChangesAsync();


        await RunAsync(instance.Id);
        return instance.Id;
    }

    public async Task RunAsync(Guid instanceId)
    {
        var instance = await _db.Instances.FindAsync(instanceId);
        var defEntity = await _db.Definitions.FindAsync(instance.DefinitionId);
        var def = JsonConvert.DeserializeObject<WorkflowDefinition>(defEntity.DefinitionJson);
        var ctx = JsonConvert.DeserializeObject<WorkflowContext>(instance.ContextJson);


        while (instance.CurrentStepIndex < def.Steps.Count)
        {
            var step = def.Steps[instance.CurrentStepIndex];


            if (step.Type == WorkflowStepType.Human)
            {
                _db.Tasks.Add(new WorkflowTaskEntity
                {
                    Id = Guid.NewGuid(),
                    WorkflowInstanceId = instance.Id,
                    Role = step.Role,
                    Status = WorkflowTaskStatus.Open
                });


                instance.Status = WorkflowStatus.Waiting;
                await _db.SaveChangesAsync();
                return;
            }


            if (step.Type == WorkflowStepType.System)
                await _http.PostAsJsonAsync(step.ApiEndpoint, ctx.Data);


            if (step.Type == WorkflowStepType.Event)
                _bus.Publish(step.EventName, ctx.Data);


            instance.CurrentStepIndex++;
        }


        instance.Status = WorkflowStatus.Completed;
        instance.ContextJson = JsonConvert.SerializeObject(ctx);
        await _db.SaveChangesAsync();
    }


    public async Task CompleteTask(Guid taskId, object payload)
    {
        var task = await _db.Tasks.FindAsync(taskId);
        task.Status = WorkflowTaskStatus.Completed;


        var instance = await _db.Instances.FindAsync(task.WorkflowInstanceId);
        var ctx = JsonConvert.DeserializeObject<WorkflowContext>(instance.ContextJson);


        foreach (var p in payload.GetType().GetProperties())
            ctx.SetData(p.Name, p.GetValue(payload));


        instance.Status = WorkflowStatus.Running;
        instance.CurrentStepIndex++;
        instance.ContextJson = JsonConvert.SerializeObject(ctx);


        await _db.SaveChangesAsync();
        await RunAsync(instance.Id);
    }
}