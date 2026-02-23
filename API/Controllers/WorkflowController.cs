using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowBase.WorkflowEngine _engine;


    public WorkflowController(WorkflowBase.WorkflowEngine engine)
    {
        _engine = engine;
      
    }



    [HttpPost("start")]
    public IActionResult Start(WorkflowStartRequest model)
    {
        var cleanInput = model.Input?
        .ToDictionary(
            kvp => kvp.Key,
            kvp => (object)kvp.Value
        ) ?? new Dictionary<string, object>();

        var instance = _engine.Start(
              workflowName: model.WorkflowDefinitionName,
              workflowVersion: model.WorkflowDefinitionVersion,
              input: cleanInput
          );
        return Ok(new { WorkflowInstance = instance.Id });
    }
}
