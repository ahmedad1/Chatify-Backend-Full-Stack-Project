using RepositoryPatternUOW.Core.DTOs;
using RepositoryPatternUOW.Core.Models;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternUOW.EFcore.Mapperly
{
    [Mapper]
    public partial class MapToModel
    {
        public partial User MapSignUpDto(SignUpDto signUpDto);

    }

}
