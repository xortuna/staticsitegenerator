namespace StaticSiteGenerator
{
    public class DictionaryStack
    {
        Stack<Dictionary<string, string>> _stack = new Stack<Dictionary<string, string>>();
        public IEnumerable<KeyValuePair<string, string>> GetCurrentElements()
        {
            HashSet<string> outputted = new HashSet<string>();
            foreach (var element in _stack)
            {
                foreach (var key in element)
                {
                    if (outputted.Contains(key.Key))
                        continue;
                    yield return key;
                    outputted.Add(key.Key);
                }
            }
        }

        public bool ContainsKey(string key, bool multiLevel = true)
        {
            return _stack.Any(r=>r.ContainsKey(key));
        }
        public string Get(string key)
        {
            foreach (var dic in _stack)
            {
                if (dic.ContainsKey(key))
                    return dic[key];
            }
            throw new Exception($"Error reading from dictionay, nothing found: {key}");
        }
        public void Add(string key, string value)
        {
            _stack.Peek().Add(key, value);
        }

        public void Push() {
            _stack.Push(new Dictionary<string, string>());
        }
        public void Pop() {
            _stack.Pop();
        }
        public void Clear()
        {
            _stack.Clear();
        }
    }
}
