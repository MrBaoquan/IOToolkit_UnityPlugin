using System.Text.RegularExpressions;
using UnityEngine;

namespace IOToolkit.Extension
{
    public static class IOToolkitExtension
    {
        public static int GetIntValue(this Key key)
        {
            var _numberPart = Regex.Replace(key.ToString(), @"^[A-Za-z]+_", string.Empty);
            if (!Regex.IsMatch(_numberPart, @"^\d+$"))
                return -1;
            if (int.TryParse(_numberPart, out int _value) == false)
                return -1;
            return _value;
        }
    }
}
