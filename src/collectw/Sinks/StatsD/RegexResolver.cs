using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CollectW.Sinks.StatsD
{
    public class RegexResolver : IResolveCounterType
    {
        private readonly Dictionary<string, StatsDTypes> _cached = new Dictionary<string, StatsDTypes>();
        private readonly Dictionary<Regex, StatsDTypes> _resolvers = new Dictionary<Regex, StatsDTypes>();

        public StatsDTypes Resolve(string counterIdentifier)
        {
            var type = StatsDTypes.Counting;
            if (!_cached.TryGetValue(counterIdentifier, out type))
            {
                foreach (var kvp in _resolvers)
                {
                    if (kvp.Key.Match(counterIdentifier).Success)
                    {
                        _cached.Add(counterIdentifier, kvp.Value);
                        type = kvp.Value;
                        break;
                    }
                }
            }
            return type;
        }

        public RegexResolver Add(string regex, StatsDTypes type)
        {
            _resolvers.Add(new Regex(regex, RegexOptions.IgnoreCase), type);
            return this;
        }
    }
}