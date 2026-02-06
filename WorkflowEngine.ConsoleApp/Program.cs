using WorkflowEngine.Core;

namespace WorkflowEngine.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var workflow = new Workflow { Name = "E-Shop Workflow with Dynamic Branching" };
            workflow.AddStep(new BrowseItemsStep());
            workflow.AddStep(new AddToCartStep());

            // Add a dynamic conditional branch step
            workflow.AddStep(new ConditionalBranchStep(async context =>
            {
                var paymentMethod = context.GetData<string>("PaymentMethod");

                if (paymentMethod == "Cash")
                {
                    await new CashPaymentStep().ExecuteAsync(context);
                }
                else if (paymentMethod == "CreditCard")
                {
                    await new CreditCardPaymentStep().ExecuteAsync(context);
                    await new AddGiftStep().ExecuteAsync(context);
                }
            }));

            workflow.AddStep(new ConfirmationEmailStep());

            // Set up the context
            var context = new WorkflowContext();
            context.SetData("PaymentMethod", "CreditCard"); // Change to "Cash" to test the other branch

            // Run the workflow
            var engine = new WorkflowEngine.Core.WorkflowEngine();
            await engine.RunWorkflowAsync(workflow, context);
        }
    }
}
