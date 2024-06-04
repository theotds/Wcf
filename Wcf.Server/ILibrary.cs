using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Linq;

namespace Server
{
    [ServiceContract(Name = "Library", Namespace = "")]
    public interface ILibrary
    {
        [OperationContract]
        List<int> SearchBooksByTitle(string keyword);

        [OperationContract]
        [FaultContract(typeof(BookNotFoundFault))]
        Book GetBookDetails(int bookId);

        [OperationContract]
        List<Book> GetAllBooks();

    }

    [DataContract(Name = "Author", Namespace = "")]
    public class Author
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }

    [DataContract(Name = "Book", Namespace = "")]
    public class Book
    {
        [DataMember] public int id { get; set; }
        [DataMember] public string Title { get; set; }
        [DataMember] public List<Author> Author { get; set; }
    }

    [DataContract(Name = "BookNotFoundFault", Namespace = "")]
    public class BookNotFoundFault
    {
        [DataMember] public string ErrorMessage { get; set; }
    }
}