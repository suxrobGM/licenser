using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Licenser.Domain.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            return new ObservableCollection<T>(items);
        }

        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                observableCollection.Add(item);
            }
        }
    }
}