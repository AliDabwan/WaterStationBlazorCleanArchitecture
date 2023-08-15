using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.AccountMovments.Commands.AddEdit;
using WaterS.Application.Features.AccountMovments.Commands.Delete;
using WaterS.Application.Features.AccountMovments.Queries.Export;
using WaterS.Application.Features.AccountMovments.Queries.GetAll;
using WaterS.Application.Features.AccountMovments.Queries.GetById;
using WaterS.Application.Features.AccountMovments.Queries.GetByAccountId;

using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class AccountMovmentsController : BaseApiController<AccountMovmentsController>
    {
        /// <summary>
        /// Get All Brands
        /// </summary>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.AccountMovments.View)]
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var brands = await _mediator.Send(new GetAllAccountMovmentsQuery());
        //    return Ok(brands);
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllByAccount(int pageNumber, int pageSize, int accountId, string searchString, string orderBy = null)
        {
            var DriverRegions = await _mediator.Send(new GetAllAccountMovmentsQuery(pageNumber, pageSize, accountId, searchString, orderBy));
            return Ok(DriverRegions);
        }
        /// <summary>
        /// Get a Brand By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.AccountMovments.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _mediator.Send(new GetAccountMovmentByIdQuery(id) { Id = id });
            return Ok(brand);
        }


        [AllowAnonymous]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetAccountMovmentsAsync(int id)
        {
            var result = await _mediator.Send(new GetAccountMovmentByIdQuery(id));
            return Ok(result);
        }




        /// <summary>
        /// Get a Balance By AccId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.AccountMovments.View)]
        [HttpGet("accountsid/{id}")]
        public async Task<IActionResult> GetAccountBalanceAsync(int id)
        {
            var brand = await _mediator.Send(new GetAccountMovmentByAccountIdQuery(id));
            return Ok(brand);
        }


        /// <summary>
        /// Create/Update a Brand
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.AccountMovments.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditAccountMovmentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.AccountMovments.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteAccountMovmentCommand { Id = id }));
        }

        /// <summary>
        /// Search Brands and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.AccountMovments.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportAccountMovmentsQuery(searchString)));
        }
    }
}