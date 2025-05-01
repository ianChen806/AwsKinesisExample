namespace KinesisExample.Models;

public class UserEvent
{
    public string UserId { get; set; }

    public string Action { get; set; }

    public DateTime Timestamp { get; set; }

    public string Page { get; set; }
}
