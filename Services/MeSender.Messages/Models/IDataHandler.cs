using Microsoft.AspNetCore.Mvc;

namespace MeSender.Messages.Models;

public interface IDataHandler
{
    Task ReceiveDataAsync(Guid messageId, string text, CancellationToken cancellationToken);

    Task<ActionResult<ICollection<Message>>> SendDataAsync(CancellationToken cancellationToken);
}
