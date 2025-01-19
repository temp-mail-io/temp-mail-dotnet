# TempMail.Client

The official client for https://temp-mail.io.

![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/SLAVONchick/89c85cdd66cced45519b7928f2740687/raw/temp-mail-dotnet-code-coverage.json)
[![Build & Test](https://github.com/temp-mail-io/temp-mail-dotnet/actions/workflows/pr-tests.yml/badge.svg)](https://github.com/temp-mail-io/temp-mail-dotnet/actions/workflows/pr-tests.yml)
[![NuGet Package](https://img.shields.io/nuget/v/TempMail.Client?style=flat&color=blue)](https://www.nuget.org/packages/TempMail.Client/)
[![NuGet PreRelease](https://img.shields.io/nuget/vpre/TempMail.Client?style=flat&color=orange)](https://www.nuget.org/packages/TempMail.Client)
[![TempMail.Client on fuget.org](https://www.fuget.org/packages/TempMail.Client/badge.svg)](https://www.fuget.org/packages/TempMail.Client)


`TempMail.Client` is an easy-to-use client for https://temp-mail.io based on `HttpClient`.

# Installation

```shell
dotnet add package TempMail.Client
```


# Usage

## Create Client

```csharp
using TempMail.Client;

using var client = TempMailClient.Create(
    TempMailConfigurationBuilder.Create()
        .WithApiKey("<YOUR-API-KEY>")
        .Build());
```

## Send Requests

### Get available domains ([API docs](https://docs.temp-mail.io/docs/list-domains-v-1))

```csharp
using TempMail.Client.Models;
using TempMail.Client.Requests;

var domainsResponse = await client.GetAvailableDomains();

if (domainsResponse is not { IsSuccess: true, Result.Domains: { } domains })
{
    return;
}

foreach (var domain in domains)
{
    Console.WriteLine("Domain '{0}' of type {1}", domain.Name, domain.Type);
}
```

### Create a mailbox ([API docs](https://docs.temp-mail.io/docs/create-email-v-1))

```csharp
var ourDomain = domains.First();
```

Now, we can create a mailbox in three ways:

* by e-mail

```csharp
var response = await client.CreateEmail(
    CreateEmailRequest.ByEmail($"my-email@{ourDomain.Name}"));
```

* by domain
```csharp
var response = await client.CreateEmail(
    CreateEmailRequest.ByDomain(ourDomain.Name));
```

* by domain type
```csharp
var response = await client.CreateEmail(
    CreateEmailRequest.ByDomainType(ourDomain.Type));
```

Whatever way you created the mailbox you can work with it the same way.

```csharp
if (response is not { IsSuccess: true, Result: { } email }) 
{
    return;
}

Console.WriteLine("Created e-mail {0} with TTL: {1}", email.Email, email.Ttl)
```

### Get all messages in the mailbox ([API docs](https://docs.temp-mail.io/docs/get-email-messages-v-1))

```csharp
var messagesResponse = await client.GetAllMessages(
    GetAllMessagesRequest.Create(email.Email));

if (messagesResponse is not { IsSuccess: true, Result.Messages: { } messages })
{
    return;
}

foreach (var msg in messages)
{
    Console.WriteLine("From {0} to {1}: '{2}'", msg.From, msg.To, msg.BodyText);
}
```

### Get specific message ([API docs](https://docs.temp-mail.io/docs/get-message-v-1))

```csharp
// let's imagine we haven't just got all the messages in the mailbox,
// but stored their IDs somewhere in a DB instead ;)
var messagesIds = messages.Select(x => x.Id).ToArray();

foreach (var msgId in messagesIds)
{
    var messageResponse = await client.GetSpecificMessage(
        GetSpecificMessageRequest.Create(msgId));
    
    if (messageResponse is not { IsSuccess: true, Result: { } msg })
    {
        continue;
    }
    
    Console.WriteLine("Got message from {0} to {1}: {2}", msg.From, msg.To, msg.BodyText);
}
```

### Delete specific message ([API docs](https://docs.temp-mail.io/docs/delete-message-v-1))

```csharp
// let's get rid of all the messages in the mailbox

foreach (var id in messagesIds)
{
    var msgDeletionResponse = await client.DeleteSpecificMessage(
        DeleteSpecificMessageRequest.Create(id));
}
```

### Get message source code ([API docs](https://docs.temp-mail.io/docs/get-message-source-code-v-1))

```csharp
foreach (var id in messagesIds)
{
    var sourceResponse = await client.GetMessageSourceCode(
        GetMessageSourceCodeRequest.Create(id));
    
    if (sourceResponse is not { IsSuccess: true, Result: { } source })
    {
        continue;
    }
    
    Console.WriteLine("The source code of the message {0} is:\n{1}", id, source);
}
```

### Download attachment to the message ([API docs](https://docs.temp-mail.io/docs/download-attachment-v-1))

```csharp
// let's download all the attachments of one of the messages we've got

var message = messages.First();

foreach (var attachment in message.Attachments)
{
    var attachmentResponse = await client.GetAttachment(
        GetAttachmentRequest.Create(attachment.Id));
    
    if (attachmentResponse is not { IsSuccess: true, Result: { } attchmentBody })
    {
        continue;
    }
    
    var file = Path.Combine(Path.GetTempPath(), attachment.Name);
    await File.WriteAllBytesAsync(file, attachmentBody);
    Console.WriteLine("Downloaded attachment '{0}' ({1} b) at {2}", attachment.Id, attachment.Size, file);
}
```

### Delete the mailbox ([API docs](https://docs.temp-mail.io/docs/delete-email-v-1))

```csharp
var deletionResponse = await client.DeleteEmail(
    DeleteEmailRequest.Create(email.Email));

if (deletionResponse is not { IsSuccess: true })
{
    return;
}
```

### Get rate limit status ([API docs](https://docs.temp-mail.io/docs/get-rate-limit-v-1))

```csharp
var rateLimitStatusResponse = await client.GetRateLimitStatus();

if (rateLimitStatusResponse is not { IsSuccess: true, Result: { } rateLimitStatus })
{
    return;
}

Console.WriteLine("Limit: {0}; Used: {1}; Remaining: {2}; Reset date-time: {3}",
    rateLimitStatus.Limit,
    rateLimitStatus.Used,
    rateLimitStatus.Remaining,
    rateLimitStatus.ResetDateTime);
```

## Error handling

```csharp
var definitlyErrorResponse = await client.GetSpecificMessage(
    GetSpecificMessageRequest.Create("nonexistent-message-id"));

if (definitlyErrorResponse is { IsSuccess: false, ErrorResult: {} error })
{
    
    var side = error.Error.Type switch
    {
        ErrorType.ApiError => "server",
        ErrorType.RequestError => "client"
    }
    
    // most common case is to just log error:    
    Console.WriteLine("An error occured on {0} side: {1}", side, error.Error.Detail);
    Console.WriteLine("Contact support using the request ID: {0}", error.Meta.RequestId);
    
    // or one can just throw if there is an error:
    definitlyErrorResponse.ThrowIfError();
}
```

# TempMail.Client.AspNetCore

![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/SLAVONchick/89c85cdd66cced45519b7928f2740687/raw/temp-mail-dotnet-code-coverage.json)
[![Build & Test](https://github.com/temp-mail-io/temp-mail-dotnet/actions/workflows/pr-tests.yml/badge.svg)](https://github.com/temp-mail-io/temp-mail-dotnet/actions/workflows/pr-tests.yml)
[![NuGet Package](https://img.shields.io/nuget/v/TempMail.Client.AspNetCore?style=flat&color=blue)](https://www.nuget.org/packages/TempMail.Client.AspNetCore/)
[![NuGet PreRelease](https://img.shields.io/nuget/vpre/TempMail.Client.AspNetCore?style=flat&color=orange)](https://www.nuget.org/packages/TempMail.Client.AspNetCore)
[![TempMail.Client.AspNetCore on fuget.org](https://www.fuget.org/packages/TempMail.Client.AspNetCore/badge.svg)](https://www.fuget.org/packages/TempMail.Client.AspNetCore)

### Add `TempMailClient` to ASP.NET DI container

```csharp
...
// pass your API-key
services.AddTempMailClient("<YOUR-API-KEY>");

// or do NOT pass, but ensure you set it via env variable `TEMP_MAIL_API_KEY`
services.AddTempMailClient();
...
```

### Inject and use the client

```csharp
[Controller]
[Route("/api/v1/[controller]")]
public class EmailController(ITempMailClient tempMailClient) : ControllerBase
{
    [HttpPost("/{email}/messages/search")]
    public async Task<IActionResult> Search([FromRoute] string email, [FromQuery] string query)
    {
        var messagesResponse = await tempMailClient.GetAllMessages(GetAllMessagesRequest.Create(email));
        
        messagesResponse.ThrowIfError();

        if (string.IsNullOrWhiteSpace(query))
        {
            return Ok(messagesResponse.Result!.Messages);
        }

        var queriedMessages = messagesResponse.Result!.Messages
            .Where(m =>
                m.From.Contains(query) ||
                m.To.Contains(query) ||
                m.Subject.Contains(query) ||
                m.Cc.Contains(query) ||
                m.BodyText.Contains(query));
        
        return Ok(queriedMessages);
    }
}
```