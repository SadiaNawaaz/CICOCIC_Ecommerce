using Ecommerce.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Dto;

public class ResetPasswordGateResultDto
    {
    public bool Allow { get; set; }
    public ResetPasswordStatus Status { get; set; }
    public string Message { get; set; } = "";
    }

