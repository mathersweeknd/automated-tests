using Xunit;

namespace Logistics
{
    public class LoadDistributorUnitTest
    {
        // Teste 1: Cálculo de Carga Total
        [Theory]
        [InlineData(600, 1, 300, 2, 900, 3)]
        [InlineData(250, 2, 200, 2, 450, 4)]
        [InlineData(505, 6, 134, 1, 639, 7)]
        public void TestCalculateTotalLoad(int w1, int v1, int w2, int v2, int expectedWeight, int expectedVolume)
        {
            // Arrange
            var loads = new List<Load>
            {
                new Load(w1, v1, Priority.Normal),
                new Load(w2, v2, Priority.Normal)
            };
            var distributor = new LoadDistributor();

            // Act
            var result = distributor.CalculateTotalLoad(loads);

            // Assert
            Assert.Equal(expectedWeight, result.TotalWeight);
            Assert.Equal(expectedVolume, result.TotalVolume);
        }

        // Teste 2: Distribuição por Caminhão
        [Fact]
        public void TestDistributeLoads()
        {
            // Arrange
            var trucks = new List<Truck>
            {
                new Truck { MaxWeight = 3000, MaxVolume = 10 },
                new Truck { MaxWeight = 2000, MaxVolume = 8 }
            };
            var loads = new List<Load>
            {
                new Load(500, 2, Priority.Normal),
                new Load(300, 1, Priority.Normal)
            };
            var distributor = new LoadDistributor();

            // Act
            var (distributedTrucks, unallocatedLoads) = distributor.DistributeLoads(trucks, loads);

            // Assert
            Assert.Single(distributedTrucks[0].Loads);
            Assert.Single(distributedTrucks[1].Loads);
            Assert.Empty(unallocatedLoads);
        }

        // Teste 3: Excedente de Carga
        [Theory]
        [InlineData(1000, 7, 1200, 8, 1)]
        [InlineData(500, 3, 600, 5, 1)]
        [InlineData(1500, 8, 750, 3, 0)]
        public void TestExcessLoad(int truckWeight, int truckVolume, int loadWeight, int loadVolume, int expectedUnallocated)
        {
            // Arrange
            var trucks = new List<Truck>
            {
                new Truck { MaxWeight = truckWeight, MaxVolume = truckVolume }
            };
            var loads = new List<Load>
            {
                new Load(loadWeight, loadVolume, Priority.Normal)
            };
            var distributor = new LoadDistributor();

            // Act
            var (distributedTrucks, unallocatedLoads) = distributor.DistributeLoads(trucks, loads);

            // Assert
            Assert.Equal(expectedUnallocated, unallocatedLoads.Count);
        }

        // Teste 4: Validação de Carga com Peso ou Volume Negativo
        [Fact]
        public void TestInvalidLoad()
        {
            // Arrange
            var invalidLoad = new Load(-100, 2, Priority.Normal);
            var distributor = new LoadDistributor();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => distributor.CalculateTotalLoad(new List<Load> { invalidLoad }));
            Assert.Equal("Peso e volume não podem ser negativos.", exception.Message);
        }

        // Teste 5: Carga Prioritária
        [Fact]
        public void TestPrioritizedLoad()
        {
            // Arrange
            var trucks = new List<Truck>
            {
                new Truck { MaxWeight = 1000, MaxVolume = 5 }
            };
            var loads = new List<Load>
            {
                new Load(500, 2, Priority.High),
                new Load(300, 1, Priority.Normal)
            };
            var distributor = new LoadDistributor();

            // Act
            var (distributedTrucks, unallocatedLoads) = distributor.DistributeLoads(trucks, loads);

            // Assert
            Assert.Equal(loads[0], distributedTrucks[0].Loads[0]);
            Assert.Equal(loads[1], distributedTrucks[0].Loads[1]);
        }
    }
}