using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowEngine _engine;


    public WorkflowController(WorkflowEngine engine)
    => _engine = engine;


    [HttpPost("start/{code}")]
    public async Task<Guid> Start(string code, [FromBody] object input)
    => await _engine.StartAsync(code, input);
}
