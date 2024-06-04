﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Server
{
    internal class Program
    {
        static IEnumerable<string> GetIPv4Addresses()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                      .AddressList
                      .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                      .Select(ip => ip.ToString());
        }

        static void Main(string[] args)
        {
            try
            {
                //select port
                Console.WriteLine("Please enter a port number for the server:");
                string port = Console.ReadLine();

                //get IP
                var ipv4Addresses = GetIPv4Addresses().ToList();
                string IP = ipv4Addresses[1]; 
                Console.WriteLine($"port:{port}\nip: {IP}");
                
                //connect
                string serviceAddress = $"net.tcp://{IP}:{port}/Library";
                var baseAddress = new Uri(serviceAddress);
                using (var host = new ServiceHost(typeof(Library), baseAddress))
                {
                    var netTcpBinding = new NetTcpBinding(SecurityMode.None);
                    host.AddServiceEndpoint(typeof(ILibrary), netTcpBinding, "");

                    var metadataBehavior = new ServiceMetadataBehavior { HttpGetEnabled = false };
                    host.Description.Behaviors.Add(metadataBehavior);
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

                    host.Open();
                    Console.WriteLine($"Server started at {serviceAddress}\n\nAll books disponible:");

                    // Retrieve and display all books
                    Library library = new Library();
                    var books = library.GetAllBooks();
                    foreach (var book in books)
                    {
                        string authors = string.Join(", ", book.Author.Select(a => $"{a.FirstName} {a.LastName}"));
                        Console.WriteLine($"Book ID: {book.id}\n\tTitle: {book.Title}\n\tAuthor: {authors}\n");
                    }

                    Console.WriteLine("Press any key to shut down the server...");
                    Console.ReadLine();
                    host.Close();
                }
            }
            catch (FormatException fe)
            {
                Console.WriteLine("Error: Invalid input. Please ensure you enter numeric values where expected.");
                Console.WriteLine(fe.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("An unexpected error occurred.");
                Console.WriteLine(e.Message);
            }
        }
    }
}