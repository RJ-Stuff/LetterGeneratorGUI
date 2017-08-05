namespace LetterApp.model
{
    class Filter
    {
        public string DisplayName { get; set; }
        public string InternalName { get; set; }
        public object Value { get; set; }

        public Filter(string displayName, string internalName, object value)
        {
            this.DisplayName = displayName;
            this.InternalName = internalName;
            this.Value = value;
        }

        public override string ToString()
        {
            var value = Value.ToString().Trim().Length != 0 ? " = " + Value : string.Empty;
            return $"{DisplayName}{value}";
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
