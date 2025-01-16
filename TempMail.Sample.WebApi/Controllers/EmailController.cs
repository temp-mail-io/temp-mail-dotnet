using Microsoft.AspNetCore.Mvc;
using TempMail.Client;
using TempMail.Client.Requests;

namespace TempMail.Sample.WebApi.Controllers;

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