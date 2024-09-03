using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;

public class ClusterFeatureDto
{
    public string? Cluster { get; set; }
    public long ClusterId { get; set; }
    public int Order { get; set; }
    public List<FeatureValuePair>? Features { get; set; }
}

public class FeatureValuePair
{
    public string Feature { get; set; }
    public string Value { get; set; }
}


public class RawClusterFeatureDto
{
    public string Cluster { get; set; }
    public string Feature { get; set; }
    public long ClusterId { get; set; }
    public long FeatureId { get; set; }
    public string? Value { get; set; }
    public int Order { get; set; } = 0;
}

