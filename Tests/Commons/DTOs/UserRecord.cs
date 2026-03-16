using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Commons.DTOs
{
    public record UserRecord(
        string Uid,
        string? Email,
        string? DisplayName
        );
}
