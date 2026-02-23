using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WorkflowBase;
using WorkflowEngine.Panel.MVC.Models;
using WorkflowEngine.Panel.MVC.Service;

namespace WorkflowEngine.Panel.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITaskApiService _api;

        public HomeController(ITaskApiService api)
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
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
