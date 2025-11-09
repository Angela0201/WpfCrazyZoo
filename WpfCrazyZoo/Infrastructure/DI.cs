using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCrazyZoo.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public static class DI
    {
        private static readonly Dictionary<Type, Func<object>> map = new Dictionary<Type, Func<object>>();

        public static void Register<T>(Func<T> factory)
        {
            map[typeof(T)] = () => factory();
        }

        public static T Resolve<T>()
        {
            if (map.TryGetValue(typeof(T), out var f)) return (T)f();
            throw new InvalidOperationException("Type not registered: " + typeof(T).FullName);
        }
    }
}
