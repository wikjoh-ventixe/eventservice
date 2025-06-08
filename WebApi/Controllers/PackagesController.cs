using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController(IPackageService packageService) : ControllerBase
    {
        private readonly IPackageService _packageService = packageService;

        // READ
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllByEventId(string id)
        {
            var packagesResult = await _packageService.GetAllPackagesByEventId(id);
            var packages = packagesResult.Data;

            return packagesResult.Succeeded ? Ok(packages) : StatusCode(packagesResult.StatusCode, packagesResult.ErrorMessage);
        }


        // CREATE
        [HttpPost]
        [Authorize(Policy = "UserOnly")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Package))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(CreatePackageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _packageService.CreatePackageAsync(request);
            return result.Succeeded ? Created((string?)null, result.Data) : StatusCode(result.StatusCode, result.ErrorMessage);
        }


        // UPDATE
        [HttpPut]
        [Authorize(Policy = "UserOnly")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Package))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(UpdatePackageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _packageService.UpdatePackageAsync(request);
            return result.Succeeded ? Ok(result.Data) : StatusCode(result.StatusCode, result.ErrorMessage);
        }


        // DELETE
        [HttpDelete("{id}")]
        [Authorize(Policy = "UserOnly")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _packageService.DeletePackageAsync(id);
            return result.Succeeded ? Ok($"Package with id {id} successfully deleted.") : StatusCode(result.StatusCode, result.ErrorMessage);
        }
    }
}
