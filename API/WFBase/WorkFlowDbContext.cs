using Microsoft.EntityFrameworkCore;

namespace API.WFBase
{
    public class WorkflowDbContext : DbContext
    {
        public DbSet<WorkflowInstance> Workflows => Set<WorkflowInstance>();


        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
        : base(options) { }
    }
}
