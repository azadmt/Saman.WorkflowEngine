namespace API.WFBase
{
    public class HttpApiHook : IWorkflowHook
    {
        private readonly HttpClient _http;
        private readonly string _url;


        public HttpApiHook(HttpClient http, string url)
        {
            _http = http;
            _url = url;
        }


        public async Task ExecuteAsync(WorkflowContext context)
        {
            await _http.PostAsJsonAsync(_url, context.Data);
        }
    }
}
