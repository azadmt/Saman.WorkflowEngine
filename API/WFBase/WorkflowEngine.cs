// =====================================================
// WORKFLOW ENGINE (FINAL, CLEAN, EXPLAINED)
// =====================================================
// مسئولیت این کلاس:
// - اجرای Workflow Step به Step
// - Pause روی HumanStep
// - Resume بعد از تکمیل Task
// - Persist State در DB
// - اجرای Hook (API / Event)
// =====================================================

using API.WFBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class WorkflowEngine
{
    private readonly WorkflowDbContext _db;
    private readonly IServiceProvider _serviceProvider;

    public WorkflowEngine(
        WorkflowDbContext db,
        IServiceProvider serviceProvider)
    {
        _db = db;
        _serviceProvider = serviceProvider;
    }

    // =====================================================
    // START WORKFLOW FROM DEFINITION
    // =====================================================
    // این متد:
    // 1. Workflow Definition را از DB می‌خواند
    // 2. Stepها را می‌سازد
    // 3. Instance جدید ایجاد می‌کند
    // 4. اجرا را شروع می‌کند
    // =====================================================
    public async Task<Guid> StartAsync(WorkflowDefinition definition, WorkflowContext context)
    {
        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowName = definition.Name,
            Status = WorkflowStatus.Running,
            CurrentStepIndex = 0,
            ContextJson = JsonConvert.SerializeObject(context)
        };

        _db.Workflows.Add(instance);
        await _db.SaveChangesAsync();

        var steps = definition.Steps
            .Select(WorkflowStepFactory.Create)
            .ToList();

        await ExecuteAsync(instance, steps);
        return instance.Id;
    }
 
    // =====================================================
    // COMPLETE HUMAN TASK
    // =====================================================
    // این متد توسط UI صدا زده می‌شود
    // - Task را Complete می‌کند
    // - Context را Update می‌کند
    // - Workflow را Resume می‌کند
    // =====================================================
    public async Task CompleteHumanTaskAsync(Guid taskId, Dictionary<string, object> inputFromUi)
    {
        var task = await _db.Tasks.FindAsync(taskId)
            ?? throw new Exception("Task not found");

        var instance = await _db.Workflows
            .FindAsync(task.WorkflowInstanceId)
            ?? throw new Exception("Workflow not found");

        // Context را برمی‌گردانیم
        var context = JsonConvert
            .DeserializeObject<WorkflowContext>(instance.ContextJson)!;

        // داده‌های فرم UI داخل Context می‌رود
        foreach (var kv in inputFromUi)
            context.SetData(kv.Key, kv.Value);

        task.IsCompleted = true;
        instance.Status = WorkflowStatus.Running;
        instance.CurrentStepIndex++;

        instance.ContextJson = JsonConvert.SerializeObject(context);
        await _db.SaveChangesAsync();

        // Resume Workflow
        var definitionEntity = await _db.Definitions
            .FirstAsync(d => d.Name == instance.WorkflowName);

        var definition = JsonConvert
            .DeserializeObject<WorkflowDefinition>(definitionEntity.DefinitionJson)!;

        var steps = definition.Steps
            .Select(WorkflowStepFactory.Create)
            .ToList();

        await ExecuteAsync(instance, steps);
    }

    // =====================================================
    // CORE EXECUTION LOOP
    // =====================================================
    // این متد قلب Workflow Engine است
    // - Loop می‌زند روی Stepها
    // - اگر HumanStep بود → Pause
    // - اگر SystemStep بود → Execute
    // =====================================================
    private async Task ExecuteAsync(WorkflowInstance instance, List<IWorkflowStep> steps)
    {
        // Context را از DB برمی‌گردانیم
        var context = JsonConvert
            .DeserializeObject<WorkflowContext>(instance.ContextJson)!;

        while (instance.CurrentStepIndex < steps.Count)
        {
            var step = steps[instance.CurrentStepIndex];

            // -----------------------------
            // HUMAN STEP → PAUSE WORKFLOW
            // -----------------------------
            if (step is HumanStep humanStep)
            {
                instance.Status = WorkflowStatus.WaitingForUser;

                _db.Tasks.Add(new WorkflowTask
                {
                    Id = Guid.NewGuid(),
                    WorkflowInstanceId = instance.Id,
                    StepName = humanStep.Name,
                    Role = humanStep.Role,
                    IsCompleted = false
                });

                break;
            }

            // -----------------------------
            // SYSTEM STEP → EXECUTE
            // -----------------------------
            await step.ExecuteAsync(context, _serviceProvider);

            instance.CurrentStepIndex++;
        }

        // Persist Context + Status
        instance.ContextJson = JsonConvert.SerializeObject(context);

        if (instance.CurrentStepIndex >= steps.Count)
            instance.Status = WorkflowStatus.Completed;

        await _db.SaveChangesAsync();
    }

}
