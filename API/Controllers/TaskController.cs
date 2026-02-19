using Microsoft.AspNetCore.Mvc;
using WorkflowBase;


[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly WorkflowBase.WorkflowEngine _engine;
    private readonly WorkflowTaskService _taskService;


    public TaskController(WorkflowBase.WorkflowEngine engine, WorkflowTaskService taskService)
    {
        _engine = engine;
        _taskService = taskService;
    }


    [HttpGet]
    public List<WorkflowTask> Get([FromQuery] string role)
    => _taskService.GetAvailableTasks(role, "");


    [HttpPost("{id}/Take")]
    public IActionResult Take(Guid id)
    {
        _taskService.AssigneTask(id, "", "");
        return Ok();
    }
}