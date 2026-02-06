using WorkflowEngine.Core;

public class BrowseItemsStep : IWorkflowStep
{
    public string Name => "BrowseItemsStep";

    public Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("Customer is browsing items...");
        context.SetData("Cart", new List<string> { "Laptop", "Headphones" });
        Console.WriteLine("Items added to cart: Laptop, Headphones");
        return Task.CompletedTask;
    }
}

// Step 2: Add Items to Cart
public class AddToCartStep : IWorkflowStep
{
    public string Name => "AddToCartStep";

    public Task ExecuteAsync(WorkflowContext context)
    {
        var cart = context.GetData<List<string>>("Cart");
        cart.Add("Mouse");
        Console.WriteLine("Added item to cart: Mouse");
        return Task.CompletedTask;
    }
}

// Step 3: Dynamic Conditional Branch Step
public class ConditionalBranchStep : IWorkflowStep
{
    public string Name => "ConditionalBranchStep";

    private readonly Func<WorkflowContext, Task> _branchLogic;

    public ConditionalBranchStep(Func<WorkflowContext, Task> branchLogic)
    {
        _branchLogic = branchLogic;
    }

    public async Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("Executing conditional branch logic...");
        await _branchLogic(context);
    }
}

// Step 4: Payment Steps
public class CashPaymentStep : IWorkflowStep
{
    public string Name => "CashPaymentStep";

    public Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("Processing cash payment...");
        context.SetData("PaymentStatus", "Success");
        Console.WriteLine("Cash payment successful!");
        return Task.CompletedTask;
    }
}

public class CreditCardPaymentStep : IWorkflowStep
{
    public string Name => "CreditCardPaymentStep";

    public Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("Processing credit card payment...");
        context.SetData("PaymentStatus", "Success");
        Console.WriteLine("Credit card payment successful!");
        return Task.CompletedTask;
    }
}

public class AddGiftStep : IWorkflowStep
{
    public string Name => "AddGiftStep";

    public Task ExecuteAsync(WorkflowContext context)
    {
        Console.WriteLine("Adding a free gift for credit card payment...");
        var cart = context.GetData<List<string>>("Cart");
        cart.Add("Free Gift");
        Console.WriteLine("Free gift added to the cart!");
        return Task.CompletedTask;
    }
}

// Step 5: Send Confirmation Email
public class ConfirmationEmailStep : IWorkflowStep
{
    public string Name => "ConfirmationEmailStep";

    public Task ExecuteAsync(WorkflowContext context)
    {
        var paymentStatus = context.GetData<string>("PaymentStatus");

        if (paymentStatus == "Success")
        {
            Console.WriteLine("Sending confirmation email to the customer...");
        }
        else
        {
            Console.WriteLine("Payment failed. Cannot send confirmation email.");
        }

        return Task.CompletedTask;
    }
}
