using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;



public class ObjectDto
    {

    public long ProductId {  get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string? Brand { get; set; }
    public string? Color { get; set; }
    public string? Size { get; set; }
    public int year { get; set; }
    public double ProductPrice { get; set; }
    public double VariantPrice { get; set; }
    public string? Sku { get; set; }
    public string? SSN { get; set; }
    public string? ShortDescription { get; set; }
    public string? LongDescription { get; set; }
    public string? ProductCode { get; set; }
    public string? EanNumber { get; set; }
    public string VariantType { get; set; }
    public string? Value { get; set; }
    public string? Thumbnailurl { get; set; }
    public byte[] Thumbnail { get; set; }
    public string PrintedOn {  get; set; }
    public List<ObjectDtoFeatureValue> ProductVariantFeatureValues { get; set; } = new List<ObjectDtoFeatureValue>();

    }
public class ObjectDtoFeatureValue
    {
    public long GroupId { get; set; }
    public string GroupName { get; set; }
    public string FeatureName { get; set; }
    public string? FeatureValue { get; set; }
    }
