using Microsoft.EntityFrameworkCore;

namespace API.WFBase.Persistence;

public class WorkflowDbContext : DbContext
{
    public DbSet<WorkflowDefinitionEntity> Definitions => Set<WorkflowDefinitionEntity>();
    public DbSet<WorkflowInstanceEntity> Instances => Set<WorkflowInstanceEntity>();
    public DbSet<WorkflowTaskEntity> Tasks => Set<WorkflowTaskEntity>();


    public WorkflowDbContext(DbContextOptions options) : base(options) { }
}
