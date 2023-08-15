using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterS.Application.Features.Companies.Commands.AddEdit;
using WaterS.Application.Features.Companies.Commands.Delete;
using WaterS.Application.Features.Companies.Queries.Export;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Application.Features.Companies.Queries.GetById;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Server.Controllers.v1.Catalog
{
    public class CompaniesController : BaseApiController<CompaniesController>
    {
        /// <summary>
        /// Get All Brands
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _mediator.Send(new GetAllCompaniesQuery());
            return Ok(brands);
        }

        /// <summary>
        /// Get a Brand By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Companies.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _mediator.Send(new GetCompanyByIdQuery() { Id = id });
            return Ok(brand);
        }

        /// <summary>
        /// Create/Update a Brand
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Companies.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditCompanyCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Companies.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteCompanyCommand { Id = id }));
        }

        /// <summary>
        /// Search Brands and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.Companies.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportCompaniesQuery(searchString)));
        }
    }
}