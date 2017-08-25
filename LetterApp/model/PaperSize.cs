namespace LetterApp.model
{
    using System.Collections.Generic;

    class PaperSize
    {
        public static readonly PaperSize DefaultSize =
            new PaperSize(
                "Papel A4",
                new List<Charge> {
                    Charge.DefaultCharge,
                    new Charge("SpecialCharge", "Cargo especial") });

        public string DisplayName { get; set; }

        public List<Charge> Charges { get; set; }

        public PaperSize()
        {

        }

        public PaperSize(string displayName, List<Charge> charges)
        {
            DisplayName = displayName;
            Charges = charges;
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
