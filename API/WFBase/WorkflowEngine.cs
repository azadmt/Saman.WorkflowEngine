using API.Sample;
using Newtonsoft.Json;

namespace API.WFBase
{
    public class WorkflowEngine
    {
        private readonly WorkflowDbContext _db;
        private readonly IServiceProvider _sp;


        public WorkflowEngine(WorkflowDbContext db, IServiceProvider sp)
        {
            _db = db;
            _sp = sp;
        }


        public async Task<Guid> StartAsync(List<IWorkflowStep> steps, WorkflowContext context)
        {
            var instance = new WorkflowInstance
            {
                Id = Guid.NewGuid(),
                WorkflowName = "InsuranceWorkflow",
                Status = WorkflowStatus.Running,
                CurrentStepIndex = 0,
                ContextJson = JsonConvert.SerializeObject(context)
            };


            _db.Workflows.Add(instance);
            await _db.SaveChangesAsync();


            await RunAsync(instance, steps);
            return instance.Id;
        }


        public async Task RunAsync(WorkflowInstance instance, List<IWorkflowStep> steps)
        {
            var context = JsonConvert.DeserializeObject<WorkflowContext>(instance.ContextJson)!;


            while (instance.CurrentStepIndex < steps.Count)
            {
                var step = steps[instance.CurrentStepIndex];


                if (step is HumanStep)
                {
                    instance.Status = WorkflowStatus.WaitingForUser;
                    break;
                }


                await step.ExecuteAsync(context, _sp);
                instance.CurrentStepIndex++;
            }


            instance.ContextJson = JsonConvert.SerializeObject(context);
            await _db.SaveChangesAsync();
        }


        public async Task CompleteHumanStepAsync(Guid workflowId)
        {
            var instance = await _db.Workflows.FindAsync(workflowId);
            var context = JsonConvert.DeserializeObject<WorkflowContext>(instance!.ContextJson)!;


            var step = new UnderwritingStep();
            await step.CompleteAsync(context, _sp);


            instance.Status = WorkflowStatus.Running;
            instance.CurrentStepIndex++;
            instance.ContextJson = JsonConvert.SerializeObject(context);


        }
    }
