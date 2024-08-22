﻿using Ecommerce.Shared.Entities.Roles;
using Ecommerce.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.Roles;


public class UserRole
{
    public long UserId { get; set; }
    public User User { get; set; }

    public long RoleId { get; set; }
    public Role Role { get; set; }
}
