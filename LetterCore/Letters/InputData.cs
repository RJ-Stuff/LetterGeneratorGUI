using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letters
{
    public class InputData
    {
        public InputData()
        {

        }

        public int GetCount()
        {
            return 0;
        }

        public List<Client> GetClients()
        {
            var names = $@"{Directory.GetCurrentDirectory()}\resources\names.txt";
            var address = $@"{Directory.GetCurrentDirectory()}\resources\address.txt";

            var nameArr = File.ReadAllLines(names);
            var addressArr = File.ReadAllLines(address);
            //cuando tenga el dataset actualizo esto, pero lo importante es lo otro.
            var r = new Random();
            return new List<Client>() {
                new Client(r.Next(1234, 8765),nameArr[r.Next(nameArr.Length)], 230.7F,
                "12345678", addressArr[r.Next(addressArr.Length)], addressArr[r.Next(addressArr.Length)],
                addressArr[r.Next(addressArr.Length)],
                new List<DisaggregatedDebt>()
                {
                    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                })
                //,
                //new Client(r.Next(1234, 8765),nameArr[r.Next(nameArr.Length)],addressArr[r.Next(addressArr.Length)], 23.7F,
                //new List<DisaggregatedDebt>()
                //{
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //}),
                //new Client(r.Next(1234, 8765),nameArr[r.Next(nameArr.Length)],addressArr[r.Next(addressArr.Length)], 23.7F,
                //new List<DisaggregatedDebt>()
                //{
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //}),
                //new Client(r.Next(1234, 8765),nameArr[r.Next(nameArr.Length)],addressArr[r.Next(addressArr.Length)], 23.7F,
                //new List<DisaggregatedDebt>()
                //{
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //}),
                //new Client(r.Next(1234, 8765),nameArr[r.Next(nameArr.Length)],addressArr[r.Next(addressArr.Length)], 23.7F,
                //new List<DisaggregatedDebt>()
                //{
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //    new DisaggregatedDebt(122, "Teléfono fijo", "555-55555", DateTime.Now.AddDays(-1 * r.Next(1, 7)), 4, 50F),
                //}),
            };
        }
    }
}
