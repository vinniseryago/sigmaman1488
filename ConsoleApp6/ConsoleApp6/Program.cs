using System;
using System.Collections.Generic;

public class Order
{
    public int OrderId { get; private set; }
    public string CustomerName { get; private set; }
    public List<string> Items { get; private set; }
    public decimal TotalCost { get; private set; }

    public Order(int orderId, string customerName)
    {
        OrderId = orderId;
        CustomerName = customerName;
        Items = new List<string>();
        TotalCost = 0;
    }

    public void AddItem(string item, decimal price)
    {
        Items.Add(item);
        TotalCost += price;
    }
}

public class OrderQueue
{
    private Queue<Order> orders;
    private Stack<Order> completedOrders;
    private Dictionary<int, string> orderStatuses;

    public OrderQueue()
    {
        orders = new Queue<Order>();
        completedOrders = new Stack<Order>();
        orderStatuses = new Dictionary<int, string>();
    }

    public void AddOrder(Order order)
    {
        orders.Enqueue(order);
        orderStatuses[order.OrderId] = "новый";
    }

    public Order ProcessNextOrder()
    {
        if (orders.Count > 0)
        {
            Order order = orders.Dequeue();
            orderStatuses[order.OrderId] = "обрабатывается";
            return order;
        }
        return null;
    }

    public void CompleteOrder(Order order)
    {
        orderStatuses[order.OrderId] = "завершён";
        completedOrders.Push(order);
    }

    public string GetOrderStatus(int orderId)
    {
        return orderStatuses.TryGetValue(orderId, out var status) ? status : "заказ не найден";
    }
}

public class ProductCatalog
{
    private Dictionary<int, (string Name, decimal Price, int Stock)> products;

    public ProductCatalog()
    {
        products = new Dictionary<int, (string, decimal, int)>();
    }

    public void AddProduct(int id, string name, decimal price, int stock)
    {
        products[id] = (name, price, stock);
    }

    public void RemoveProduct(int id)
    {
        products.Remove(id);
    }

    public void UpdateStock(int id, int quantity)
    {
        if (products.ContainsKey(id))
        {
            var product = products[id];
            products[id] = (product.Name, product.Price, product.Stock + quantity);
        }
    }

    public bool TryGetProduct(int id, out (string Name, decimal Price, int Stock) product)
    {
        return products.TryGetValue(id, out product);
    }
}

// Пример использования
class Program
{
    static void Main(string[] args)
    {
        ProductCatalog catalog = new ProductCatalog();
        catalog.AddProduct(1, "Товар A", 100m, 10);
        catalog.AddProduct(2, "Товар B", 200m, 5);

        OrderQueue orderQueue = new OrderQueue();

        // Создание нового заказа
        Order order1 = new Order(1, "Клиент 1");
        if (catalog.TryGetProduct(1, out var product))
        {
            order1.AddItem(product.Name, product.Price);
            catalog.UpdateStock(1, -1); // Уменьшаем количество на складе
        }

        orderQueue.AddOrder(order1);

        // Обработка заказа
        var processedOrder = orderQueue.ProcessNextOrder();
        if (processedOrder != null)
        {
            Console.WriteLine($"Обрабатывается заказ ID: {processedOrder.OrderId}, Клиент: {processedOrder.CustomerName}");
            orderQueue.CompleteOrder(processedOrder);
            Console.WriteLine($"Статус заказа ID {processedOrder.OrderId}: {orderQueue.GetOrderStatus(processedOrder.OrderId)}");
        }
    }
}
