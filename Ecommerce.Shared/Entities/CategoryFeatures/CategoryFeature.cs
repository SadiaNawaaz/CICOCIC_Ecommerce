using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities.CategoryFeatures;

public class CategoryFeature: BaseEntity
    {


    public int IceCatFeatureGroupId { get; set; }   
    public int CategoryId { get; set; }
    public int FeatureId { get; set; }
    public int FeatureGroupId { get; set; }
    public int? MeasureId { get; set; }
    public string ?Name { get; set; }
    }

