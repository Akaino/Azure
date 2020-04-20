###### This is under construction. It works. But you won't really `see` that it actually works...

# Async HTTP Pattern
###### Prerequisites (Not required if fetched from repo)
- Press `F1` and create a new `Function Project`
  - Choose:
    - C#
    - HttpTrigger
    - Subscription
    - Name
    - Function-Level authorisation
- Install the `durable functions` npm package
  - `npm install durable functions`

###### Instructions
Build and run (storage emulator required) or publish directly to azure.
Test with Postman:
- `POST` Request to ...APIPatternExample_HTTPStartV1
- `POST` Request to ...APIPatternExample_HTTPStartV2
- `GET` Request to ...APIPatternExample_Status (including id from previous POST and code from previous-previous POST)