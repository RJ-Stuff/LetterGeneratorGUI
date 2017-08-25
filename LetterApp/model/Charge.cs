namespace LetterApp.model
{
    class Charge
    {
        public static readonly Charge DefaultCharge = new Charge("SimpleCharge", "Cargo simple");

        public string ChargeClazz { get; set; }

        public string DisplayName { get; set; }

        public Charge() { }

        public Charge(string chargeClazz, string displayName)
        {
            ChargeClazz = chargeClazz;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + ChargeClazz.GetHashCode();
                hash = hash * 23 + DisplayName.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetHashCode() == GetHashCode();
        }
    }
}
