using Microsoft.EntityFrameworkCore;

namespace API.WFBase
{
    public class WorkflowDbContext : DbContext
    {
        public DbSet<WorkflowInstance> Workflows => Set<WorkflowInstance>();

        public DbSet<WorkflowDefinitionEntity> Definitions => Set<WorkflowDefinitionEntity>();
        public DbSet<WorkflowTask> Tasks => Set<WorkflowTask>();
        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
        : base(options) { }
    }
}
