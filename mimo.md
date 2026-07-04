# Saman.WorkflowEngine

A lightweight, extensible Workflow Engine with integrated Rule Engine for business processes such as insurance underwriting and policy issuance. Fully code-based architecture (no external DSL).

## Tech Stack

- **.NET / C#**
- **Blazor** (WorkflowEngine.Panel)
- **MVC** (WorkflowEngine.Panel.MVC)
- **LiteDB** (persistence via LiteDbProxy)
- **Aspire** (Saman.WorkflowEngine.AppHost)

## Solution Structure

```
Saman.WorkflowEngine.sln
├── WorkflowEngine.Core/                          # Core library
│   ├── WorkflowBase/                             # State machine, transitions, activities, tasks, context
│   │   ├── WorkflowEngine.cs                     # Main engine: start, transition, execute activities
│   │   ├── WorkflowDefinition.cs                 # Static workflow structure (states, transitions, activities)
│   │   ├── WorkflowInstance.cs                   # Runtime instance (current state, status, context)
│   │   ├── StateDefinition.cs                    # State definition (Name, StateType)
│   │   ├── TransitionDefinition.cs               # Transition (From→To, optional condition)
│   │   ├── ActivityDefinition.cs                 # Activity definition
│   │   ├── WorkflowActivity.cs                   # Base class for activities
│   │   ├── WorkflowContext.cs                    # Business data bridge between workflow & rule engine
│   │   ├── HumanTaskDefinition.cs                # User tasks within workflow
│   │   ├── WorkflowTask.cs                       # Task entity
│   │   ├── WorkflowTaskService.cs                # Task lifecycle management
│   │   ├── WorkflowRepository.cs                 # Persistence abstraction
│   │   └── WorkflowHistoryEntry.cs               # Audit trail
│   ├── RuleEnginBase/                            # Rule engine
│   │   ├── IRule.cs                              # Rule interface (Evaluate → RuleResult)
│   │   ├── RuleSet.cs                            # Collection of rules
│   │   ├── RuleSetRegistry.cs                    # Rule registration
│   │   ├── RuleParameters.cs                     # Generic input container
│   │   ├── RuleResult.cs                         # Evaluation result
│   │   ├── RuleSeverity.cs                       # Info, Warning, Error, Critical
│   │   └── RuleBinding.cs                        # Rule-to-context binding
│   ├── Persistence/
│   │   └── LiteDbProxy.cs                        # LiteDB persistence implementation
│   └── Common/
│       ├── Builder/                              # Fluent builders for definitions
│       ├── ExpressionEvaluator.cs                # Dynamic expression evaluation
│       └── IDataContxt.cs                        # Data context interface
│
├── WrokflowDefinition.HealthInsuranceIssue/      # Example workflow: Health Insurance Issue
│   ├── WorkflowFactory.cs                        # Workflow definition factory
│   ├── DataContract/
│   │   └── HealthPolicyRequest.cs                # Data model (Age, HasUnderlyingDisease, etc.)
│   ├── RuleDefinitions/
│   │   ├── AgeLimitRule.cs                       # Validates applicant age
│   │   └── UnderlyingDiseaseRule.cs              # Checks pre-existing conditions
│   └── Activity/
│       ├── EvaluateMedicalRiskActivity.cs        # Executes rules via RuleEngine, updates state
│       ├── DoctorReviewActivity.cs               # Doctor review step
│       ├── ApproveProposalActivity.cs            # Approval step
│       ├── RejectProposalActivity.cs             # Rejection step
│       └── UserCompleteDocActivity.cs            # Document completion step
│
├── WorkflowEngine.Panel/                         # Blazor UI panel
│   └── Components/, Services/
│
├── WorkflowEngine.Panel.MVC/                     # MVC UI panel
│   └── Controllers/, Views/, Models/, Helpers/, Service/
│
├── WorkflowEngine.ConsoleApp/                    # Console test harness
│
├── API/                                          # WorkflowHost REST API
│   └── Controllers/, Common/
│
├── HealthInsurance.Api/                          # Health Insurance specific API
│   └── Controllers/
│
└── Saman.WorkflowEngine.AppHost/                 # .NET Aspire AppHost
```

## Architecture Patterns

- **State Machine Pattern**: Workflows = States + Transitions + Activities
- **Strategy Pattern**: Rules implement `IRule`, evaluated via `RuleEngine`
- **Repository Pattern**: `WorkflowRepository`, `WorkflowTaskRepository`
- **Builder Pattern**: Fluent builders for definitions in `Common/Builder/`

## Core Execution Flow

1. Create `WorkflowDefinition` via `WorkflowFactory`
2. Start `WorkflowInstance` with `WorkflowContext` (business data)
3. Execute entry activities on Start state
4. Evaluate transitions (conditional or unconditional)
5. Move to next state, repeat until End state
6. Persist via repository

## Rule + Workflow Integration Patterns

1. **Rule Inside Activity** (recommended for complex logic): Activity → RuleEngine → Context update → Transition checks context
2. **Rule as Transition Condition** (lightweight): Transition directly evaluates rule result

## Human Tasks

- `HumanTaskDefinition` creates `WorkflowTask` entries
- Managed via `WorkflowTaskService` with statuses: Pending, InProgress, Completed
- Used for approval steps (Underwriter Review, Doctor Review, etc.)

## Key Interfaces

- `IRule` — `Evaluate(RuleParameters) → RuleResult`
- `IWorkflowContex` — Workflow context interface
- `IDataContxt` — Data context interface

## Notes

- Project folder has a typo: `WrokflowDefinition` (should be `WorkflowDefinition`)
- Uses LiteDB for local persistence (WorkFlowHost.db, HealthDb)
- .NET Aspire integration via AppHost (currently only obj/ folder present)
