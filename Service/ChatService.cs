using COMP1640.Models;

public class ChatService
{
    private readonly Comp1640Context _context;

    public ChatService(Comp1640Context context)
    {
        _context = context;
    }

    public async Task CreateMessageAsync(int ChatId, string SenderId, string Content)
    {
        var Message = new COMP1640.Models.Message
        {
            Content = Content,
            SentAt = DateTime.Now,
            ChatId = ChatId,
            UserId = SenderId
        };
        _context.Messages.Add(Message);
        var result = await _context.SaveChangesAsync();
    }
}