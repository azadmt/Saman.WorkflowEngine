using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace API.WFBase;

public enum WorkflowStatus { Running, Waiting, Completed, Failed }
