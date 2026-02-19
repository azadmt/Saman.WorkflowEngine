using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.ConsoleApp.RuleEngineCore;
using WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow.DataContract;

namespace WorkflowEngine.ConsoleApp.WorkflowDefinitions.HealthInsuranceIssueWorkflow.RuleDefinitions;

public class AgeLimitRule : IRule
{
    public string Name => nameof(AgeLimitRule);

    public string Title => "بررسی حداقل و حداکثر بازه سنی مجاز";

    //move to rule binding for mor dynamic
    public RuleSeverity Severity => RuleSeverity.Blocker;
    private int maxAge;
    private int minAge;
    public async Task<RuleResult> EvaluateAsync(IRuleContext context, RuleParameters parameters)
    {
        var policy = context.GetData<HealthPolicyRequest>("PolicyRequest");
        maxAge = parameters.Get<int>(nameof(maxAge));
        minAge = parameters.Get<int>(nameof(minAge));
        var invalidInsureds = new List<Insured>();
        foreach (var item in policy.Insureds)
        {
            if (IsInValid(item.BirthDate))
                invalidInsureds.Add(item);
        }

        return new RuleResult()
        {
            Passed = !invalidInsureds.Any(),
            Messages = invalidInsureds.Select(x => $"{x.Id} - {x.BirthDate}").ToList(),
            Name = Title
        };
    }


    bool IsInValid(DateTime date)
    {
        return maxAge < GetAge(date) || GetAge(date) < minAge;
    }
    static int GetAge(DateTime birthDate)
    {
        DateTime n = DateTime.Now; // To avoid a race condition around midnight
        int age = n.Year - birthDate.Year;

        if (n.Month < birthDate.Month || (n.Month == birthDate.Month && n.Day < birthDate.Day))
            age--;

        return age;

    }


}



