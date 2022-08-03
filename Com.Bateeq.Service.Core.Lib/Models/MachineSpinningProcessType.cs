using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class MachineSpinningProcessType : StandardEntity
    {
        public string Type { get; set; }

        public virtual int MachineSpinningId { get; set; }

        [ForeignKey("MachineSpinningId")]
        public virtual MachineSpinningModel MachineSpinning { get; set; }
    }
}
