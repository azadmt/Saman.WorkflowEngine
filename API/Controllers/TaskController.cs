using API.WFBase;
using API.WFBase.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly WorkflowEngine _engine;
    private readonly WorkflowDbContext _db;


    public TaskController(WorkflowEngine engine, WorkflowDbContext db)
    {
        _engine = engine; _db = db;
    }


    [HttpGet]
    public async Task<List<WorkflowTaskEntity>> Get([FromQuery] string role)
    => await _db.Tasks.Where(t => t.Role == role && t.Status == WorkflowTaskStatus.Open).ToListAsync();


    [HttpPost("{id}/complete")]
    public async Task Complete(Guid id, [FromBody] object payload)
    => await _engine.CompleteTask(id, payload);
}