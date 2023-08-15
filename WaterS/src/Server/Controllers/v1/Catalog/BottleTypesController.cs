using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.BottleTypes.Commands.AddEdit;
using WaterS.Application.Features.BottleTypes.Commands.Delete;
using WaterS.Application.Features.BottleTypes.Queries.Export;
using WaterS.Application.Features.BottleTypes.Queries.GetAll;
using WaterS.Application.Features.BottleTypes.Queries.GetById;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class BottleTypesController : BaseApiController<BottleTypesController>
    {
        /// <summary>
        /// Get All Brands
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.BottleTypes.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _mediator.Send(new GetAllBottleTypesQuery());
            return Ok(brands);
        }

        /// <summary>
        /// Get a Brand By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.BottleTypes.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _mediator.Send(new GetBottleTypeByIdQuery() { Id = id });
            return Ok(brand);
        }

        /// <summary>
        /// Create/Update a Brand
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.BottleTypes.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditBottleTypeCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.BottleTypes.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteBottleTypeCommand { Id = id }));
        }

        /// <summary>
        /// Search Brands and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.BottleTypes.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportBottleTypesQuery(searchString)));
        }
    }
}