using System;
using System.Collections.Generic;
using System.Linq;

namespace Boggle.GameData
{
    public class AttributeSet
    {
        private Dictionary<string, GameAttribute> _attributes = new Dictionary<string, GameAttribute>();
        public Dictionary<string, GameAttribute> GetAttributes() { return _attributes; }

        private AttributeSet _pointerSet = null;
        public List<KeyValuePair<string, GameAttribute>> ToList() { return _attributes.ToList(); }
        public void SetPointers(AttributeSet p)
        {
            _pointerSet = p;
            foreach (var a in _attributes) a.Value.SetPointers(_pointerSet);
        }

        public AttributeSet() { }

        public AttributeSet(AttributeSet copyFrom)
        {
            foreach (var a in copyFrom._attributes)
            {
                _attributes.Add(a.Key, new GameAttribute(a.Value));
            }
            if (copyFrom._pointerSet != null) SetPointers(new AttributeSet(copyFrom._pointerSet));
            else SetPointers(null);
        }
        public AttributeSet(AttributeSet copyFrom, AttributeSet pointers)
        {
            foreach (var a in copyFrom._attributes)
            {
                _attributes.Add(a.Key, new GameAttribute(a.Value));
            }
            SetPointers(pointers);
        }


        public bool Has(string name)
        {
            if(name.Contains('.') && _attributes.ContainsKey(name.Split('.')[0]) && _attributes[name.Split('.')[0]].AttType == GameAttribute.AttributeType.Object)
            {
                return ((AttributeSet)_attributes[name.Split('.')[0]]).Has(name.Substring(name.IndexOf('.') + 1));
            }
            return _attributes.ContainsKey(name);
        }
        public int Count()
        {
            return _attributes.Count;
        }
        public void Add(string name, string value)
        {
            Add(name, GameAttribute.FromJSON(value, _pointerSet));
        }
        public void Add(string name, GameAttribute value)
        {
            value.SetPointers(_pointerSet);
            if (_attributes.ContainsKey(name)) _attributes[name] = value;
            else _attributes.Add(name, value);
        }
        public void Set(string name, string value) => Add(name, value);
        public void Set(string name, GameAttribute value) => Add(name, value);

        public GameAttribute this[string name] { get => Get(name); set => Add(name, value); }
        private GameAttribute this[params object[] path] { get => Get(path); set => Set(value, path); }
        public GameAttribute Get(string name)
        {
            if (name.Contains('.'))
                return Get(name.Split('.').Select<string, object>(n => { int i; if (int.TryParse(n, out i)) return i; return n; }).ToArray());
            if (_attributes.ContainsKey(name)) return _attributes[name];
            return new GameAttribute();
        }
        public GameAttribute Get(params object[] path)
        {
            GameAttribute target = Get(path[0] as string);
            for (int i = 1; i < path.Length; i++)
            {
                if (path[i] is string) target = target.Get(path[i] as string);
                if (path[i] is int) target = target.Get((int)path[i]);
            }
            return target;
        }
        private void Set(GameAttribute value, params object[] path)
        {
            if (path.Length == 0) { Add(path[0] as string, value); return; }
            GameAttribute target = Get(path[0] as string);
            for (int i = 1; i < path.Length - 1; i++)
            {
                if (path[i] is string) target = target.Get(path[i] as string);
                if (path[i] is int) target = target.Get((int)path[i]);
            }
            if (path[path.Length - 1] is string) target.Set(path[path.Length - 1] as string, value);
            if (path[path.Length - 1] is int) target.Set((int)path[path.Length - 1], value);
        }

        public void Merge(AttributeSet merge)
        {
            foreach (var a in merge._attributes)
            {
                if (_attributes.ContainsKey(a.Key)) this[a.Key].Set(a.Value);
                else _attributes.Add(a.Key, new GameAttribute(a.Value));
            }
        }
        public void Remove(AttributeSet merge)
        {
            foreach (var a in merge._attributes)
            {
                if (_attributes.ContainsKey(a.Key)) _attributes.Remove(a.Key);
            }
        }
        public void Remove(params string[] keys)
        {
            foreach (string s in keys)
            {
                if (_attributes.ContainsKey(s)) _attributes.Remove(s);
            }
        }








        public string ToJSON()
        {
            string ret = "{";
            foreach (string name in _attributes.Keys)
            {
                ret += '"' + name + '"' + ':' + _attributes[name].ToJSON() + ',';
            }
            if (ret[ret.Length - 1] == ',') ret = ret.Substring(0, ret.Length - 1);
            return ret + "}";
        }







        public static AttributeSet FromJSON(string json, AttributeSet pointers = null)
        {
            AttributeSet ret = new AttributeSet();
            if (pointers == null) ret._pointerSet = new AttributeSet();
            else ret._pointerSet = pointers;
            try
            {
                List<KeyValuePair<string, string>> split = SplitJSON(json);

                foreach (var s in split)
                {
                    ret.Add(s.Key, s.Value);
                    if (pointers == null && s.Key.StartsWith("^")) ret._pointerSet.Add(s.Key, ret.Get(s.Key));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return ret;
        }
        private static List<KeyValuePair<string, string>> SplitJSON(string json)
        {
            bool inString = false;
            bool escape = false;
            int objDepth = 0;
            int arrDepth = 0;
            bool onKey = true;
            string key = "";
            string val = "";
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];
                if (onKey)
                {
                    if (char.IsLetterOrDigit(c) || c == '_' || c == '-' || (c == '^' && key.Length == 0))
                    {
                        key += c;
                    }
                    else if (c == ':')
                    {
                        onKey = false;
                    }
                    continue;
                }
                if (inString)
                {
                    if (!escape && c == '\\') escape = true;
                    else if (escape)
                    {

                        escape = false;
                    }
                    else
                    {
                        if (c == '"') inString = false;
                    }
                }
                else
                {
                    if (c == '"') inString = true;
                    else if (c == '{') objDepth++;
                    else if (c == '}') objDepth--;
                    else if (c == '[') arrDepth++;
                    else if (c == ']') arrDepth--;
                }

                if (objDepth <= 0 && arrDepth == 0 && !inString && (c == ',' || objDepth < 0))
                {
                    ret.Add(new KeyValuePair<string, string>(key, val.Trim()));
                    key = "";
                    val = "";
                    onKey = true;
                    if (objDepth < 0) break;
                }
                else if (!inString && char.IsWhiteSpace(c)) { }
                else val += c;

                if (objDepth < 0 || arrDepth < 0) throw new ArgumentException("Error in JSON file " + key + ": " + val);
            }

            return ret;
        }





        public static bool operator ==(AttributeSet a1, AttributeSet a2)
        {
            if (a1 is null && !(a2 is null)) return false;
            if (a2 is null && !(a1 is null)) return false;
            if (a1 is null && a2 is null) return true;
            foreach (var a in a1._attributes)
            {
                if (!a2.Has(a.Key)) return false;
                if (a.Value != a2[a.Key]) return false;
            }
            foreach (var a in a2._attributes)
            {
                if (!a1.Has(a.Key)) return false;
            }
            return true;
        }
        public static bool operator !=(AttributeSet a1, AttributeSet a2)
        {
            return !(a1 == a2);
        }
        public static AttributeSet operator +(AttributeSet a1, AttributeSet a2)
        {
            AttributeSet ret = new AttributeSet(a1);
            ret.Merge(a2);
            return ret;
        }
        public static AttributeSet operator -(AttributeSet a1, AttributeSet a2)
        {
            AttributeSet ret = new AttributeSet(a1);
            ret.Remove(a2);
            return ret;
        }
        public static AttributeSet operator *(AttributeSet a1, AttributeSet a2)
        {
            return a1 + a2;
        }
        public static AttributeSet operator /(AttributeSet a1, AttributeSet a2)
        {
            return a1 - a2;
        }
        public static AttributeSet operator &(AttributeSet a1, AttributeSet a2)
        {
            return a1 + a2;
        }
        public static AttributeSet operator |(AttributeSet a1, AttributeSet a2)
        {
            return (a1 - a2) + (a2 - a1);
        }
        public static bool operator >=(AttributeSet a1, AttributeSet a2)
        {
            if (a1 is null && !(a2 is null)) return false;
            if (a2 is null && !(a1 is null)) return true;
            if (a1 is null && a2 is null) return true;
            foreach (var a in a2._attributes)
            {
                if (!a1.Has(a.Key)) return false;
            }
            return true;
        }
        public static bool operator <=(AttributeSet a1, AttributeSet a2)
        {
            if (a1 is null && !(a2 is null)) return true;
            if (a2 is null && !(a1 is null)) return false;
            if (a1 is null && a2 is null) return true;
            foreach (var a in a1._attributes)
            {
                if (!a2.Has(a.Key)) return false;
            }
            return true;
        }
        public static bool operator >(AttributeSet a1, AttributeSet a2)
        {
            if (a1 is null && !(a2 is null)) return false;
            if (a2 is null && !(a1 is null)) return true;
            if (a1 is null && a2 is null) return false;
            foreach (var a in a2._attributes)
            {
                if (!a1.Has(a.Key)) return false;
            }
            return a1.Count() > a2.Count();
        }
        public static bool operator <(AttributeSet a1, AttributeSet a2)
        {
            if (a1 is null && !(a2 is null)) return true;
            if (a2 is null && !(a1 is null)) return false;
            if (a1 is null && a2 is null) return false;
            foreach (var a in a1._attributes)
            {
                if (!a2.Has(a.Key)) return false;
            }
            return a1.Count() < a2.Count();
        }
    }
}
