using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Application.Features.Customers.Commands.Delete;
using WaterS.Application.Features.Customers.Commands.UnDelete;

using WaterS.Application.Features.Customers.Queries.Export;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Application.Features.Customers.Queries.GetCustomerById;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class CustomersController : BaseApiController<CustomersController>
    {
        /// <summary>
        /// Get All Customers
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        //[AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, int customerId, int companyId, int stationId, int driverId, int regionId, int without, string searchString,string SearchBy,string Statue, string orderBy = null)
        {
            var Customers = await _mediator.Send(new GetAllCustomersQuery(pageNumber,pageSize, customerId, companyId, stationId, driverId, regionId, without, searchString,SearchBy, Statue, orderBy));
            return Ok(Customers);
        }


       
        /// <summary>
        /// Get a Customer Image by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
       // [Authorize(Policy = Permissions.Customers.View)]//
        [AllowAnonymous]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetCustomerAsync(int id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Add/Edit a Customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Customers.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditCustomerCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK response</returns>
        [Authorize(Policy = Permissions.Customers.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteCustomerCommand { Id = id }));
        }


        [HttpPost("bymac")]
        public async Task<IActionResult> UnDelete(int id)
        {
            return Ok(await _mediator.Send(new UnDeleteCustomerCommand { Id = id }));
        }
        /// <summary>
        /// Search Customers and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Customers.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportCustomersQuery(searchString)));
        }
    }
}