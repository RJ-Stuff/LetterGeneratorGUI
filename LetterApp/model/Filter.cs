using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterApp.model
{
    class Filter
    {
        public string DisplayName { get; set; }
        public string InternalName { get; set; }
        public object Value { get; set; }

        public Filter(string DisplayName, string InternalName, object Value)
        {
            this.DisplayName = DisplayName;
            this.InternalName = InternalName;
            this.Value = Value;
        }

        public override string ToString()
        {
            var value = Value.ToString().Trim().Length != 0 ? " = " + Value.ToString() : "";
            return $"{DisplayName}{value}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                
                hash = hash * 23 + DisplayName.GetHashCode();
                hash = hash * 23 + InternalName.GetHashCode();
                hash = hash * 23 + Value.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as Filter;

            if (item == null)
            {
                return false;
            }

            return this.GetHashCode() == item.GetHashCode();
        }
    }
}
