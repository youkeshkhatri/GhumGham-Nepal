using GhumGhamNepal.Core.Models.AspNetEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhumGhamNepal.Core.Services
{
    public interface IHttpContextService
    {
        AspNetUser User { get; }
    }
}
