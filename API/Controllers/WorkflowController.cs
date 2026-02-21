using Microsoft.AspNetCore.Mvc;
using WorkflowBase;
using WrokflowDefinition.HealthInsuranceIssue.DataContract;

[ApiController]
[Route("api/workflows")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowBase.WorkflowEngine _engine;


    public WorkflowController(WorkflowBase.WorkflowEngine engine)
    => _engine = engine;


    [HttpPost("start/{workflowName}/{workflowVersion}")]
    public IActionResult Start(string workflowName,int workflowVersion, [FromBody] Dictionary<string,object> input)
    {
        var poicyRequest = HealthPolicyRequest.GenerateSample(underlyingDiseaseCount: 1);
        var instance = _engine.Start(
              workflowName: "health-underwriting",
              workflowVersion: 1,
              input: new Dictionary<string, object>
              {
                  ["PolicyRequest"] = poicyRequest,
              }
          );
        return Ok(instance.Id);
    }
}
