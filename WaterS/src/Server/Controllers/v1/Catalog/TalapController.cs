using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WaterS.Application.Features.Talaps.Commands.AddEdit;
using WaterS.Application.Features.Talaps.Commands.Delete;
using WaterS.Application.Features.Talaps.Queries.Export;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Application.Features.Talaps.Queries.GetTalapById;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class TalapsController : BaseApiController<TalapsController> 
    {
        /// <summary>
        /// Get All Talaps
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Talaps.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize,int customerId, int companyId, int stationId, int driverId,int regionId, string statue,
            DateTime? dateFrom, DateTime? dateTo, string searchString, string orderBy = null)
        {
            var Talaps = await _mediator.Send(new GetAllTalapsQuery(pageNumber, pageSize, customerId, companyId, stationId, driverId, regionId, statue,
               dateFrom, dateTo, searchString, orderBy));
            return Ok(Talaps);
        }



        /// <summary>
        /// Get a Talap Image by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Talaps.View)]//
        //[AllowAnonymous]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetTalapAsync(int id)
        {
            var result = await _mediator.Send(new GetTalapByIdQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Add/Edit a Talap
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Talaps.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditTalapCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Talap
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.Talaps.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteTalapCommand { Id = id }));
        }

        /// <summary>
        /// Search Talaps and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Talaps.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportTalapsQuery(searchString)));
        }
    }
}