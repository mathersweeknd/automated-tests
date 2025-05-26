using Domain.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class SchedulerService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private const int Capacity = 10;

        public SchedulerService(IDeliveryRepository deliveryRepository) => _deliveryRepository = deliveryRepository;

        public bool AddDelivery(Delivery delivery)
        {
            if(delivery.Vehicle == null)
                throw new ArgumentNullException(nameof(delivery.Vehicle), "Veículo não selecionado");

            var existingDelivery = _deliveryRepository.GetByDeliveryTimeAndVehicle(delivery.DeliveryTime, delivery.Vehicle);
            if (existingDelivery != null && delivery.Priority <= existingDelivery.Priority)
                throw new Exception("Já existe uma entrega marcada para esse horário.");
            _deliveryRepository.Add(delivery);
            return true;
        }

        public List<Delivery> GetDailyRoutes(Vehicle vehicle, DateTime deliveryTime)
            => _deliveryRepository.GetByRouteDelivery(deliveryTime, vehicle) ?? new List<Delivery>();

        public (int TotalDeliveries, double OccupancyPercentage) GetVehicleOccupancy(Vehicle vehicle, DateTime date)
        {
            var totalDeliveries = GetDailyRoutes(vehicle, date).Count;
            return (totalDeliveries, (double)totalDeliveries / Capacity * 100);
        }
    }
}