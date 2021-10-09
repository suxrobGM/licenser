using System;
using System.Threading.Tasks;

namespace Licenser.Domain.Services.Abstractions
{
    public interface INotificationService<TKey, TValue>
    {
        Task Update(TKey key, TValue value);

        event Func<TKey, TValue, Task> Notify;
    }
}