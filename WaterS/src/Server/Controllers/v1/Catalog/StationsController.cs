using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.Stations.Commands.AddEdit;
using WaterS.Application.Features.Stations.Commands.Delete;
using WaterS.Application.Features.Stations.Queries.Export;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Application.Features.Stations.Queries.GetStationById;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class StationsController : BaseApiController<StationsController>
    {
        /// <summary>
        /// Get All Stations
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Stations = await _mediator.Send(new GetAllStationsQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(Stations);
        }

        /// <summary>
        /// Get a Station Image by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.Stations.View)]//
        [AllowAnonymous]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetStationAsync(int id)
        {
            var result = await _mediator.Send(new GetStationByIdQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Add/Edit a Station
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Stations.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditStationCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Station
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.Stations.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteStationCommand { Id = id }));
        }

        /// <summary>
        /// Search Stations and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Stations.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportStationsQuery(searchString)));
        }
    }
}