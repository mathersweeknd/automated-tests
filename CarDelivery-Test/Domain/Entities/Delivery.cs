namespace Domain.Entities
{
    public class Delivery
    {
        public int Id { get; set; }
        public Priority Priority { get; set; } = Priority.Normal;
        public string ClientName { get; set; }
        public DateTime DeliveryTime { get; set; }
        public Vehicle Vehicle { get; set; }

        public Delivery(int id, string clientName, DateTime deliveryTime, Vehicle vehicle, Priority priority = Priority.Normal)
        {
            Id = id;
            ClientName = clientName ?? throw new ArgumentNullException(nameof(clientName));
            DeliveryTime = deliveryTime;
            Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
            Priority = priority;
        }
    }

    public enum Priority
    {
        Normal,
        High
    }
}