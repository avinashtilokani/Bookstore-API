using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bookstore_API.Contracts;
using Bookstore_API.Data;
using Bookstore_API.DTOs;
using Bookstore_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore_API.Controllers
{
    /// <summary>
    /// Interacts with the books table
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public BooksController(IBookRepository bookRepository,
            ILoggerService logger,
            IMapper mapper,
            IWebHostEnvironment env
            )
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
            _env = env;
        }

        private string GetImagePath(string fileName) 
            => ($"{_env.ContentRootPath}\\uploads\\{fileName}");

        /// <summary>
        /// Get all books
        /// </summary>
        /// <returns>A list of books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Atempted Call");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                foreach(var item in response)
                {
                    if(!string.IsNullOrEmpty(item.Image))
                    {
                        if(System.IO.File.Exists(GetImagePath(item.Image)))
                        {
                            byte[] imageBytes = System.IO.File.ReadAllBytes(GetImagePath(item.Image));
                            item.File = Convert.ToBase64String(imageBytes);
                        }
                    }
                }
                _logger.LogInfo($"{location}: Success");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message}-{e.InnerException}");
            }
        }

        /// <summary>
        /// Get book by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Book record</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Atempted call for id: {id}");
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record for id: {id}");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                if(!string.IsNullOrEmpty(response.Image))
                {
                    var imgPath = GetImagePath(response.Image);
                    if(System.IO.File.Exists(imgPath))
                    {
                        byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath);
                        response.File = Convert.ToBase64String(imgBytes);
                    }
                }
                _logger.LogInfo($"{location}: Success in getting record with id: {id}");
                
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message}-{e.InnerException}");
            }
        }

        /// <summary>
        /// Create a book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns>Book object</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Create Attempted");

                if (bookDTO == null)
                {
                    _logger.LogWarn($"{location}: Empty request was submitted");
                    return BadRequest(ModelState);
                }

                if(!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var success = await _bookRepository.Create(book);
                if(!success)
                {
                    return internalError($"{location}: Creation failed");
                }
                if(!string.IsNullOrEmpty(bookDTO.File))
                {
                    var imgPath = GetImagePath(bookDTO.Image);
                    byte[] imgBytes = Convert.FromBase64String(bookDTO.File);
                    System.IO.File.WriteAllBytes(imgPath, imgBytes);
                }
                _logger.LogInfo($"{location}: Creation was successful");
                _logger.LogInfo($"{location}: {book}");
                return Created("Create", new { book });

            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message}-{e.InnerException}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody]BookUpdateDTO bookDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Update attempted on record with id: {id}");

                if(id<1 || bookDTO == null || id != bookDTO.Id)
                {
                    _logger.LogWarn($"{location}: Update failed with bad data - id: {id}");
                    return BadRequest();
                }
                if(!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }
                if(! await _bookRepository.Exists(id))
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                var oldImage = await _bookRepository.GetImageFileName(id);
                var book = _mapper.Map<Book>(bookDTO);
                var success = await _bookRepository.Update(book);

                if (!success)
                {
                    return internalError($"{location}: Update failed for record with id: {id}");
                }

                if (!bookDTO.Image.Equals(oldImage))
                {
                    if(System.IO.File.Exists(GetImagePath(oldImage)))
                    {
                        System.IO.File.Delete(GetImagePath(oldImage));
                    }
                }

                if(!string.IsNullOrEmpty(bookDTO.File))
                {
                    byte[] imageBytes = Convert.FromBase64String(bookDTO.File);
                    System.IO.File.WriteAllBytes(GetImagePath(bookDTO.Image), imageBytes);
                }
                _logger.LogInfo($"{location}: Update successful for record with id: {id}");
                return NoContent();
            }
            catch (Exception e)
            {
                return internalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Removes a book by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Delete attempted on record with id: {id}");
                if(id < 1)
                {
                    _logger.LogWarn($"Delete failed with bad data - id: {id}");
                    return BadRequest();
                }
                if(! await _bookRepository.Exists(id))
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                var book = await _bookRepository.FindById(id);
                
                var success = await _bookRepository.Delete(book);
                if (!success)
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
