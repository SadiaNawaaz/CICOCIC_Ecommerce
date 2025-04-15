using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;

public class AdyenSettings
    {
    public string ApiKey { get; set; }
    public string ClientKey { get; set; }
    public string MerchantAccount { get; set; }
    public string ReturnUrl { get; set; }
    }
