using IvanNikandrovKt_31_23.Dto;
using Microsoft.AspNetCore.Mvc;

namespace IvanNikandrovKt_31_23.Controllers
{
    /// <summary>Общая логика контроллеров: превращает результат сервиса в HTTP-ответ.</summary>
    public abstract class ApiBaseController : ControllerBase
    {
        protected IActionResult ToResult<T>(ServiceResult<T> result)
        {
            switch (result.Status)
            {
                case ServiceStatus.Ok:
                    return Ok(result.Data);
                case ServiceStatus.NotFound:
                    return NotFound(new { error = result.Message });
                default:
                    return BadRequest(new { error = result.Message });
            }
        }
    }
}
