using System;
using System.Collections.Generic;
using System.Linq;

namespace Supermarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string CommandStart = "1";
            const string CommandStop = "2";

            bool isWork = true;
            Supermarket supermarket = new Supermarket();
            supermarket.CreateAssortment();
            supermarket.FormQueueClients();

            while (isWork)
            {
                Console.WriteLine($"Супермаркет заработал {supermarket.Money} .");
                Console.WriteLine($"Нажмите {CommandStart}, чтобы обслужить следующего клиента");
                Console.WriteLine($"Нажмите {CommandStop}, чтобы завершить работу");
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandStart:
                        supermarket.Work();
                        break;

                    case CommandStop:
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("Неккоректный ввод!");
                        break;
                }

                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    class UserUtils
    {
        private static Random _random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
    }

    class Supermarket
    {
        private List<Product> _assortment = new List<Product>();
        private Queue<Client> _clients = new Queue<Client>();

        public int Money { get; private set; }

        public void CreateAssortment()
        {
            _assortment.Add(new Product("хлеб", 40));
            _assortment.Add(new Product("молоко", 60));
            _assortment.Add(new Product("мясо", 150));
            _assortment.Add(new Product("рыба", 200));
            _assortment.Add(new Product("тушенка", 170));
            _assortment.Add(new Product("сосиски", 190));
            _assortment.Add(new Product("шампунь", 230));
            _assortment.Add(new Product("туалетная бумага", 130));
            _assortment.Add(new Product("шоколадка", 70));
        }

        public void Work()
        {
            Product[] catalog = new Product[_assortment.Count];
            _assortment.CopyTo(catalog);
            GetClient().SelectProduct(catalog);
            CalculateClient(GetClient());
        }

        public void FormQueueClients()
        {
            int countClients = 10;

            for (int i = 0; i < countClients; i++)
            {
                _clients.Enqueue(new Client());
            }
        }

        private void CalculateClient(Client client)
        {
            Console.WriteLine("Обслуживаем текущего клиента на кассе...");
            Console.WriteLine("У него в корзине следующие товары:");
            client.ShowProductInCart();

            if (client.CanPay())
            {
                Console.WriteLine($"У клиента {client.Money} денег, а товары в его корзине стоят {client.GetCartCost()} !");
                Console.WriteLine("У клиента хватило денег для оплаты товаров! Клиент уходит.");
                GetCashGeskMoney(client);
                Console.WriteLine($"Супермаркет заработал {client.GetCartCost()} !");
                _clients.Dequeue();
            }
            else
            {
                Console.WriteLine($"У клиента {client.Money} денег, а товары в его корзине стоят {client.GetCartCost()} !");

                while (client.Money < client.GetCartCost())
                {
                    client.ThrowOutRandomProduct();
                }

                Console.WriteLine($"У клиента {client.Money} денег, а оставшиеся товары в его корзине стоят {client.GetCartCost()}.");
                Console.WriteLine($"Клиент оплачивает оставшиеся товары и уходит !");
                GetCashGeskMoney(client);
                Console.WriteLine($"Супермаркет заработал {client.GetCartCost()} !");
                _clients.Dequeue();
            }

            Console.WriteLine($"В очереди осталось {_clients.Count} клиентов.");
        }

        private Client GetClient()
        {
            if (_clients.Count == 0)
            {
                Console.WriteLine("Новый клиент пришёл в наш супермаркет!");
                _clients.Enqueue(new Client());
            }

            return _clients.First();
        }

        private int GetCashGeskMoney(Client client)
        {
            if (client.GetCartCost() == 0)
            {
                Console.WriteLine("Клиенту не хватило денег на оплату товаров, отменили все товары в его корзине, клиент уходит!");
                return 0;
            }
            else
            {
                return Money += client.GetCartCost();
            }
        }
    }

    class Client
    {
        private List<Product> _shoppingCart = new List<Product>();

        public Client()
        {
            int minQuantityMoney = 300;
            int maxQuantityMoney = 1500;
            Money = UserUtils.GenerateRandomNumber(minQuantityMoney, maxQuantityMoney);
        }

        public int Money { get; private set; }

        public void SelectProduct(Product[] catalog)
        {
            int shopingCartCapacity = 5;
            int assortmentMinIndex = 0;

            for (int i = 0; i <= shopingCartCapacity; i++)
            {
                _shoppingCart.Add(catalog[UserUtils.GenerateRandomNumber(assortmentMinIndex, catalog.Length)]);
            }
        }

        public int GetCartCost()
        {
            int totalShoppingCartCost = 0;

            for (int i = 0; i < _shoppingCart.Count; i++)
            {
                totalShoppingCartCost += _shoppingCart[i].Cost;
            }

            return totalShoppingCartCost;
        }

        public void ThrowOutRandomProduct()
        {
            Console.WriteLine("\nОтменяем случайный товар из корзины клиента...");
            int minProductIndex = 0;
            int maxProductIndex = _shoppingCart.Count;
            _shoppingCart.Remove(_shoppingCart[UserUtils.GenerateRandomNumber(minProductIndex, maxProductIndex)]);
            ShowProductInCart();
            Console.ReadKey();
            Console.Clear();
        }

        public bool CanPay()
        {
            return Money > GetCartCost();
        }

        public void ShowProductInCart()
        {
            for (int i = 0; i < _shoppingCart.Count; i++)
            {
                _shoppingCart[i].ShowInfo();
            }
        }
    }

    class Product
    {
        public Product(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }

        public string Name { get; private set; }
        public int Cost { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Товар {Name} стоимостью - {Cost} .");
        }
    }
}
