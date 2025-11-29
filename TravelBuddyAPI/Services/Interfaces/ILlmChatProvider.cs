namespace Services.Interfaces
{
    public interface ILlmChatProvider
    {
        Task<string> AskAsync(string system, string user, CancellationToken cancellationToken = default);
    }
}
