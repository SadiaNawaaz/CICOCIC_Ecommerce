using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Enums;

public enum ResetPasswordStatus
    {
    Allow,
    MissingEmail,
    ProductNotFound,
    LinkNotFound,
    EmailMismatch,
    AlreadyClaimed
    }