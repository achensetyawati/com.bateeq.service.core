using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class Item : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Uom { get; set; }

        [StringLength(255)]
        public string Tags { get; set; }

        [StringLength(255)]
        public string Remark { get; set; }

        [StringLength(255)]
        public string ArticleRealizationOrder { get; set; }

      
        public string Size { get; set; }

        public string ImgFile { get; set; }
        [StringLength(255)]
        public string ImagePath { get; set; }
        
        public int ArticleColorsId { get; set; }

        [StringLength(255)]
        public string ColorCode { get; set; }

        [StringLength(255)]
        public string ColorDocName { get; set; }

        public int ArticleProcessId { get; set; }

        [StringLength(255)]
        public string ProcessDocCode { get; set; }
        [StringLength(255)]
        public string ProcessDocName { get; set; }
        
        public int ArticleMaterialsId { get; set; }

        [StringLength(255)]
        public string MaterialDocCode { get; set; }
        [StringLength(255)]
        public string MaterialDocName { get; set; }
        
        public int ArticleMaterialCompositionsId { get; set; }


        [StringLength(255)]
        public string MaterialCompositionDocCode { get; set; }
        [StringLength(255)]
        public string MaterialCompositionDocName { get; set; }

        public int ArticleCollectionsId { get; set; }

        [StringLength(255)]
        public string CollectionDocCode { get; set; }

        [StringLength(255)]
        public string CollectionDocName { get; set; }

        public int ArticleSeasonsId { get; set; }

        [StringLength(255)]
        public string SeasonDocCode { get; set; }
        [StringLength(255)]
        public string SeasonDocName { get; set; }

        public int ArticleCountersId { get; set; }

        [StringLength(255)]
        public string CounterDocCode { get; set; }

        [StringLength(255)]
        public string CounterDocName { get; set; }

        public int ArticleSubCountersId { get; set; }

        [StringLength(255)]
        public string StyleDocCode { get; set; }

        [StringLength(255)]
        public string StyleDocName { get; set; }

        public int ArticleCategoriesId { get; set; }

        [StringLength(255)]
        public string CategoryDocCode { get; set; }

        [StringLength(255)]
        public string CategoryDocName { get; set; }

        public double? DomesticCOGS { get; set; }

        public double? DomesticWholesale { get; set; }

        public double? DomesticRetail { get; set; }

        public double? DomesticSale { get; set; }

        public double? InternatinalCOGS { get; set; }

        public double? InternationalWholesale { get; set; }

        public double? InternatioalRetail { get; set; }

        public double? InternationalSale { get; set; }

        public double TotalQty { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // throw new NotImplementedException();

            List<ValidationResult> validationResult = new List<ValidationResult>();

            return validationResult;
        }






    }
}
