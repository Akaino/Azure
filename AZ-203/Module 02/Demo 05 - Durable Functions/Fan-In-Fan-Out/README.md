# Fan-In Fan-Out Pattern
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
Test with Postman and this payload:

```json
{"Greetings": [
            {
                "$type": "DurableDemos.FanOutInPatternExample+Greeting, DurableDemos",
                "CityName": "New York",
                "Message": "Yo"
            },
            {
                "$type": "DurableDemos.FanOutInPatternExample+Greeting, DurableDemos",
                "CityName": "London",
                "Message": "Good day"
            }
        ]}
```