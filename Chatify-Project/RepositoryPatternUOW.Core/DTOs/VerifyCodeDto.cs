﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.DTOs
{
    public record VerifyCodeDto(string Email,string Code);
}
