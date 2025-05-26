namespace Domain.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Capacity { get; }

        public Vehicle(int id, string name, double capacity)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Capacity = capacity > 0 ? capacity : throw new ArgumentException("A capacidade deve ser maior que zero.");
        }
    }
}