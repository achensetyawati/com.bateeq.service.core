using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Models.Module;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class Storage : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? UnitId { get; set; }

        public string UnitName { get; set; }

        public string DivisionName { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(255)]
        public string Phone { get; set; }

        public bool IsCentral { get; set; }
        public virtual ICollection<ModuleSource> ModuleSources { get; set; }
        public virtual ICollection<ModuleDestination> ModuleDestinations { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(this.Code))
                yield return new ValidationResult("Kode tidak boleh kosong", new List<string> { "code" });

            if (string.IsNullOrWhiteSpace(this.Name))
                yield return new ValidationResult("Nama tidak boleh kosong", new List<string> { "name" });

            if (this.UnitId.Equals(null))
                yield return new ValidationResult("Unit tidak boleh kosong", new List<string> { "unit" });

            
            /* Service Validation */
            StorageService service = (StorageService)validationContext.GetService(typeof(StorageService));

            if (service.DbContext.Set<Storage>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)  ) > 0)
            {
                yield return new ValidationResult("Kode sudah ada", new List<string> { "code" });
            }

            if ( ModuleSources.Count==0 && ModuleDestinations.Count == 0)
            {
                yield return new ValidationResult("silakan pilih module destination/source", new List<string> { "modules" });
            }
            else
            {
                if (ModuleSources.Count > 0)
                {
                    string sourceError = "[";
                    int sourceErrorCount = 0;
                    foreach (var s in ModuleSources)
                    {
                        sourceError += "{";

                        if (s.ModuleId == 0)
                        {
                            sourceErrorCount++;
                            sourceError += "moduleSource: 'nama module harus dipilih', ";
                        }



                        sourceError += "}, ";
                    }

                    sourceError += "]";

                    if (sourceErrorCount > 0)
                        yield return new ValidationResult(sourceError, new List<string> { "ModuleSources" });
                }
                else if (ModuleDestinations.Count > 0)
                {
                    string destinationError = "[";
                    int destinationErrorCount = 0;
                    foreach (var s in ModuleDestinations)
                    {
                        destinationError += "{";

                        if (s.ModuleId == 0)
                        {
                            destinationErrorCount++;
                            destinationError += "moduleSource: 'nama module harus dipilih', ";
                        }
                        destinationError += "}, ";
                    }

                    destinationError += "]";

                    if (destinationErrorCount > 0)
                        yield return new ValidationResult(destinationError, new List<string> { "ModuleDestinations" });
                }
            }

            

            //return validationResult;
        }
    }
}
