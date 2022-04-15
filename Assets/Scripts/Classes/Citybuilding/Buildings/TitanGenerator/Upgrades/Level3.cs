namespace Classes.Citybuilding.Buildings.TitanGenerator.Upgrades
{
    public class Level3 : GeneratorBuildingUpgrade
    {
        public Level3()
        {
            Level = 3;
            ModelName = "TitanGenerator3";
            Output = new Resources
            {
                Titan = 2
            };
            BaseCost = new Resources
            {
                Water = 40,
                Energy = 50,
                Food = 20
            };
        }
    }
}