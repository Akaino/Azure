# Function Chaining
###### Prerequisites
- Press `F1` and create a new `Function Project`
  - Choose:
    - C#
    - HttpTrigger
    - Subscription
    - Name
    - Function-Level authorisation
- Install the `durable functions` npm package
  - `npm install durable functions`

Now we'll add an orchestrator function by selecting the Azure Functions AddIn, creating a new function and choosing `DurableFunctionsOrchestration`. This is a simple template to orchestrate three functions.

Remove the firstly created function.

###### Instructions
Build and run (storage emulator required) or publish directly to azure.