using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WorkflowBase;

namespace WorkflowEngine.Core.Common;

public static class ExpressionEvaluator
{
    private static readonly ScriptOptions _options = ScriptOptions.Default
        .AddReferences(
            typeof(WorkflowContext).Assembly,           // اسمبلی پروژه خودت
            typeof(object).Assembly,                     // mscorlib / System.Private.CoreLib
            typeof(System.Linq.Enumerable).Assembly      // System.Linq
                                                         // اگر از نوع‌های بیشتری مثل DateTime، List و ... استفاده می‌کنی، اضافه کن
        )
        .AddImports(
            "System",
            "System.Linq",
            "WorkflowBase"                               // namespace خود WorkflowContext
        );

    private static readonly ConcurrentDictionary<string, Script<bool>> _conditionCache = new();
    private static readonly ConcurrentDictionary<string, Script<string>> _statementCache = new();
    public static async Task<bool> EvaluateConditionAsync(string expression, WorkflowContext context)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return true;  

        try
        {
            if (!_conditionCache.TryGetValue(expression, out var script))
            {
                script = CSharpScript.Create<bool>(expression, globalsType: typeof(WorkflowContext), options: _options);
                _conditionCache[expression] = script;
            }

            var state = await script.RunAsync(context);
            return state.ReturnValue;
            
        }
        catch (CompilationErrorException ex)
        {
            // لاگ کن یا exception مناسب بنداز
            var errors = string.Join("\n", ex.Diagnostics.Select(d => d.ToString()));
            throw new InvalidOperationException($"Compile error {nameof(EvaluateConditionAsync)}:\n{expression}\n\n{errors}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Execution error {nameof(EvaluateConditionAsync)}:\n{expression}", ex);
        }
    }

    public static async Task<string?> EvaluateStatementAsync(string expression, WorkflowContext context)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return null;

        try
        {
            if (!_statementCache.TryGetValue(expression, out var script))
            {
                script = CSharpScript.Create<string>(expression, globalsType: typeof(WorkflowContext), options: _options);
                _statementCache[expression] = script;
            }

            var state = await script.RunAsync(context);
            return state.ReturnValue;
        
        }
        catch (CompilationErrorException ex)
        {
            var errors = string.Join("\n", ex.Diagnostics.Select(d => d.ToString()));
            throw new InvalidOperationException($"Compile error in {nameof(EvaluateStatementAsync)}:\n{expression}\n\n{errors}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Execution error in {nameof(EvaluateStatementAsync)}:\n{expression}", ex);
        }
    }
}