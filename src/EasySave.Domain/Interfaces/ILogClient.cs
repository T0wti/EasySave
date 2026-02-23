namespace EasySave.Domain.Interfaces
{
    public interface ILogClient
    {
        Task SendAsync<T>(T entry);
    }
}
