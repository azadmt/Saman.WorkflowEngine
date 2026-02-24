using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using WorkflowBase;
using WrokflowDefinition.HealthInsuranceIssue.DataContract;

namespace HealthInsurance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PolicyController : ControllerBase
{
    private readonly LiteDatabase _dbContext;

    public PolicyController(LiteDatabase dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var policy = _dbContext
            .GetCollection<HealthPolicyRequest>()
            .Query()
            .ToList();
             ;

        return Ok(policy);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var policy = _dbContext.GetCollection<HealthPolicyRequest>()
              .FindById(id);
      
        return Ok(policy);
    }
    [HttpPost("approve")]
    public async Task< IActionResult> Approve(ApprovePolicy model)
    {
      var policy=  _dbContext.GetCollection<HealthPolicyRequest>()
            .FindById(model.PolicyId);
       policy.State = "Approve";
        policy.ExtraRate=model.ExtraRate;   
        _dbContext.GetCollection<HealthPolicyRequest>()
      .Upsert(policy);
        return Ok(policy);
    }

    [HttpPost("reject")]
    public async Task<IActionResult> Reject(RejectPolicy rejectPolicy)
    {
        var policy = _dbContext.GetCollection<HealthPolicyRequest>()
          .FindById(rejectPolicy.PolicyId);
        policy.State = "Reject";
        _dbContext.GetCollection<HealthPolicyRequest>()
        .Upsert(policy);
        return Ok(policy);
    }


    [HttpPost]
    public async Task<IActionResult> CreatPolicy(int underlyingDiseaseCount=0,int withInvlidAgeCount=0 )
    {
        var policyRequest = HealthPolicyRequest.GenerateSample(100000,underlyingDiseaseCount,withInvlidAgeCount);
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5020/");
      
        var request = new WorkflowStartRequest
        {
            WorkflowDefinitionName = "health-underwriting",
            WorkflowDefinitionVersion = 1,
            Input = new Dictionary<string, JsonElement>
            {
                ["PolicyRequest"] = System.Text.Json.JsonSerializer.SerializeToElement(policyRequest)
            }
        };
        _dbContext.GetCollection<HealthPolicyRequest>()
           .Upsert(policyRequest);
        var result = await client
            .PostAsJsonAsync("api/workflows/start", request);
            var workflowId= await result.Content.ReadAsStringAsync();
           ;
        return Ok( new {PolicyId=policyRequest.Id, WorkFlowInstanceId=new Guid(workflowId) });


    }
}
public class WorkflowStartRequest
{
    public string WorkflowDefinitionName { get; set; }
    public int WorkflowDefinitionVersion { get; set; }
    public Dictionary<string, JsonElement> Input { get; set; } = new();
}

public class ApprovePolicy
{
    public Guid PolicyId { get; set; }
    public decimal ExtraRate{ get; set; }
}

public class RejectPolicy
{
    public Guid PolicyId { get; set; }
}
