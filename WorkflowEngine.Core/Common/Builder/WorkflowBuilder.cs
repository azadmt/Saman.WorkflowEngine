using System;
using System.Collections.Generic;
using WorkflowBase; // شامل StateDefinition, StateType, HumanTaskDefinition, TransitionDefinition, TaskInput, InputType, DisplayField, GridColumn, WorkflowContext و ...

namespace WorkflowEngine.Core.Common.Builder
{
    public static class Workflow
    {
        public static WorkflowBuilder Define(string name, int version = 1)
            => new WorkflowBuilder(name, version);
    }

    public class WorkflowBuilder
    {
        private readonly WorkflowDefinition _def;

        internal WorkflowBuilder(string name, int version)
        {
            _def = new WorkflowDefinition
            {
                Name = name,
                Version = version,
                States = new Dictionary<string, StateDefinition>(),
                StartState = null!
            };
        }

        public WorkflowBuilder StartsAt(string stateId)
        {
            _def.StartState = stateId;
            return this;
        }

        public WorkflowBuilder WithRuleSet(string ruleSetId)
        {
            _def.RuleSetId = ruleSetId;
            return this;
        }

        public StateBuilder State(string id, StateType stateType, string title)
        {
            var state = new StateDefinition();
            state.Type = stateType;
            state.Title = title;
            _def.States[id] = state;
            return new StateBuilder(this, state);
        }

        public WorkflowDefinition Build()
        {
            if (string.IsNullOrEmpty(_def.StartState))
                throw new InvalidOperationException("Start state is required");

            if (!_def.States.Any(x => x.Value.Type == StateType.End))
                throw new InvalidOperationException("final state is required");
            return _def;
        }
    }

    public class StateBuilder
    {
        protected readonly WorkflowBuilder Parent;
        protected readonly StateDefinition State;

        internal StateBuilder(WorkflowBuilder parent, StateDefinition state)
        {
            Parent = parent;
            State = state;
        }

        internal void AddTransition(TransitionDefinition transition)
        {
            State.Transitions.Add(transition);
        }

        public StateBuilder WithDecisionField(
                string label = "وضعیت تصمیم",
                Action<TaskInput>? configureOptions = null)
        {
            EnsureHumanTask();

            var decision = new TaskInput
            {
                Name = "Decision",              // نام ثابت و جنریک
                Lable = label,
                Type = InputType.Dropdown,      // یا Radio اگر ترجیح می‌دهید
                Options = new List<KeyValuePair<string, string>>()
            };



            configureOptions?.Invoke(decision);

            // همیشه آخرین ورودی تصمیم باشد یا جایگزین قبلی شود
            var existing = State.HumanTask!.Inputs.FirstOrDefault(i => i.Name == "Decision");
            if (existing != null)
                State.HumanTask!.Inputs.Remove(existing);

            State.HumanTask!.Inputs.Add(decision);

            return this;
        }

        //public StateBuilder Title(string title)
        //{
        //    State.Title = title;
        //    return this;
        //}

        public StateBuilder WithRuleset(string ruleSetId)
        {
            State.RulesetId = ruleSetId;
            return this;
        }
        //public StateBuilder Automatic()
        //{
        //    State.Type = StateType.Automatic;
        //    return this;
        //}

        public StateBuilder HumanTask(string role, string uiContract)
        {
            State.Type = StateType.HumanTask;
            State.HumanTask = new HumanTaskDefinition
            {
                Role = role,
                UiContract = uiContract,
                Inputs = new List<TaskInput>(),
                ContextDisplayFields = new List<DisplayField>()
            };
            return this;
        }

        //public StateBuilder IsStart()
        //{
        //    State.Type = StateType.Start;
        //    return this;
        //}

        //public StateBuilder End()
        //{
        //    State.Type = StateType.End;
        //    return this;
        //}

        public StateBuilder Activity<TActivity>() where TActivity : IWorkflowActivity, new()
        {
            State.Activities.Add(new TActivity());
            return this;
        }

        // شرطی برای Automatic states
        public TransitionBuilder When(Func<WorkflowContext, bool> condition)
            => new TransitionBuilder(this, condition, useExpression: false);

        public TransitionBuilder WhenExpression(string expression)
            => new TransitionBuilder(this, null, useExpression: true) { ConditionExpression = expression };

        // برای Human Task events
        public HumanTaskTransitionBuilder On(string eventName)
            => new HumanTaskTransitionBuilder(this, eventName);

        // ────────────── Inputهای ساده ──────────────
        public StateBuilder InputNumber(string name, string label)
        {
            EnsureHumanTask();
            State.HumanTask!.Inputs.Add(new TaskInput
            {
                Name = name,
                Lable = label,
                Type = InputType.Number
            });
            return this;
        }

        public StateBuilder InputTextarea(string name, string label)
        {
            EnsureHumanTask();
            State.HumanTask!.Inputs.Add(new TaskInput
            {
                Name = name,
                Lable = label,
                Type = InputType.Textarea
            });
            return this;
        }

        // ────────────── MultiSelect ──────────────
        public StateBuilder InputMultiSelect(
            string name,
            string label,
            Action<TaskInput>? configure = null)
        {
            EnsureHumanTask();

            var input = new TaskInput
            {
                Name = name,
                Lable = label,
                Type = InputType.MultiSelect,
                Options = new List<KeyValuePair<string, string>>()
            };

            configure?.Invoke(input);
            State.HumanTask!.Inputs.Add(input);
            return this;
        }

        public StateBuilder InputMultiSelectFromContext(
            string name,
            string label,
            Func<IWorkflowContext, List<KeyValuePair<string, string>>>? optionsProvider = null)
        {
            EnsureHumanTask();

            var input = new TaskInput
            {
                Name = name,
                Lable = label,
                Type = InputType.MultiSelect,
                OptionsDataProvider = optionsProvider
            };

            State.HumanTask!.Inputs.Add(input);
            return this;
        }

        // ────────────── Dropdown (با Action) ──────────────
        public StateBuilder InputDropdown(
            string name,
            string label,
            Action<TaskInput> configure)
        {
            EnsureHumanTask();

            var input = new TaskInput
            {
                Name = name,
                Lable = label,
                Type = InputType.Dropdown,
                Options = new List<KeyValuePair<string, string>>()
            };

            configure(input);
            State.HumanTask!.Inputs.Add(input);
            return this;
        }

        // ────────────── Display fields ──────────────
        public StateBuilder Display(string label, Func<WorkflowContext, object?> valueProvider, int order = 0)
        {
            EnsureHumanTask();
            State.HumanTask!.ContextDisplayFields.Add(new DisplayField
            {
                Label = label,
                Order = order,
                ValueProvider = valueProvider
            });
            return this;
        }

        public GridDisplayBuilder DisplayGrid(string label, Func<WorkflowContext, object?> dataProvider, int order = 0)
        {
            EnsureHumanTask();

            var field = new DisplayField
            {
                Label = label,
                Order = order,
                GridColumns = new List<GridColumn>(),
                ValueProvider = dataProvider
            };

            State.HumanTask!.ContextDisplayFields.Add(field);
            return new GridDisplayBuilder(this, field);
        }

        public WorkflowBuilder Done() => Parent;

        private void EnsureHumanTask()
        {
            if (State.HumanTask == null)
                throw new InvalidOperationException("This method is only available for HumanTask states.");
        }
    }

    public class TransitionBuilder
    {
        private readonly StateBuilder _parent;
        private readonly Func<WorkflowContext, bool>? _condition;
        private readonly bool _useExpression;
        public string? ConditionExpression { get; set; }

        internal TransitionBuilder(StateBuilder parent, Func<WorkflowContext, bool>? condition, bool useExpression)
        {
            _parent = parent;
            _condition = condition;
            _useExpression = useExpression;
        }

        public StateBuilder GoTo(string targetState)
        {
            var trans = new TransitionDefinition
            {
                To = targetState,
                ConditionFunc = _condition,
                ConditionExpression = _useExpression ? ConditionExpression : null
            };
            _parent.AddTransition(trans);
            return _parent;
        }
    }

    public class HumanTaskTransitionBuilder
    {
        private readonly StateBuilder _parent;
        private readonly string _event;

        internal HumanTaskTransitionBuilder(StateBuilder parent, string eventName)
        {
            _parent = parent;
            _event = eventName;
        }

        public StateBuilder GoTo(string targetState, string? title = null)
        {
            var trans = new TransitionDefinition
            {
                Event = _event,
                To = targetState,
                Title = title
            };
            _parent.AddTransition(trans);
            return _parent;
        }
    }

    public class GridDisplayBuilder
    {
        private readonly StateBuilder _parent;
        private readonly DisplayField _field;

        internal GridDisplayBuilder(StateBuilder parent, DisplayField field)
        {
            _parent = parent;
            _field = field;
        }

        public GridDisplayBuilder Column(string header, string field, string? format = null)
        {
            _field.GridColumns.Add(new GridColumn { Header = header, Field = field, Format = format });
            return this;
        }

        public StateBuilder Done() => _parent;
    }
}