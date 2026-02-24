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
    public ActionResult<List<WorkflowTask>> Get([FromQuery] string role)
    {
        var tasks = _taskService.GetAvailableTasks(role, "");
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public ActionResult<WorkflowTask> Get(Guid id)
    {
        var task = _taskService.TaskDetail(id);
        return Ok(task);
    }

    [HttpPost("{id}/complete")]
    public ActionResult<WorkflowTask> Complete(Guid id, Dictionary<string,object> formInput)
    {

        var task=_taskService.TaskDetail(id);
     
         _engine.Resume(task.WorkflowInstanceId, formInput["Doctor_ApprovalStatus"].ToString(), formInput, task);
        return Ok();
    }

    [HttpPost("{id}/Take")]
    public IActionResult Take(Guid id)
    {
        _taskService.AssigneTask(id, "", "");
        return Ok();
    }
}