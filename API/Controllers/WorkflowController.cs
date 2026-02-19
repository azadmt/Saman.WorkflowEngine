using Microsoft.AspNetCore.Mvc;
using WorkflowBase;

[ApiController]
[Route("api/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowBase.WorkflowEngine _engine;


    public WorkflowController(WorkflowBase.WorkflowEngine engine)
    => _engine = engine;


    [HttpPost("start/{code}")]
    public IActionResult Start(string code, [FromBody] Dictionary<string,object> input)
    {
     _=   _engine.Start(code,1, input);
        return Ok();
    }
}
