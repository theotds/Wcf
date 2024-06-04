using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace WCF.Client
{
    internal class Program
    {
        [ServiceContract]
        public interface ILibraryService
        {
            [OperationContract]
            List<int> SearchBooksByTitle(string title);

            [OperationContract]
            List<int> SearchBooksByAuthor(string author);

            [OperationContract]
            Book GetBookDetails(int bookId);
        }

        static void Main(string[] args)
        {
            // Getting the address and port for the service
            Console.WriteLine("Enter the IP address of the service (e.g., localhost):");
            string ipAddress = Console.ReadLine();
            Console.WriteLine("Enter the port number for the service (e.g., 6565):");
            string port = Console.ReadLine();
            string serviceUri = $"net.tcp://{ipAddress}:{port}/Library";

            // Setting up the connection
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            var channelFactory = new ChannelFactory<ILibrary>(binding, new EndpointAddress(serviceUri));
            var serviceProxy = channelFactory.CreateChannel();

            while (true)
            {
                // User chooses search type or exits
                Console.WriteLine("Choose your search option (or type '0' to exit):");
                Console.WriteLine("1 - Search by book title");
                Console.WriteLine("2 - Search by author's name");
                string option = Console.ReadLine();

                if (option == "0")
                {
                    Console.WriteLine("Exiting client...");
                    break;  // Break out of the loop to exit the program
                }

                List<int> bookIds;

                if (option == "1")
                {
                    Console.WriteLine("Enter the book title to search:");
                    string title = Console.ReadLine();
                    bookIds = serviceProxy?.SearchBooksByTitle(title);
                }
                else if (option == "2")
                {
                    Console.WriteLine("Enter the author's name to search:");
                    string author = Console.ReadLine();
                    bookIds = serviceProxy?.SearchBooksByTitle(author);
                }
                else
                {
                    Console.WriteLine("Invalid option selected. Please choose a valid option.");
                    continue;  // Skip the rest of the loop and prompt again
                }

                // Retrieve and display book details for each ID
                if (bookIds != null && bookIds.Count > 0)
                {
                    Console.WriteLine($"Found {bookIds.Count} matching books:\n");
                    foreach (int bookId in bookIds)
                    {
                        Book book = serviceProxy.GetBookDetails(bookId);
                        if (book != null)
                        {
                            string authors = string.Join(", ", book.Author.Select(a => $"{a.FirstName} {a.LastName}"));
                            Console.WriteLine($"Book ID: {book.id}\n\tTitle: {book.Title}\n\tAuthor: {authors}\n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No books found matching your search criteria.");
                }
            }
        }
    }
}
