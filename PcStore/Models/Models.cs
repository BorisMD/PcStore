using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcStore.Models
{
    public class CPU : ComponentBase
    {
        public int Cores { get; set; }
        public double ClockSpeed { get; set; }
    }

    public class GPU : ComponentBase
    {
        public int MemorySize { get; set; }
        public string MemoryType { get; set; }
    }

    public class Motherboard : ComponentBase
    {
        public string Socket { get; set; }
        public int MemorySlots { get; set; }
    }

    public class RAM : ComponentBase
    {
        public int Capacity { get; set; }
        public int Speed { get; set; }
    }

    public class Memory : ComponentBase
    {
        public int Capacity { get; set; }
        public string Type { get; set; }
    }

    public class PowerSupply : ComponentBase
    {
        public int Wattage { get; set; }
        public string Efficiency { get; set; }
    }

}
