using System.Collections.Generic;

namespace MetaphysicsIndustries.Giza
{
    public class ImportCache
    {
        public bool ContainsKey(string name)
        {
            return _cache.ContainsKey(name);
        }

        public Dictionary<string, Definition> this[string key]
        {
            get => _cache[key];
            set => _cache[key] = value;
        }

        private Dictionary<string, Dictionary<string, Definition>>
            _cache =
                new Dictionary<string,
                    Dictionary<string, Definition>>();
    }
}
