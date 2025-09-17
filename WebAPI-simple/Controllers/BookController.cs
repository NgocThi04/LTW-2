using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public BookController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            
            var allBooksDomain = _dbContext.Books;
            
            var allBooksDTO = allBooksDomain.Select(Books => new BookWithAuthorAndPublisherDTO
            {
                Id = Books.Id,
                Title = Books.Title,
                Description = Books.Description,
                IsRead = Books.IsRead,
                DateRead = Books.IsRead ? Books.DateRead.Value : null,
                Rate = Books.IsRead ? Books.Rate.Value : null,
                Genre = Books.Genre,
                CoverUrl = Books.CoverUrl,
                PublisherName = Books.Publisher.FullName,
                AuthorNames = Books.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).ToList();

            return Ok(allBooksDTO);

        }

        [HttpGet("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _dbContext.Books
                .Where(n => n.Id == id)
                .Select(book => new BookWithAuthorAndPublisherDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead ? book.DateRead : null,
                    Rate = book.IsRead ? book.Rate : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    PublisherName = book.Publisher.FullName,
                    AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
                })
                .FirstOrDefault();

            if (bookWithIdDTO == null)
            {
                return NotFound();
            }

            return Ok(bookWithIdDTO);
        }

        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            var bookDomainModel = new Books
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.IsRead ? addBookRequestDTO.DateRead : null,
                Rate = addBookRequestDTO.IsRead ? addBookRequestDTO.Rate : null,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherID = addBookRequestDTO.PublisherId,
            };

            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var bookAuthors = addBookRequestDTO.AuthorIds.Select(authorId => new Book_Author
                {
                    BookId = bookDomainModel.Id,
                    AuthorId = authorId
                }).ToList();
                _dbContext.Book_Authors.AddRange(bookAuthors);
                _dbContext.SaveChanges();
            }
            return Ok();
        }

        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(x => x.Id == id);
            if (bookDomain == null)
                return NotFound(new { Message = $"Book with Id={id} not found." });

            if(bookDomain != null)
            {
                bookDomain.Title = bookDTO.Title;
                bookDomain.Description = bookDTO.Description;
                bookDomain.IsRead = bookDTO.IsRead;
                bookDomain.DateRead = bookDTO.IsRead ? bookDTO.DateRead : null;
                bookDomain.Rate = bookDTO.IsRead ? bookDTO.Rate : null;
                bookDomain.Genre = bookDTO.Genre;
                bookDomain.CoverUrl = bookDTO.CoverUrl;
                bookDomain.PublisherID = bookDTO.PublisherId;
                _dbContext.SaveChanges();
            }

            var authorDomain = _dbContext.Book_Authors.Where(a => a.BookId == id).ToList();
            if (authorDomain != null)
            {
                _dbContext.Book_Authors.RemoveRange(authorDomain);
                _dbContext.SaveChanges();
            }

            foreach (var authorid in bookDTO.AuthorIds)
            {
                var _book_author = new Book_Author()
                {
                    BookId = id,
                    AuthorId = authorid
                };
                _dbContext.Book_Authors.Add(_book_author);
                _dbContext.SaveChanges() ;
            }

            return Ok(bookDTO);
          
        }


        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
          
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);

            if (bookDomain != null)
            {
                _dbContext.Books.Remove(bookDomain);   
                _dbContext.SaveChanges();             
            }
            return Ok();
        }


    }

}
