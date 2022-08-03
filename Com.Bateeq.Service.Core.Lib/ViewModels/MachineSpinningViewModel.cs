using Com.DanLiris.Service.Core.Lib.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class MachineSpinningViewModel : BasicViewModel, IValidatableObject
    {
        public string No { get; set; }
        public string Code { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public int? Year { get; set; }
        public string Condition { get; set; }
        //public string Type { get; set; }
        public string CounterCondition { get; set; }
        public int? Delivery { get; set; }
        public double? CapacityPerHour { get; set; }
        public string UomId { get; set; }
        public string UomUnit { get; set; }
        public string Line { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string MachineCode { get; set; }


        public List<MachineSpinningProcessTypeViewModel> Types { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(No))
            {
                yield return new ValidationResult("No Mesin harus diisi", new List<string>() { "No" });
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Merk Mesin harus diisi", new List<string> { "Name" });
            }

            if (string.IsNullOrWhiteSpace(Brand))
            {
                yield return new ValidationResult("Type Mesin harus diisi", new List<string> { "Brand" });

            }

            if (string.IsNullOrEmpty(UnitName))
                yield return new ValidationResult("Unit harus diisi", new List<string> { "Unit" });

            if (!string.IsNullOrEmpty(No) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(UnitName) && !string.IsNullOrEmpty(Line)
                    && !string.IsNullOrEmpty(Brand) /*&& !string.IsNullOrEmpty(Type)*/)
            {
                CoreDbContext dbContext = validationContext == null ? null : (CoreDbContext)validationContext.GetService(typeof(CoreDbContext));
                var duplicate = dbContext.MachineSpinnings.Where(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Name.Equals(this.Name)
                            && r.No == No && r.UnitName == UnitName && r.Line == Line && r.Brand == Brand && r.MachineCode == MachineCode).Count();

                if (duplicate > 0) /* Name Unique */
                {
                    yield return new ValidationResult("Mesin sudah ada", new List<string> { "Name" });
                    yield return new ValidationResult("Mesin sudah ada", new List<string> { "No" });
                    yield return new ValidationResult("Mesin sudah ada", new List<string> { "Unit" });
                    yield return new ValidationResult("Mesin sudah ada", new List<string> { "Line" });
                    yield return new ValidationResult("Mesin sudah ada", new List<string> { "Brand" });
                    //yield return new ValidationResult("Mesin sudah ada", new List<string> { "Type" });
                    yield return new ValidationResult("Mesin sudah ada", new List<string> { "MachineCode" });
                }
            }

            if (Year == null || Year <= 0)
            {
                yield return new ValidationResult("Tahun harus diisi", new List<string> { "Year" });
            }

            if (string.IsNullOrWhiteSpace(Condition))
            {
                yield return new ValidationResult("Kondisi harus diisi", new List<string> { "Condition" });
            }

            if (string.IsNullOrWhiteSpace(CounterCondition))
            {
                yield return new ValidationResult("Kondisi Counter harus diisi", new List<string> { "CounterCondition" });
            }

            //if (string.IsNullOrWhiteSpace(Type))
            //{
            //    yield return new ValidationResult("Jenis Proses harus diisi", new List<string> { "Type" });
            //}

            if (Delivery == null || Delivery <= 0)
            {
                yield return new ValidationResult("Delivery harus diisi", new List<string> { "Delivery" });
            }

            if (string.IsNullOrWhiteSpace(UomUnit))
            {
                yield return new ValidationResult("Satuan harus diisi", new List<string> { "Uom" });
            }

            if (CapacityPerHour == null || CapacityPerHour <= 0)
            {
                yield return new ValidationResult("Kapasitas per Jam harus diisi", new List<string> { "CapacityPerHour" });
            }

            if (string.IsNullOrEmpty(Line))
                yield return new ValidationResult("Line harus diisi", new List<string> { "Line" });

            if (string.IsNullOrEmpty(MachineCode))
            {
                yield return new ValidationResult("Machine Code harus diisi", new List<string>() { "MachineCode" });
            }

            if(Types == null || Types.Count == 0)
            {
                yield return new ValidationResult("Types harus diisi", new List<string>() { "Types" });
            }
            else
            {
                foreach(var item in Types)
                {
                    if (Types.Any(x => x != item && x.Type == item.Type))
                    {
                        yield return new ValidationResult("Jenis Proses Tidak bisa duplikat", new List<string>() { "Types" });
                        break;
                    }
                        

                }
            }


        }
    }

}
