using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.AccountNames.Commands.AddEdit;
using WaterS.Application.Features.AccountNames.Commands.Delete;
using WaterS.Application.Features.AccountNames.Queries.Export;
using WaterS.Application.Features.AccountNames.Queries.GetAll;
using WaterS.Application.Features.AccountNames.Queries.GetById;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class AccountNamesController : BaseApiController<AccountNamesController>
    {
        /// <summary>
        /// Get All Accounts
        /// </summary>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.AccountName.View)]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _mediator.Send(new GetAllAccountNamesQuery());
            return Ok(brands);
        }

        /// <summary>
        /// Get a Brand By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        //[Authorize(Policy = Permissions.AccountName.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _mediator.Send(new GetAccountNameByIdQuery() { Id = id });
            return Ok(brand);
        }

        /// <summary>
        /// Create/Update a Brand
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.AccountName.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditAccountNameCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.AccountName.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteAccountNameCommand { Id = id }));
        }

        /// <summary>
        /// Search Brands and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.AccountName.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportAccountNameQuery(searchString)));
        }
    }
}