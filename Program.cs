using System;
using System.IO;
using NLog.Web;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Add Product");
                    Console.WriteLine("6) Edit a specific Product");
                    Console.WriteLine("7) Display Products");
                    Console.WriteLine("8) Display specific Product");
                    Console.WriteLine("9) Edit a specific Category");
                    Console.WriteLine("10) Display all Categories and their active products");
                    Console.WriteLine("11) Display a specific Category and its active products");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NorthwindConsole_32_AMPContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        logger.Info("Categories displayed");
                    }
                    else if (choice == "2")
                    {
                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();
                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            logger.Info("Validation passed");
                            var db = new NorthwindConsole_32_AMPContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");

                                db.AddCategory(category);

                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        var db = new NorthwindConsole_32_AMPContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NorthwindConsole_32_AMPContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                        logger.Info($"All categories displayed without error");
                    }
                    else if (choice == "5")
                    {
                        Product product = new Product();
                        Console.WriteLine("Enter Product Name:");
                        product.ProductName = Console.ReadLine();
                        Console.WriteLine("Enter the Product Supplier ID:");
                        product.SupplierId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Product Category ID:");
                        product.CategoryId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Product Quantity per Unit:");
                        product.QuantityPerUnit = Console.ReadLine();
                        Console.WriteLine("Enter Product Unit Price:");
                        product.UnitPrice = decimal.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Product Units in Stock:");
                        product.UnitsInStock = short.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Product Units on Order:");
                        product.UnitsOnOrder = short.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Product Reorder Level:");
                        product.ReorderLevel = short.Parse(Console.ReadLine());
                        Console.WriteLine("Enter if Product is discontinued or not (true if so, false if not):");
                        product.Discontinued = bool.Parse(Console.ReadLine());
                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            logger.Info("Validation passed");
                            var db = new NorthwindConsole_32_AMPContext();
                            // check for unique name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");

                                db.AddProduct(product);
                                logger.Info($"Added new product, {product.ProductName}");

                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "6")
                    {
                        Product product = new Product();
                        Console.WriteLine("Please enter the id of the product you wish to edit");
                        product.ProductId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Update Product Name:");
                        product.ProductName = Console.ReadLine();
                        Console.WriteLine("Update the Product Supplier ID:");
                        product.SupplierId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Update Product Category ID:");
                        product.CategoryId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Update Product Quantity per Unit:");
                        product.QuantityPerUnit = Console.ReadLine();
                        Console.WriteLine("Update Product Unit Price:");
                        product.UnitPrice = decimal.Parse(Console.ReadLine());
                        Console.WriteLine("Update Product Units in Stock:");
                        product.UnitsInStock = short.Parse(Console.ReadLine());
                        Console.WriteLine("Update Product Units on Order:");
                        product.UnitsOnOrder = short.Parse(Console.ReadLine());
                        Console.WriteLine("Update Product Reorder Level:");
                        product.ReorderLevel = short.Parse(Console.ReadLine());
                        Console.WriteLine("Update if Product is discontinued or not (true if so, false if not):");
                        product.Discontinued = bool.Parse(Console.ReadLine());
                        var db = new NorthwindConsole_32_AMPContext();
                        db.EditProduct(product);
                        logger.Info($"Product {product.ProductName} edited");




                    }
                    else if (choice == "7")
                    {
                        Console.WriteLine("If you wish to see all products, enter 1, if you wish to see only active products enter 2, or if you wish to see only discontinued products enter 3");
                        string userChoice = Console.ReadLine();
                        if (userChoice == "1")
                        {
                            var db = new NorthwindConsole_32_AMPContext();
                            var query = db.Products.OrderBy(p => p.ProductName);
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductName}");
                            }
                            logger.Info($"All products displayed");
                        }
                        if (userChoice == "2")
                        {
                            var db = new NorthwindConsole_32_AMPContext();
                            var query = db.Products.OrderBy(p => p.ProductName);
                            foreach (var item in query)
                            {
                                if (item.Discontinued == false)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"{item.ProductName}");
                                }
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"All active products displayed");
                        }
                        if (userChoice == "3")
                        {
                            var db = new NorthwindConsole_32_AMPContext();
                            var query = db.Products.OrderBy(p => p.ProductName);
                            foreach (var item in query)
                            {
                                if (item.Discontinued == true)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"{item.ProductName}");
                                }
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"All Discontinued products displayed");
                        }
                    }
                    if (choice == "8")
                    {
                        Console.WriteLine("Enter the id of the product you wish to see");
                        int userChooser = int.Parse(Console.ReadLine());
                        var db = new NorthwindConsole_32_AMPContext();
                        var query = db.Products.OrderBy(p => p.ProductId);
                        foreach (var item in query)
                        {
                            if (item.ProductId == userChooser)
                            {
                                Console.WriteLine("Product Name:" + item.ProductName);
                                Console.WriteLine("Product Supplier ID:" + item.SupplierId);
                                Console.WriteLine("Product Category ID:" + item.CategoryId);
                                Console.WriteLine("Product Quantity Per Unit:" + item.QuantityPerUnit);
                                Console.WriteLine("Product Unit Price:" + item.UnitPrice);
                                Console.WriteLine("Product Units In Stock:" + item.UnitsInStock);
                                Console.WriteLine("Product Units On Order:" + item.UnitsOnOrder);
                                Console.WriteLine("Product Reorder Level:" + item.ReorderLevel);
                                Console.WriteLine("Product Discontinued?:" + item.Discontinued);
                                logger.Info($"Displayed product: {item.ProductName}");

                            }
                        }
                    }
                    if (choice == "9")
                    {
                        Category category = new Category();
                        Console.WriteLine("Please enter the id of the category you wish to edit");
                        category.CategoryId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Update Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Update Category Description:");
                        category.Description = Console.ReadLine();
                        var db = new NorthwindConsole_32_AMPContext();
                        db.EditCategory(category);
                        logger.Info($"Edited category {category.CategoryName}");

                    }

                    else if (choice == "10")
                    {
                        var db = new NorthwindConsole_32_AMPContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                if (p.Discontinued == false)
                                {
                                    Console.WriteLine($"\t{p.ProductName}");
                                }

                            }
                        }
                        logger.Info($"Displayed all categories and their active products");
                    }
                    else if (choice == "11")
                    {
                        var db = new NorthwindConsole_32_AMPContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose active products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            if (p.Discontinued == false)
                            {
                                Console.WriteLine(p.ProductName);
                            }
                        }
                        logger.Info($"Displayed a category and their active products");
                    }
                    Console.WriteLine();
                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
    }
}
