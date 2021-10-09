using System;
using System.Threading.Tasks;
using Licenser.Server.Domain.Services.Abstractions;

namespace Licenser.Server.Infrastructure.Services
{
    public class NotificationService<TKey, TValue> : INotificationService<TKey, TValue>
    {
        public async Task Update(TKey key, TValue value)
        {
            if (Notify != null)
            {
                await Notify.Invoke(key, value);
            }
        }

        public event Func<TKey, TValue, Task> Notify;
    }
}