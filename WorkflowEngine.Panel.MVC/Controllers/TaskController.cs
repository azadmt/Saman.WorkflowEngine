using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Panel.MVC.Service;

namespace WorkflowEngine.Panel.MVC.Controllers;

public class TasksController : Controller
{
    private readonly ITaskApiService _api;

    public TasksController(ITaskApiService api)
    {
        _api = api;
    }

    // /Tasks
    public async Task<IActionResult> Index()
    {
        var role = User?.Claims?.FirstOrDefault(c => c.Type == "role")?.Value ?? "Doctor";
        var tasks = await _api.GetTasks(role);
        return View(tasks);
    }

    // /Tasks/Details/{id}
    public async Task<IActionResult> Details(Guid id)
    {
        var task = await _api.GetTask(id);
        if (task == null) return NotFound();
        return View(task);
    }
}