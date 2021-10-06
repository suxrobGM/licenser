using System;
using System.Threading.Tasks;
using Sms.Licensing.Domain.Services.Abstractions;

namespace Sms.Licensing.Infrastructure.Services
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