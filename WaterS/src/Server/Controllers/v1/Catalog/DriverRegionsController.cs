using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.DriverRegions.Commands.AddEdit;
using WaterS.Application.Features.DriverRegions.Commands.Delete;
using WaterS.Application.Features.DriverRegions.Queries.Export;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Application.Features.DriverRegions.Queries.GetDriverRegion;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class DriverRegionsController : BaseApiController<DriverRegionsController>
    {
        /// <summary>
        /// Get All DriverRegions
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize,int companyId, int stationId, 
            int driverId,int regionId, string searchString, string orderBy = null)
        {
            var DriverRegions = await _mediator.Send(new GetAllDriverRegionsQuery(pageNumber, pageSize, companyId, stationId,
                driverId, regionId, searchString, orderBy));
            return Ok(DriverRegions);
        }



        /// <summary>
        /// Get a DriverRegion Image by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
       // [Authorize(Policy = Permissions.DriverRegions.View)]//
        [AllowAnonymous]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetDriverRegionAsync(int id)
        {
            var result = await _mediator.Send(new GetDriverRegionQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Add/Edit a DriverRegion
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DriverRegions.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDriverRegionCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a DriverRegion
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.DriverRegions.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteDriverRegionCommand { Id = id }));
        }

        /// <summary>
        /// Search DriverRegions and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DriverRegions.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportDriverRegionsQuery(searchString)));
        }
    }
}