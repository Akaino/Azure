# API Management

###### Introduction
API Management is an Azure service to basically create a proxy endpoint for your (or others) REST APIs.

###### Instructions
- Create an API Management Resource in Azure.
  - For whatever reason this takes about `30-40 minutes`...
  - Consumption plan is the cheapest option.
- Once the API Management is available create a new `Blank API` from the `APIs` sidebar menu.
  - Choose `Full` settings and fill out the form
    - `Display name:` The visible name for this API
    - `Name:` Your name for this API
    - `Description:` A _meaningful_ description of your API
    - `Web service URL:` The URL of the web service endpoint you want to talk against
      - E.g. http://httpbin.org
    - `URL scheme:` `HTTP` or `HTTPS` or `Both`
    - `API URL suffix:` A URL suffix for _your_ endpoint
      - E.g. `myAPI` or `httpbinProxy` or `httpbin`
    - `Base URL:` This is a combination of your API management URL and the suffix
    - `Tags:` _Meaningful_ tags to orginize your API
    - `Products:` The Product(s) you want your API to be published with
    - `Version this API?` Choose whether or not you want versioning

Once you created your API you can add operations and configure them.

- Click `+ Add operation`
  - `Display name:` A display name for this operation
  - `Name:` A name for this operation
  - `URL:` Operation (`GET`, `POST`, `DELETE` etc.) and the endpoint to talk against
    - This is the endpoint of the previously given `web service URL`
  - `Decription:` A _meaningful_ description of this operation
  - `Tags:` Again, tags.

###### What happens now
You now created an API and mapped your own operation to the `Web service URL` and it's operation. This means you can now call your own API (https://myAPI.azure-api.net/`operationName`) as a proxy.
The requests will be forwarded to the `Web service URL`.