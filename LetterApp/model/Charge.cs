namespace LetterApp.model
{
    class Charge
    {
        public static readonly Charge DEFAULT_CHARGE = new Charge("", "Sin cargo");

        public string ChargeClazz { get; set; }
        public string DisplayName { get; set; }

        public Charge() { }

        public Charge(string ChargeClazz, string DisplayName)
        {
            this.ChargeClazz = ChargeClazz;
            this.DisplayName = DisplayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + ChargeClazz.GetHashCode();
                hash = hash * 23 + DisplayName.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
