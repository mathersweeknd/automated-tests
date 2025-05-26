using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDeliveryRepository
    {
        void Add(Delivery delivery);
        Delivery? Get(int id);
        bool RemoveById(int id);
        bool Update(Delivery delivery);
        Delivery? GetByDeliveryTimeAndVehicle(DateTime deliveryTime, Vehicle vehicle);
        List<Delivery> GetByRouteDelivery(DateTime deliveryTime, Vehicle vehicle);
    }
}