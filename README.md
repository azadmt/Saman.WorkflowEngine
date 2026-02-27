# Workflow Engine Documentation

## Overview

This project provides a lightweight, extensible **Workflow Engine** integrated with a **Rule Engine** designed for business processes such as insurance underwriting and policy issuance.

The implementation is divided into three main areas:

1. **WorkflowCore** – Generic workflow infrastructure
2. **RuleEngineCore** – Generic rule evaluation engine
3. **WorkflowDefinitions** – Concrete business workflows (example: Health Insurance Issue)

The architecture is fully code-based (no external DSL) and designed for extensibility, testability, and domain-driven usage.

---

## راهنمای استفاده و ساختار پروژه

این بخش راهنمایی عملی برای توسعه‌دهندگان و مصرف‌کنندگان این کتابخانهٔ Workflow Engine فراهم می‌کند: ساختار پروژه، نحوه تعریف و ثبت ورک‌فلوها، اجرای نمونه و نکات عملیاتی و امنیتی.

### مروری کلی

این مخزن شامل یک هستهٔ موتور گردش‌کار (Workflow Engine) است که به شما اجازه می‌دهد:
- تعریف جریان‌های کاری (workflow definitions) شامل حالت‌های خودکار و وظایف انسانی
- اجرای جریان‌ها به صورت خودکار و ایجاد وظایف انسانی برای UI
- نوشتن شرط‌ها و عبارات داینامیک با استفاده از Roslyn scripting

پروژه‌ها و پوشه‌های مهم:

- `WorkflowEngine.Core`  
  شامل مدل‌های اصلی (`WorkflowDefinition`, `StateDefinition`, `WorkflowInstance`, `WorkflowContext`)، موتور اجرا (`WorkflowEngine`)، و ابزارهایی مانند `ExpressionEvaluator` و `WorkflowBuilder`.

- `WorkflowEngine.Panel` / `WorkflowEngine.Panel.MVC`  
  رابط کاربری برای نمایش و تکمیل وظایف انسانی (مثلاً `Views/Tasks/Details.cshtml`)

- `WrokflowDefinition.*`  
  نمونه‌های تعریف‌شده از ورک‌فلوها (مثل `HealthInsuranceIssue`) که نشان‌دهندهٔ استفاده از Fluent Builder و حالت‌های HumanTask و Automatic است.

- `WorkflowEngine.Core.Persistence`  
  پیاده‌سازی LiteDB برای ذخیرهٔ نمونه‌ها، تاریخچه و وظایف. این لایه قابل جایگزینی است.

### نحوه تعریف و ثبت یک ورک‌فلو

می‌توانید با دو روش ورک‌فلو را تعریف کنید:
1. به صورت شیء `WorkflowDefinition` و پر کردن `States` و `Transitions`.
2. با Fluent Builder: `Workflow.Define(...).State(...).WhenExpression(...).Build()`.

نمونهٔ ثبت در کد:

// ساختن تعاریف
var definitions = new[] { new HealthInsuranceWorkflow().GetFluentDefinition() };
// ثبت در Registry
var engine = new WorkflowEngine(workflowRepo, taskService, dbContext);
engine.RegisterWorkflow(definitions);

### اجرای یک ورک‌فلو (Start)

برای شروع یک نمونهٔ جدید:

var inputs = new Dictionary<string, object> { ["PolicyRequest"] = policyRequest };
var instance = engine.Start("health-underwriting", 1, inputs);

موتور مقادیر ورودی را در `WorkflowContext` ذخیره می‌کند و اجرای خودکار را آغاز می‌نماید تا زمانی که به حالت HumanTask یا End برسد.

### تکمیل وظایف انسانی (Human Tasks)

وقتی که به حالت `HumanTask` می‌رسیم، موتور یک `WorkflowTask` تولید و در پایگاه داده ثبت می‌کند. UI مسئول نمایش `Inputs` و `ContextDisplayFields` است.

برای تکمیل وظیفه، کنترلر باید ورودی‌ها را به `Resume` پاس دهد:

await controller.Resume(instanceId, "APPROVE", new Dictionary<string, object> { ["Doctor_Notes"] = "..." });

در صورتی که رزومه با یک `WorkflowTask` همراه باشد، `WorkflowTaskService` آن را می‌بندد.

### نوشتن شرط‌ها با ExpressionEvaluator

شما می‌توانید از `WhenExpression("GetData<string(\"RiskLevel\") == \"low\"")` استفاده کنید. عبارت‌ها با Roslyn کامپایل و cache می‌شوند تا عملکرد بهتری داشته باشند.

احتیاط امنیتی: عبارات به صورت کامل دسترسی به فضای نام‌هایی که به `ScriptOptions` اضافه شده‌اند دارند، بنابراین از ورود عبارات از منابع غیرقابل اعتماد خودداری کنید.

### نکات مهم و قراردادهای پروژه

- زمان‌ها: از `DateTimeOffset.UtcNow` برای ذخیرهٔ زمان‌ها استفاده کنید تا مشکلات منطقه زمانی حذف شوند.
- Thread-safety: Registryهای داخلی باید thread-safe باشند (ConcurrentDictionary). در حالت تولیدی از قفل یا مکانیزم نسخه‌بندی برای جلوگیری از race condition استفاده کنید.
- خطاها: از `SingleOrDefault()` و بررسی مقدار بازگشتی برای جلوگیری از خطاهای `InvalidOperationException` استفاده کنید و پیام‌های معنی‌دار بدهید.
- امنیت اسکریپت: اگر عبارات از کاربران غیرقابل اعتماد ساخته می‌شود، بهتر است محیط اجرای آنها را محدود یا از فرایند جداگانه استفاده کنید.
- حلقه‌های بی‌نهایت: موتور فعلی بازگشتی است؛ برای جلوگیری از حلقه‌های نامتناهی، محدودیت تعداد انتقال‌ها در هر اجرا (مثلاً 100) یا تشخیص حلقه پیشنهاد می‌شود.

### تست و CI

- برای تست واحد، از repositoryهای جایگزین یا mock برای `IWorkflowRepository` و `LiteDbContext` استفاده کنید.
- سناریوهای کلیدی برای پوشش تست:
  - ارزیابی عبارات (صحیح/خطا)
  - مسیرهای اتوماتیک با چند شرط (low/medium/high)
  - تولید و تکمیل `WorkflowTask` و تغییر وضعیت نمونه
  - حالت‌های خطا (transition ناموجود، رفتار در صورت خطای اجرای activity)

### راهنمای مشارکت (Contributing)

- قبل از ارسال Pull Request، تست‌های واحد مربوطه را اضافه کنید.
- قواعد سبک: فایل `.editorconfig` را رعایت کنید (indentation و نام‌گذاری).
- در صورت تغییر مدل‌های ذخیره‌سازی، Migrations یا راهنمای بروز رسانی DB را به مستندات اضافه کنید.

### مثال سریع (Flow کامل)

1. ثبت ورک‌فلو از `WrokflowDefinition.HealthInsuranceIssue` در startup.
2. `engine.Start("health-underwriting", 1, inputs)` اجرا می‌شود.
3. موتور به حالت `AutoMedicalCheck` می‌رود، `EvaluateMedicalRiskActivity` اجرا می‌شود.
4. اگر ریسک متوسط باشد، موتور وظیفه‌ای برای نقش `Doctor` ایجاد می‌کند.
5. UI وظیفه را نمایش می‌دهد؛ پزشک تصمیم را گرفته و فرم را ارسال می‌کند.
6. Controller با `engine.Resume(...)` ادامهٔ جریان را انجام می‌دهد.

---

# 1. High-Level Architecture

## Core Concepts

### WorkflowDefinition

Defines the static structure of a workflow:

* States
* Transitions
* Activities
* Start state

### WorkflowInstance

Represents a runtime instance of a workflow:

* Current State
* Status
* Context (business data)
* Executed activities

### WorkflowEngine

Responsible for:

* Starting workflow instances
* Executing transitions
* Triggering activities
* Managing state changes

### WorkflowContext

Holds business data during workflow execution.
This is the bridge between workflow and rule engine.

---

# 2. WorkflowCore Structure

## 2.1 StateDefinition

Represents a state in the workflow.

Properties:

* `Name`
* `StateType` (Start, Normal, End)

Example:

* Draft
* RiskEvaluation
* Approved
* Rejected

---

## 2.2 TransitionDefinition

Represents movement between two states.

Properties:

* `FromState`
* `ToState`
* Optional condition (predicate)

Transitions can:

* Be unconditional
* Be condition-based
* Be rule-driven

---

## 2.3 ActivityDefinition

Represents executable logic during a state or transition.

Examples:

* EvaluateMedicalRiskActivity
* SendNotificationActivity

Activities operate on:

* `WorkflowContext`
* `WorkflowInstance`

---

## 2.4 HumanTaskDefinition

Defines user tasks inside workflow.

Examples:

* Underwriter Review
* Medical Officer Approval

Creates `WorkflowTask` entries inside `WorkflowTaskRepository`.

---

## 2.5 WorkflowEngine

Main responsibilities:

* Load workflow definition
* Create workflow instance
* Execute transitions
* Run activities
* Update state
* Persist via repository

Execution Flow:

1. Start instance
2. Enter Start state
3. Execute entry activities
4. Evaluate transitions
5. Move to next state
6. Repeat until End state

---

# 3. RuleEngineCore Structure

## 3.1 IRule

Each rule implements:

* `Evaluate(RuleParameters parameters)`

Returns a result containing:

* IsSuccess
* Severity
* Message

---

## 3.2 RuleEngine

Responsible for:

* Executing multiple rules
* Aggregating results
* Handling severity levels

Severity Levels:

* Info
* Warning
* Error
* Critical

---

## 3.3 RuleParameters

Generic container for rule input data.
Allows rules to stay independent from workflow.

---

# 4. Example: Health Insurance Issue Workflow

Location:

`WorkflowDefinitions/HealthInsuranceIssueWorkflow`

## 4.1 Business Scenario

Issuing a health insurance policy with:

* Age validation
* Underlying disease validation
* Risk evaluation activity

---

## 4.2 Data Contract

`HealthPolicyRequest`

Contains:

* Age
* HasUnderlyingDisease
* Other medical fields

Stored inside `WorkflowContext`.

---

## 4.3 Rules Used

### AgeLimitRule

* Validates applicant age
* Returns Error if outside allowed range

### UnderlyingDiseaseRule

* Checks pre-existing conditions
* Can return Warning or Error depending on configuration

---

## 4.4 EvaluateMedicalRiskActivity

This activity:

1. Extracts `HealthPolicyRequest` from context
2. Creates `RuleParameters`
3. Executes rules via `RuleEngine`
4. Interprets results
5. Updates workflow state accordingly

This demonstrates **Rule Engine + Workflow Engine integration**.

---

# 5. Full Execution Example

## Step 1 – Create Workflow Definition

Via `WorkflowFactory`.

Defines:

States:

* Draft (Start)
* RiskEvaluation
* Approved (End)
* Rejected (End)

Transitions:

* Draft → RiskEvaluation
* RiskEvaluation → Approved (if rules pass)
* RiskEvaluation → Rejected (if rules fail)

---

## Step 2 – Start Workflow

```csharp
var workflow = WorkflowFactory.Create();
var engine = new WorkflowEngine(workflowRepository, taskRepository);

var context = new WorkflowContext();
context.Set("HealthRequest", request);

var instance = engine.Start(workflow, context);
```

---

## Step 3 – Risk Evaluation Activity Executes

Inside activity:

```csharp
var parameters = new RuleParameters();
parameters.Add("Age", request.Age);
parameters.Add("HasUnderlyingDisease", request.HasUnderlyingDisease);

var result = ruleEngine.Execute(parameters);
```

If:

* No errors → transition to Approved
* Error/Critical → transition to Rejected

---

# 6. When Rule Depends on Transition

There are two patterns in this implementation:

## Pattern 1 – Rule Inside Activity (Independent of Transition)

Flow:

State → Activity executes → Updates context → Transition checks context

Rule does NOT know about transition.

Recommended when:

* Complex logic
* Multiple rules
* Reusable validation logic

---

## Pattern 2 – Rule as Transition Condition

Transition directly checks rule result:

```csharp
transition.Condition = (context) => ruleEngine.Execute(...).IsSuccess;
```

Recommended when:

* Simple validation
* Lightweight condition

---

# 7. Repositories

## WorkflowRepository

Responsible for:

* Storing workflow instances
* Retrieving instances

## WorkflowTaskRepository

Responsible for:

* Storing human tasks
* Managing task lifecycle

---

# 8. Extending the Engine

## Add New Workflow

1. Create new folder under `WorkflowDefinitions`
2. Define:

   * Data contract
   * Rules
   * Activities
   * WorkflowFactory

## Add New Rule

1. Implement `IRule`
2. Register in RuleEngine
3. Use inside activity or transition

## Add New Activity

1. Inherit from `WorkflowActivity`
2. Override execution logic
3. Attach to state or transition

---

# 9. Design Principles Used

* Separation of Concerns
* Open/Closed Principle
* Strategy Pattern (Rules)
* State Machine Pattern (Workflow)
* Repository Pattern

---

# 10. Summary

This implementation provides:

* Strong separation between workflow and rules
* Business-oriented extensibility
* Support for human tasks
* Support for complex insurance underwriting flows

It is suitable for:

* Policy issuance
* Claims processing
* Endorsement workflows
* Underwriting processes

---

For further improvements:

* Add persistence layer (EF Core)
* Add event sourcing
* Add audit logging
* Add BPMN exporter
* Add distributed execution support

---

End of Documentation
