using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace Server
{
    public class Library : ILibrary
    {
        private List<Book> books = new List<Book>();

        public Library()
        {
            books = LoadBooksFromFile("C:\\Users\\PC\\source\\repos\\Wcf\\Wcf.Server\\books.txt");
            Console.WriteLine($"{books.Count} books loaded successfully");
        }

        private List<Book> LoadBooksFromFile(string filePath)
        {
            var books = new List<Book>();
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                // Split first on " - " to separate title and author
                var titleAuthor = line.Split(new string[] { " - " }, StringSplitOptions.None);
                if (titleAuthor.Length == 2)
                {
                    // Split the first part to separate the ID and the title using ". " as the delimiter
                    var idTitle = titleAuthor[0].Split(new string[] { ". " }, StringSplitOptions.RemoveEmptyEntries);
                    if (idTitle.Length == 2)
                    {
                        int id = int.Parse(idTitle[0]);  // Assuming ID parsing always succeeds; consider try-catch for safety
                        string title = idTitle[1];
                        string author = titleAuthor[1];

                        var authorParts = author.Split(' ');
                        string firstName = string.Join(" ", authorParts.Take(authorParts.Length - 1)); // All but last as first name
                        string lastName = authorParts.Last();

                        books.Add(new Book
                        {
                            id = id,
                            Title = title,
                            Author = new List<Author> { new Author { FirstName = firstName, LastName = lastName } }
                        });

                    }
                }
            }
            
            return books;
        }

        public List<Book> GetAllBooks()
        {
            return books;
        }

        public Book GetBookDetails(int bookId)
        {
            return books.FirstOrDefault(b => b.id == bookId);
        }

        // Search for books by title or author name
        public List<int> SearchBooksByTitle(string keyword)
        {
            keyword = keyword?.ToLower() ?? string.Empty;
            return books.Where(b => TitleContainsKeyword(b, keyword) || AuthorContainsKeyword(b, keyword))
                        .Select(b => b.id)
                        .ToList();
        }

        // Check if book title contains the keyword
        private bool TitleContainsKeyword(Book book, string keyword)
        {
            return book.Title.ToLower().Contains(keyword);
        }

        // Check if any author's full name contains the keyword
        private bool AuthorContainsKeyword(Book book, string keyword)
        {
            return book.Author.Any(author =>
                (author.FirstName.ToLower() + " " + author.LastName.ToLower()).Contains(keyword) ||
                author.FirstName.ToLower().Contains(keyword) ||
                author.LastName.ToLower().Contains(keyword));
        }
    }
}