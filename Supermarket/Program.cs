using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

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
            CashDesk cashDesk = new CashDesk();

            while (isWork)
            {
                Console.WriteLine($"Супермаркет заработал {supermarket.Money} .");
                Console.WriteLine($"Нажмите {CommandStart}, чтобы обслужить следующего клиента");
                Console.WriteLine($"Нажмите {CommandStop}, чтобы завершить работу");
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandStart:
                        Start(supermarket,cashDesk);
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

        private static void Start(Supermarket supermarket, CashDesk cashDesk)
        {
            Client client = new Client();
            client.SelectProduct(supermarket);
            cashDesk.CalculateClient(client);
            Console.WriteLine($"Супермаркет заработал {client.GetCartCost()} !");
            supermarket.GetCashGeskMoney(client);
        }
    }

    class Supermarket
    {
        private List<Product> _assortment = new List<Product>();

        public int Money { get; protected set; }

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

        public Product GetRandomProduct(Random product)
        {
            int minProductIndex = 0;
            int maxProductIndex = _assortment.Count;
            return _assortment[product.Next(minProductIndex, maxProductIndex)];
        }

        public int GetCashGeskMoney(Client client)
        {
           return Money += client.GetCartCost();
        }
    }

    class CashDesk
    {
        private Queue<Client> _clients = new Queue<Client>();

        public int SupermarketIncome { get; private set; }

        public void CalculateClient(Client client)
        {
            Console.WriteLine("Новый клиент пришёл в наш супермаркет!");
            _clients.Enqueue(client);
            Console.WriteLine("У него в корзине следующие товары:");
            client.ShowProductInCart();

            if (SolvencyClient(client))
            {
                Console.WriteLine("У клиента хватило денег для оплаты товаров! Клиент уходит.");
                _clients.Dequeue();
            }
            else
            {
                Console.WriteLine($"У клиента {client.Money} денег, а товары в его корзине стоят {client.GetCartCost()} !");
                client.ThrowOutRandomProduct();
            }
        }

        private bool SolvencyClient(Client client)
        {
            return client.Money > client.GetCartCost();
        }
    }

    class Client
    {
        private List<Product> _shoppingCart = new List<Product>();

        public int Money { get; private set; }

        public Client()
        {
            Random randomQuantityMoney = new Random();
            int minQuantityMoney = 300;
            int maxQuantityMoney = 1500;
            Money = randomQuantityMoney.Next(minQuantityMoney, maxQuantityMoney);
        }

        public void SelectProduct(Supermarket assortment)
        {
            Random randomProduct = new Random();
            int shopingCartCapacity = 5;

            for (int i = 0; i <= shopingCartCapacity; i++)
            {
                _shoppingCart.Add(assortment.GetRandomProduct(randomProduct));
            }
        }

        public int GetShoppingCartCapasity()
        {
            return _shoppingCart.Count();
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
            Random randomProduct = new Random();

            while (Money < GetCartCost())
            {
                Console.WriteLine("Отменяем случайный товар из корзины клиента...");
                int minProductIndex = 0;
                int maxProductIndex = _shoppingCart.Count;
                _shoppingCart.Remove(_shoppingCart[randomProduct.Next(minProductIndex, maxProductIndex)]);
                ShowProductInCart();
                Console.ReadKey();
                Console.Clear();
            }
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
        public string Name { get; private set; }
        public int Cost { get; private set; }

        public Product(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Товар {Name} стоимостью - {Cost} .");
        }
    }
}
