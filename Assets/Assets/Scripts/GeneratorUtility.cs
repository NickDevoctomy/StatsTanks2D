using System.Collections.Generic;

public static class GeneratorUtility
{
    public static List<IGenerator> CreateGenerators()
    {
        return new List<IGenerator>
        {
            new WallGenerator(),
            new SpawnGenerator()
        };
    }
}
