# API

```csharp
APIController aPIController = client.APIController;
```

## Class Name

`APIController`


# Get Jwt

Get JWT access token from AAD to pass in auth header

```csharp
GetJwtAsync(
    string grantType = null,
    string clientId = null,
    string clientSecret = null,
    string scope = null)
```

## Parameters

| Parameter | Type | Tags | Description |
|  --- | --- | --- | --- |
| `grantType` | `string` | Form, Optional | This should always be "client_credentials" |
| `clientId` | `string` | Form, Optional | This is the client/appID listed in your AD app registration |
| `clientSecret` | `string` | Form, Optional | This is the client secret you will create and copy in from your AD app registration |
| `scope` | `string` | Form, Optional | This is the resource URI found in the app manifest of your AD app registration with a "/.default" suffix (ie. api://xxxxxxx/.default) |

## Response Type

`Task<object>`

## Example Usage

```csharp
try
{
    object result = await aPIController.GetJwtAsync(null, null, null, null);
}
catch (ApiException e){};
```

## Errors

| HTTP Status Code | Error Description | Exception Class |
|  --- | --- | --- |
| Default | unexpected error | [`ErrorException`](../../doc/models/error-exception.md) |

