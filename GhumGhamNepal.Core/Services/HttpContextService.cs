using GhumGham_Nepal.Repository;
using GhumGhamNepal.Core.Models.AspNetEntity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhumGhamNepal.Core.Services
{
    public class HttpContextService : IHttpContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<AspNetUser> _userRepository;
        private AspNetUser _curentUser;
        private readonly object _lock = new Object();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public HttpContextService(IHttpContextAccessor httpContext, IRepository<AspNetUser> userRepository)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _httpContextAccessor = httpContext;
            _userRepository = userRepository;
        }

        public AspNetUser User
        {
            get
            {
                lock (_lock)
                {
                    if (_curentUser != null)
                        return _curentUser;

                    var currentUserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
                    if (currentUserName == null)
#pragma warning disable CS8603 // Possible null reference return.
                        return _curentUser;
#pragma warning restore CS8603 // Possible null reference return.

#pragma warning disable CS8601 // Possible null reference assignment.
                    _curentUser = _userRepository.Table.FirstOrDefault(x => x.UserName == currentUserName);
#pragma warning restore CS8601 // Possible null reference assignment.
                }
#pragma warning disable CS8603 // Possible null reference return.
                return _curentUser;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
    }
}
