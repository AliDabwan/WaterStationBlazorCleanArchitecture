using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.Drivers.Commands.AddEdit;
using WaterS.Application.Features.Drivers.Commands.Delete;
using WaterS.Application.Features.Drivers.Queries.Export;
using WaterS.Application.Features.Drivers.Queries.GetAllDrivers;
using WaterS.Application.Features.Drivers.Queries.GetDriverById;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class DriversController : BaseApiController<DriversController>
    {
        /// <summary>
        /// Get All Drivers
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.Drivers.View)]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, int companyId, int stationId, int driverId,int without, string searchString, string orderBy = null)
        {
            var Drivers = await _mediator.Send(new GetAllDriversQuery(pageNumber, pageSize,  searchString, companyId, stationId, driverId, orderBy,without));
            return Ok(Drivers);
        }


       
        /// <summary>
        /// Get a Driver Image by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
       // [Authorize(Policy = Permissions.Drivers.View)]//
        [AllowAnonymous]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetDriverAsync(int id)
        {
            var result = await _mediator.Send(new GetDriverByIdQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Add/Edit a Driver
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Drivers.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDriverCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Driver
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.Drivers.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteDriverCommand { Id = id }));
        }

        /// <summary>
        /// Search Drivers and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Drivers.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportDriversQuery(searchString)));
        }
    }
}