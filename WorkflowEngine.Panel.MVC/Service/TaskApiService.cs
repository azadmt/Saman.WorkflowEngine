using WorkflowBase;

namespace WorkflowEngine.Panel.MVC.Service
{
    public interface ITaskApiService
    {
        Task<List<WorkflowTask>> GetTasks(string role);
        Task<WorkflowTask?> GetTask(Guid id);
        Task<bool> CompleteTask(Guid id, object payload);
    }

    public class TaskApiService : ITaskApiService
    {
        private readonly HttpClient _http;

        public TaskApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<WorkflowTask>> GetTasks(string role)
        {
            var result= await _http.GetFromJsonAsync<List<WorkflowTask>>(
                $"api/tasks?role={Uri.EscapeDataString(role)}")
                ?? new List<WorkflowTask>();
            return result;
        }

        public async Task<WorkflowTask?> GetTask(Guid id)
        {
            return await _http.GetFromJsonAsync<WorkflowTask>($"api/tasks/{id}");
        }

        public async Task<bool> CompleteTask(Guid id, object payload)
        {
            var res = await _http.PostAsJsonAsync($"api/tasks/{id}/complete", payload);
            return res.IsSuccessStatusCode;
        }
    }
}
