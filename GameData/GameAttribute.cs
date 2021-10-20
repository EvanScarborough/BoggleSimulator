using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Boggle.GameData
{
    public class GameAttribute : IEnumerator, IEnumerable
    {
        public enum AttributeType { Boolean, Number, String, Array, Object };
        public AttributeType AttType { get; private set; }

        private AttributeSet _pointerSet = null;
        public bool HasPointers() { return _pointerSet != null; }
        public void SetPointers(AttributeSet p)
        {
            _pointerSet = p;
            if (AttType == AttributeType.Object) Obj.SetPointers(_pointerSet);
            if (AttType == AttributeType.Array) foreach (GameAttribute a in Arr) a.SetPointers(_pointerSet);
        }
        protected AttributeSet GetPointers() { return _pointerSet; }


        private bool Bool { get; set; } = false;
        private double Num { get; set; } = 0;
        private string Str { get; set; } = "";

        private List<GameAttribute> _arr = null;
        private List<GameAttribute> Arr
        {
            get { if (_arr == null) _arr = new List<GameAttribute>(); return _arr; }
            set { _arr = value; }
        }

        private AttributeSet _obj = null;
        private AttributeSet Obj
        {
            get { if (_obj == null) _obj = new AttributeSet(); return _obj; }
            set { _obj = value; }
        }

        public GameAttribute() { }
        public GameAttribute(GameAttribute copyFrom)
        {
            AttType = copyFrom.AttType;
            if (AttType == AttributeType.Boolean) Bool = copyFrom.Bool;
            if (AttType == AttributeType.Number) Num = copyFrom.Num;
            if (AttType == AttributeType.String) Str = copyFrom.Str;
            if (AttType == AttributeType.Object) Obj = new AttributeSet(copyFrom.Obj);
            if (AttType == AttributeType.Array)
            {
                Arr = new List<GameAttribute>();
                foreach (GameAttribute a in copyFrom.Arr)
                {
                    Arr.Add(new GameAttribute(a));
                }
            }
            if (copyFrom._pointerSet != null) SetPointers(new AttributeSet(copyFrom._pointerSet));
            else SetPointers(null);
        }
        public GameAttribute(bool v, AttributeSet pointers = null) { Bool = v; AttType = AttributeType.Boolean; SetPointers(pointers); }
        public GameAttribute(double v, AttributeSet pointers = null) { Num = v; AttType = AttributeType.Number; SetPointers(pointers); }
        public GameAttribute(float v, AttributeSet pointers = null) { Num = v; AttType = AttributeType.Number; SetPointers(pointers); }
        public GameAttribute(int v, AttributeSet pointers = null) { Num = v; AttType = AttributeType.Number; SetPointers(pointers); }
        public GameAttribute(string v, AttributeSet pointers = null) { Str = v; AttType = AttributeType.String; SetPointers(pointers); }
        public GameAttribute(List<GameAttribute> v, AttributeSet pointers = null) { Arr = v; AttType = AttributeType.Array; SetPointers(pointers); }
        public GameAttribute(AttributeSet v, AttributeSet pointers = null) { Obj = v; AttType = AttributeType.Object; SetPointers(pointers); }


        public void Set(bool v) { Bool = v; AttType = AttributeType.Boolean; }
        public void Set(double v) { Num = v; AttType = AttributeType.Number; }
        public void Set(float v) { Num = v; AttType = AttributeType.Number; }
        public void Set(int v) { Num = v; AttType = AttributeType.Number; }
        public void Set(string v) { Str = v; AttType = AttributeType.String; }
        public void Set(List<GameAttribute> v) { Arr = v; AttType = AttributeType.Array; }
        public void Set(AttributeSet v) { Obj = v; AttType = AttributeType.Object; }
        public void Set(GameAttribute v)
        {
            AttType = v.AttType;
            if (AttType == AttributeType.Boolean) Bool = v.Bool;
            if (AttType == AttributeType.Number) Num = v.Num;
            if (AttType == AttributeType.String) Str = v.Str;
            if (AttType == AttributeType.Object) Obj = new AttributeSet(v.Obj, _pointerSet);
            if (AttType == AttributeType.Array)
            {
                Arr.Clear();
                foreach (GameAttribute a in v.Arr) { Arr.Add(new GameAttribute(a)); }
            }
        }



        public GameAttribute this[string name] { get => Get(name); set => Set(name, value); }
        public GameAttribute this[int index] { get => Get(index); set => Set(index, value); }
        public GameAttribute Get(string name)
        {
            if (AttType != AttributeType.Object || Obj == null) return new GameAttribute();
            return Obj.Get(name);
        }
        public void Set(string name, GameAttribute value)
        {
            if (AttType != AttributeType.Object || Obj == null) return;
            Obj.Set(name, value);
        }
        public GameAttribute Get(int index)
        {
            if (AttType != AttributeType.Array || Arr == null || index < 0 || index >= Arr.Count) return new GameAttribute();
            return Arr[index];
        }
        public void Set(int index, GameAttribute value)
        {
            if (AttType != AttributeType.Array || Arr == null || index < 0 || index >= Arr.Count) return;
            Arr[index] = value;
        }


        public override string ToString()
        {
            if (AttType == AttributeType.Boolean) return Bool.ToString();
            if (AttType == AttributeType.Number) return Num.ToString();
            if (AttType == AttributeType.String) return Str;
            if (AttType == AttributeType.Object || AttType == AttributeType.Array) return ToJSON();
            return "";
        }








        public string ToJSON()
        {
            if (AttType == AttributeType.Boolean) return Bool ? "true" : "false";
            if (AttType == AttributeType.Number) return Num.ToString();
            if (AttType == AttributeType.String) return ToJSONString(Str);
            if (AttType == AttributeType.Object) return Obj.ToJSON();
            if (AttType == AttributeType.Array)
            {
                string ret = "[";
                foreach (GameAttribute a in Arr)
                {
                    ret += a.ToJSON() + ",";
                }
                if (ret[ret.Length - 1] == ',') ret = ret.Substring(0, ret.Length - 1);
                return ret + "]";
            }
            return "null";
        }




        public static GameAttribute FromJSON(string json, AttributeSet pointers = null)
        {
            GameAttribute ret = null;
            if (json[0] == '{') ret = new GameAttribute(AttributeSet.FromJSON(json, pointers));
            else if (json[0] == '[') ret = FromJSONArray(json);
            else if (json[0] == '"') ret = new GameAttribute(CleanJSONString(json));
            else if (json.ToLower() == "true" || json.ToLower() == "false") ret = new GameAttribute(json.ToLower() == "true");
            else ret = new GameAttribute(double.Parse(json));
            ret.SetPointers(pointers);
            return ret;
        }
        public static string CleanJSONString(string json)
        {
            string ret = "";
            bool inString = false;
            bool escape = false;
            foreach (char c in json)
            {
                if (!inString)
                {
                    if (c == '"') inString = true;
                }
                else
                {
                    if (!escape && c == '\\') { escape = true; continue; }
                    if (!escape && c == '"') return ret;
                    if (escape && (c == 'n' || c == 'r')) ret += '\n';
                    else if (escape && c == 't') ret += '\t';
                    else ret += c;
                    escape = false;
                }
            }
            return ret;
        }
        public static string ToJSONString(string str)
        {
            return '"' + str.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t") + '"';
        }
        private static GameAttribute FromJSONArray(string json)
        {
            List<string> array = SplitJSONArray(json);
            return new GameAttribute(array.Select(j => FromJSON(j)).ToList());
        }

        public static List<string> SplitJSONArray(string json)
        {
            json = json.Substring(1);
            bool inString = false;
            bool escape = false;
            int objDepth = 0;
            int arrDepth = 0;
            string val = "";
            List<string> ret = new List<string>();

            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];
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

                if (objDepth == 0 && arrDepth <= 0 && !inString && (c == ',' || arrDepth < 0))
                {
                    val = val.Trim();
                    if (val.Length > 0) ret.Add(val);
                    val = "";
                    if (arrDepth < 0) break;
                }
                else if (!inString && char.IsWhiteSpace(c)) { }
                else val += c;

                if (objDepth < 0 || arrDepth < 0) throw new ArgumentException("Error in JSON file " + val);
            }

            return ret;
        }














        public static bool operator ==(GameAttribute a1, GameAttribute a2)
        {
            if (a1 is null && !(a2 is null)) return false;
            if (a2 is null && !(a1 is null)) return false;
            if (a1 is null && a2 is null) return true;
            if (a1.AttType != a2.AttType) return false;
            if (a1.AttType == AttributeType.Number) return a1.Num == a2.Num;
            if (a1.AttType == AttributeType.String) return a1.Str == a2.Str;
            if (a1.AttType == AttributeType.Boolean) return a1.Bool == a2.Bool;
            if (a1.AttType == AttributeType.Object) return a1.Obj == a2.Obj;
            if (a1.AttType == AttributeType.Array)
            {
                if (a1.Arr.Count != a2.Arr.Count) return false;
                for (int i = 0; i < a1.Arr.Count; i++)
                {
                    if (a1[i] != a2[i]) return false;
                }
                return true;
            }
            return false;
        }
        public static bool operator !=(GameAttribute a1, GameAttribute a2)
        {
            return !(a1 == a2);
        }
        public static bool operator >(GameAttribute a1, GameAttribute a2)
        {
            if (a1 is null && !(a2 is null)) return false;
            if (a2 is null && !(a1 is null)) return true;
            if (a1 is null && a2 is null) return false;
            if (a1.AttType == AttributeType.Number) return a1.Num > a2.Num;
            if (a1.AttType == AttributeType.String) return string.Compare(a1.Str, a2.Str) > 0;
            if (a1.AttType == AttributeType.Boolean) return a1.Bool && !a2.Bool;
            if (a1.AttType == AttributeType.Object) return a1.Obj > a2.Obj;
            if (a1.AttType == AttributeType.Array) return a1.Arr.Count > a2.Arr.Count;
            return false;
        }
        public static bool operator <(GameAttribute a1, GameAttribute a2)
        {
            if (a1 is null && !(a2 is null)) return true;
            if (a2 is null && !(a1 is null)) return false;
            if (a1 is null && a2 is null) return false;
            if (a1.AttType == AttributeType.Number) return a1.Num < a2.Num;
            if (a1.AttType == AttributeType.String) return string.Compare(a1.Str, a2.Str) < 0;
            if (a1.AttType == AttributeType.Boolean) return !a1.Bool && a2.Bool;
            if (a1.AttType == AttributeType.Object) return a1.Obj < a2.Obj;
            if (a1.AttType == AttributeType.Array) return a1.Arr.Count < a2.Arr.Count;
            return false;
        }
        public static bool operator >=(GameAttribute a1, GameAttribute a2)
        {
            if (a1 is null && !(a2 is null)) return false;
            if (a2 is null && !(a1 is null)) return true;
            if (a1 is null && a2 is null) return true;
            if (a1.AttType == AttributeType.Number) return a1.Num >= a2.Num;
            if (a1.AttType == AttributeType.String) return string.Compare(a1.Str, a2.Str) >= 0;
            if (a1.AttType == AttributeType.Boolean) return (a1.Bool && !a2.Bool) || (a1.Bool == a2.Bool);
            if (a1.AttType == AttributeType.Object) return a1.Obj >= a2.Obj;
            if (a1.AttType == AttributeType.Array) return a1.Arr.Count >= a2.Arr.Count;
            return false;
        }
        public static bool operator <=(GameAttribute a1, GameAttribute a2)
        {
            if (a1 is null && !(a2 is null)) return true;
            if (a2 is null && !(a1 is null)) return false;
            if (a1 is null && a2 is null) return true;
            if (a1.AttType == AttributeType.Number) return a1.Num <= a2.Num;
            if (a1.AttType == AttributeType.String) return string.Compare(a1.Str, a2.Str) <= 0;
            if (a1.AttType == AttributeType.Boolean) return (!a1.Bool && a2.Bool) || (a1.Bool == a2.Bool);
            if (a1.AttType == AttributeType.Object) return a1.Obj <= a2.Obj;
            if (a1.AttType == AttributeType.Array) return a1.Arr.Count <= a2.Arr.Count;
            return false;
        }
        public static GameAttribute operator +(GameAttribute a1, GameAttribute a2)
        {
            if (a1.AttType == AttributeType.Number) return new GameAttribute(a1.Num + a2.Num);
            if (a1.AttType == AttributeType.String) return new GameAttribute(a1.Str + a2.ToString());
            if (a1.AttType == AttributeType.Boolean) return new GameAttribute(a1.Bool && a2.Bool);
            if (a1.AttType == AttributeType.Object) return new GameAttribute(a1.Obj + a2.Obj);
            if (a1.AttType == AttributeType.Array)
            {
                List<GameAttribute> ret = new List<GameAttribute>();
                foreach (GameAttribute a in a1.Arr) ret.Add(new GameAttribute(a));
                foreach (GameAttribute a in a2.Arr) ret.Add(new GameAttribute(a));
                return new GameAttribute(ret);
            }
            return new GameAttribute();
        }
        public static GameAttribute operator -(GameAttribute a1, GameAttribute a2)
        {
            if (a1.AttType == AttributeType.Number) return new GameAttribute(a1.Num - a2.Num);
            if (a1.AttType == AttributeType.String) return new GameAttribute(a1.Str.Replace(a2.Str, ""));
            if (a1.AttType == AttributeType.Boolean) return new GameAttribute(a1.Bool && !a2.Bool);
            if (a1.AttType == AttributeType.Object) return new GameAttribute(a1.Obj - a2.Obj);
            if (a1.AttType == AttributeType.Array)
            {
                List<GameAttribute> ret = new List<GameAttribute>();
                for (int i = 0; i < Math.Min(a1.Arr.Count, a1.Arr.Count); i++)
                {
                    ret.Add(new GameAttribute(a1.Arr[i] - a2.Arr[i]));
                }
                return new GameAttribute(ret);
            }
            return new GameAttribute();
        }
        public static GameAttribute operator *(GameAttribute a1, GameAttribute a2)
        {
            if (a1.AttType == AttributeType.Number) return new GameAttribute(a1.Num * a2.Num);
            if (a1.AttType == AttributeType.String) return new GameAttribute(a1.Str + a2.ToString());
            if (a1.AttType == AttributeType.Boolean) return new GameAttribute(a1.Bool && a2.Bool);
            if (a1.AttType == AttributeType.Object) return new GameAttribute(a1.Obj * a2.Obj);
            if (a1.AttType == AttributeType.Array)
            {
                List<GameAttribute> ret = new List<GameAttribute>();
                for (int i = 0; i < Math.Min(a1.Arr.Count, a1.Arr.Count); i++)
                {
                    ret.Add(new GameAttribute(a1.Arr[i] * a2.Arr[i]));
                }
                return new GameAttribute(ret);
            }
            return new GameAttribute();
        }
        public static GameAttribute operator /(GameAttribute a1, GameAttribute a2)
        {
            if (a1.AttType == AttributeType.Number) return new GameAttribute(a1.Num / a2.Num);
            if (a1.AttType == AttributeType.String) return new GameAttribute(a1.Str.Replace(a2.ToString(), ""));
            if (a1.AttType == AttributeType.Boolean) return new GameAttribute(a1.Bool == !a2.Bool);
            if (a1.AttType == AttributeType.Object) return new GameAttribute(a1.Obj / a2.Obj);
            if (a1.AttType == AttributeType.Array)
            {
                List<GameAttribute> ret = new List<GameAttribute>();
                for (int i = 0; i < Math.Min(a1.Arr.Count, a1.Arr.Count); i++)
                {
                    ret.Add(new GameAttribute(a1.Arr[i] / a2.Arr[i]));
                }
                return new GameAttribute(ret);
            }
            return new GameAttribute();
        }




        public static explicit operator double(GameAttribute v)
        {
            if (v.HasPointers() && v.AttType == AttributeType.String && v.Str.Contains("^"))
            {
                if (v.GetPointers().Has(v.Str)) return (double)v.GetPointers().Get(v.Str);
            }
            return v.Num;
        }
        public static explicit operator int(GameAttribute v)
        {
            if (v.HasPointers() && v.AttType == AttributeType.String && v.Str.Contains("^"))
            {
                if (v.GetPointers().Has(v.Str)) return (int)v.GetPointers().Get(v.Str);
            }
            return (int)v.Num;
        }
        public static explicit operator float(GameAttribute v)
        {
            if (v.HasPointers() && v.AttType == AttributeType.String && v.Str.Contains("^"))
            {
                if (v.GetPointers().Has(v.Str)) return (float)v.GetPointers().Get(v.Str);
            }
            return (float)v.Num;
        }
        public static explicit operator string(GameAttribute v)
        {
            if (v.HasPointers() && v.AttType == AttributeType.String && v.Str.Contains("^"))
            {
                string s = v.Str;
                foreach (var p in v.GetPointers().ToList()) if (s.Contains(p.Key)) s = s.Replace(p.Key, (string)p.Value);
                return s;
            }
            if (v.AttType == AttributeType.Number) return v.Num.ToString();
            if (v.AttType == AttributeType.Boolean) return v.Bool ? "true" : "false";
            return v.Str;
        }
        public static explicit operator bool(GameAttribute v)
        {
            if (v.HasPointers() && v.AttType == AttributeType.String && v.Str.Contains("^"))
            {
                if (v.GetPointers().Has(v.Str)) return (bool)v.GetPointers().Get(v.Str);
            }
            return v.Bool;
        }
        public static implicit operator AttributeSet(GameAttribute v)
        {
            if (v.HasPointers() && v.AttType == AttributeType.String && v.Str.Contains("^"))
            {
                if (v.GetPointers().Has(v.Str)) return (AttributeSet)v.GetPointers().Get(v.Str);
            }
            return v.Obj;
        }
        public static explicit operator List<GameAttribute>(GameAttribute v)
        {
            if (v.HasPointers() && v.AttType == AttributeType.String && v.Str.Contains("^"))
            {
                if (v.GetPointers().Has(v.Str)) return (List<GameAttribute>)v.GetPointers().Get(v.Str);
            }
            return v.Arr;
        }
        public static explicit operator Microsoft.Xna.Framework.Color(GameAttribute v)
        {
            try
            {
                System.Drawing.Color c = ColorTranslator.FromHtml((string)v);
                return new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);
            }
            catch { return Microsoft.Xna.Framework.Color.White; }
        }




        private int _position = -1;
        public object Current => Arr[_position];
        public bool MoveNext()
        {
            _position++; return _position < Arr.Count;
        }

        public void Reset()
        {
            _position = 0;
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }

        public int Count => Arr.Count;

        public List<GameAttribute> ToList() { return Arr; }
    }
}
