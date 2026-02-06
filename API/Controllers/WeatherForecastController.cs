using API.WFBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[ApiController]
[Route("api/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowDbContext _db;
    private readonly WorkflowEngine _engine;


    public WorkflowController(WorkflowDbContext db, WorkflowEngine engine)
    {
        _db = db;
        _engine = engine;
    }


    // 1️⃣ Create Workflow Definition
    [HttpPost("definitions")]
    public async Task<Guid> CreateDefinition([FromBody] WorkflowDefinition def)
    {
        var entity = new WorkflowDefinitionEntity
        {
            Id = Guid.NewGuid(),
            Name = def.Name,
            DefinitionJson = JsonConvert.SerializeObject(def)
        };


        _db.Definitions.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }


    // 2️⃣ Start Workflow Instance
    //[HttpPost("start/{definitionId}")]
    //public async Task<Guid> Start(Guid definitionId, [FromBody] Dictionary<string, object> input)
    //{
    //    var ctx = new WorkflowContext();
    //    foreach (var kv in input)
    //        ctx.SetData(kv.Key, kv.Value);


    //    return await _engine.StartAsync(definitionId, ctx);
    //}



    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly WorkflowDbContext _db;


        public TaskController(WorkflowDbContext db)
        {
            _db = db;
        }


        // 3️⃣ Task List for UI
        [HttpGet("by-role/{role}")]
        public async Task<IEnumerable<WorkflowTask>> GetTasks(string role)
        => await _db.Tasks
        .Where(t => t.Role == role && !t.IsCompleted)
        .ToListAsync();
    }
}
