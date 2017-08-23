namespace LetterApp.model
{
    using System.Linq;

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
            var value = (Value ?? string.Empty).ToString().Trim();
            var op = "=<>";

            if (!string.IsNullOrEmpty(value) && !value.Any(c => op.Contains(c)))
            {
                value = $" = {value}";
            }
            
            return $"{DisplayName}{value}";
        }

        public string InternalToString()
        {
            return ToString().Replace(DisplayName, InternalName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

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

            return GetHashCode() == item.GetHashCode();
        }
    }

}
