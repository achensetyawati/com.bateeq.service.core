using Com.Moonlay.Models;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class MachineSpinningModel : StandardEntity
    {
        public string No { get; set; }
        public string Code { get; set; }

        public string Brand { get; set; }
        public string Name { get; set; }
        //public string Type { get; set; }
        public int Year { get; set; }
        public string Condition { get; set; }
        public string CounterCondition { get; set; }
        public int Delivery { get; set; }
        public double CapacityPerHour { get; set; }
        public string UomId { get; set; }
        public string UomUnit { get; set; }
        public string Line { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string MachineCode { get; set; }


        public virtual ICollection<MachineSpinningProcessType> Types { get; set; }
    }
}
