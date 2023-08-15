using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.Regions.Commands.AddEdit;
using WaterS.Application.Features.Regions.Commands.Delete;
using WaterS.Application.Features.Regions.Queries.Export;
using WaterS.Application.Features.Regions.Queries.GetAll;
using WaterS.Application.Features.Regions.Queries.GetById;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class RegionsController : BaseApiController<RegionsController>
    {
        /// <summary>
        /// Get All Brands
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _mediator.Send(new GetAllRegionsQuery());
            return Ok(brands);
        }

        /// <summary>
        /// Get a Brand By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        //[Authorize(Policy = Permissions.Regions.View)]
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var brand = await _mediator.Send(new GetRegionByIdQuery() { Id = id });
        //    return Ok(brand);
        //}


        [AllowAnonymous]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetRegionAsync(int id)
        {
            var result = await _mediator.Send(new GetRegionByIdQuery(id));
            return Ok(result);
        }
        /// <summary>
        /// Create/Update a Brand
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Regions.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditRegionCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Regions.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteRegionCommand { Id = id }));
        }

        /// <summary>
        /// Search Brands and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.Regions.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportRegionsQuery(searchString)));
        }
    }
}