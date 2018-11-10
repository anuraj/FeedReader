namespace FeedReader.Services
{
    public interface IUserService
    {
        string Name { get; }
        string Id { get; }
        bool IsAuthenticated { get; }
    }
}