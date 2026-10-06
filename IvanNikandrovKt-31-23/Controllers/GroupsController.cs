using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IvanNikandrovKt_31_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupsController : ApiBaseController
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>Список групп (фильтр: специальность, год/курс, статус удаления).</summary>
        [HttpPost("list")]
        public async Task<IActionResult> GetGroups([FromBody] GroupFilter filter, CancellationToken ct)
        {
            return Ok(await _groupService.GetGroupsAsync(filter, ct));
        }

        [HttpPost]
        public async Task<IActionResult> AddGroup([FromBody] GroupRequest request, CancellationToken ct)
        {
            return ToResult(await _groupService.AddGroupAsync(request, ct));
        }

        [HttpPut("{groupId:int}")]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] GroupRequest request, CancellationToken ct)
        {
            return ToResult(await _groupService.UpdateGroupAsync(groupId, request, ct));
        }

        /// <summary>Удаление группы (вместе со студентами группы).</summary>
        [HttpDelete("{groupId:int}")]
        public async Task<IActionResult> DeleteGroup(int groupId, CancellationToken ct)
        {
            return ToResult(await _groupService.DeleteGroupAsync(groupId, ct));
        }
    }
}
