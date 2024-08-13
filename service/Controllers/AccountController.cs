using Microsoft.AspNetCore.Mvc;
using service.Infrastructure.Data;

namespace service.Controllers
{
    /// <summary>
    /// Controller class for handling account management related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(YourEntityRepository repository, ILogger<AccountController> logger) : ControllerBase
    {
        private readonly YourEntityRepository _repository = repository;
        private readonly ILogger<AccountController> _logger = logger;

        /// <summary>
        /// Getting All Data from Users Table.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            _logger.LogInformation("Getting All Data from Users Table.");
            var entities = await _repository.GetAllAsync();
            return Ok(entities);
        }
    }
}
