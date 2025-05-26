using Application.Services;
using Domain.Interfaces;
using Domain.Entities;
using AutoFixture;
using Moq;
using Xunit;

namespace SchedulerServiceTests
{
    public class CarDeliveryTests
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IDeliveryRepository> _deliveryRepositoryMock = new();
        private readonly SchedulerService _schedulerService;

        public CarDeliveryTests() => _schedulerService = new SchedulerService(_deliveryRepositoryMock.Object);

        private Delivery CreateDelivery(string client, DateTime time, Vehicle vehicle, Priority priority = Priority.Normal)
            => _fixture.Build<Delivery>()
                      .With(d => d.ClientName, client)
                      .With(d => d.DeliveryTime, time)
                      .With(d => d.Vehicle, vehicle)
                      .With(d => d.Priority, priority)
                      .Create();

        private List<Delivery> CreateDeliveries(Vehicle vehicle, DateTime date, int count)
            => _fixture.Build<Delivery>()
                      .With(d => d.DeliveryTime, date)
                      .With(d => d.Vehicle, vehicle)
                      .CreateMany(count)
                      .ToList();

        // 1 - Adicionar entrega
        [Fact]
        public void AddDelivery()
        {
            var vehicle = new Vehicle(1, "Scania Cegonha", 100.0);
            var delivery = CreateDelivery("ClientTest", new DateTime(2025, 01, 31, 17, 30, 0), vehicle);
            _deliveryRepositoryMock.Setup(x => x.Add(delivery));

            _schedulerService.AddDelivery(delivery);

            _deliveryRepositoryMock.Verify(x => x.Add(delivery), Times.Once);
        }

        // 2 - Exceção de entregas no mesmo horário
        [Fact]
        public void SameTimeExceptionDelivery()
        {
            var vehicle = new Vehicle(1, "Scania Cegonha", 100.0);
            var delivery = CreateDelivery("ClientTest", new DateTime(2025, 01, 31, 17, 30, 0), vehicle);
            _deliveryRepositoryMock.Setup(x => x.GetByDeliveryTimeAndVehicle(delivery.DeliveryTime, delivery.Vehicle!)).Returns(delivery);

            Assert.Throws<Exception>(() => _schedulerService.AddDelivery(delivery));
        }

        // 3 - Prioridade de entregas
        [Fact]
        public void PriorityDelivery()
        {
            var vehicle = new Vehicle(1, "Scania Cegonha", 100.0);
            var existingDelivery = CreateDelivery("Existing", new DateTime(2025, 01, 31, 17, 30, 0), vehicle, Priority.Normal);
            var highPriorityDelivery = CreateDelivery("HighPriority", new DateTime(2025, 01, 31, 17, 30, 0), vehicle, Priority.High);

            _deliveryRepositoryMock.Setup(x => x.GetByDeliveryTimeAndVehicle(highPriorityDelivery.DeliveryTime, highPriorityDelivery.Vehicle!))
                .Returns(existingDelivery);
            _deliveryRepositoryMock.Setup(x => x.Add(highPriorityDelivery));

            _schedulerService.AddDelivery(highPriorityDelivery);

            _deliveryRepositoryMock.Verify(x => x.Add(highPriorityDelivery), Times.Once);
            _deliveryRepositoryMock.Verify(x => x.RemoveById(existingDelivery.Id), Times.Never);
        }

        // 4 - Rotas de entregas diárias
        [Fact]
        public void DailyRoutesDelivery()
        {
            var vehicle = new Vehicle(1, "Scania Cegonha", 100.0);
            var date = new DateTime(2025, 01, 31);
            var deliveries = CreateDeliveries(vehicle, date, 3);

            _deliveryRepositoryMock.Setup(x => x.GetByRouteDelivery(date, vehicle)).Returns(deliveries);

            var result = _schedulerService.GetDailyRoutes(vehicle, date);

            Assert.Equal(3, result.Count);
            Assert.All(deliveries, delivery => Assert.Contains(delivery, result));
        }

        // 5 - Ocupação do veículo
        [Fact]
        public void VehicleOccupancyDelivery()
        {
            var vehicle = new Vehicle(1, "Scania Cegonha", 100.0);
            var date = new DateTime(2025, 01, 31);
            var deliveries = CreateDeliveries(vehicle, date, 7);

            _deliveryRepositoryMock.Setup(x => x.GetByRouteDelivery(date, vehicle)).Returns(deliveries);

            var (totalDeliveries, occupancyPercentage) = _schedulerService.GetVehicleOccupancy(vehicle, date);

            Assert.Equal(7, totalDeliveries);
            Assert.Equal(70, occupancyPercentage);
        }
    }
}