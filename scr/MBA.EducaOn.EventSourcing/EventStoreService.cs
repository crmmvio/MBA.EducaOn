using MBA.EducaOn.EventSourcing.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MBA.EducaOn.EventSourcing;

public class EventStoreService : IEventStoreService
{
    //private readonly IEventStoreConnection _connection;

    public EventStoreService(IConfiguration configuration)
    {
        //_connection = EventStoreConnection.Create(
        //    configuration.GetConnectionString("EventStoreConnection"));

        //_connection.ConnectAsync();
    }

    //public IEventStoreConnection GetConnection()
    //{
    //    return _connection;
    //}
}
