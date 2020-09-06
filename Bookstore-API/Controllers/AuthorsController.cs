using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bookstore_API.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bookstore_API.DTOs;
using Microsoft.IdentityModel.Tokens;
using Bookstore_API.Data;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Authorization;

namespace Bookstore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in bookstore's database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Authors
        /// </summary>
        /// <returns>List of all authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location} - Attempted Call");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Get all authors - Success");
                _logger.LogInfo($"{location}: Success");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message}-{e.InnerException}");
            }

        }

        /// <summary>
        /// Get Author by Id
        /// </summary>
        /// <returns>An author record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Atempted Call  for id: {id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record for id: {id}");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{location}: Success in getting record with id: {id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message}-{e.InnerException}");
            }

        }

        /// <summary>
        /// Creates an author
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorCreateDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Create Attempted");

                if (authorCreateDTO == null)
                {
                    _logger.LogWarn($"{location}: Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorCreateDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return internalError($"{location}: Creation failed");
                }
                _logger.LogInfo($"{location}: Creation was successful");
                _logger.LogInfo($"{location}: {author}");
                return Created("Created", new { author });

            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message}-{e.InnerException}");
            }
        }

        /// <summary>
        /// Updates an Author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorUpdateDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Update attempted on record with id: {id}");
                if (id < 1 || authorUpdateDTO == null || id != authorUpdateDTO.Id)
                {
                    _logger.LogWarn($"{location}: Update failed with bad data - id: {id}");
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }
               
                if (! await _authorRepository.Exists(id))
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                var author = _mapper.Map<Author>(authorUpdateDTO);
                var isSuccess = await _authorRepository.Update(author);

                if (!isSuccess)
                {
                    return internalError($"{location}: Update failed for record with id: {id}");
                }

                _logger.LogInfo($"{location}: Update successful for record with id: {id}");
                return NoContent();

            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message}-{e.InnerException}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Delete attempted on record with id: {id}");
                if (id < 1)
                {
                    _logger.LogWarn($"Delete failed with bad data - id: {id}");
                    return BadRequest();
                }
                var author = await _authorRepository.FindById(id);
                if (! await _authorRepository.Exists(id))
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                var isSuccess = await _authorRepository.Delete(author);

                if (!isSuccess)
                {
                    return internalError($"{location}: Delete failed for record with id: {id}");
                }

                _logger.LogInfo($"{location}: Delete successful for record with id: {id}");
                return NoContent();

            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message}-{e.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult internalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact administrator");
        }
    }
}
