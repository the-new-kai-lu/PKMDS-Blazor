using PKHeX.Core;
using Pkmds.Core.Extensions;
using Pkmds.Core.Utilities;
using Xunit;

namespace Pkmds.Tests;

public class HGEngineSpeciesTests
{
    [Theory]
    [InlineData((ushort)1076, "Voltuff", 45, 3)]
    [InlineData((ushort)1078, "Raijinque", 85, 5)]
    [InlineData((ushort)1083, "Fimbulisk", 110, 5)]
    [InlineData((ushort)1086, "Ragnaroc", 100, 5)]
    public void CustomSpeciesAreEditable(ushort species, string name, int hp, int growth)
    {
        Assert.True(species.IsValidSpecies());
        Assert.Equal(name, GameInfo.Strings.specieslist[species]);
        var pk = new PK4 { Species = species };
        Assert.Equal(hp, pk.PersonalInfo.HP);
        Assert.Equal(growth, pk.PersonalInfo.EXPGrowth);
    }

    [Fact]
    public void GapAndOutOfRangeAreNotCustomSpecies()
    {
        Assert.False(((ushort)1075).IsValidSpecies());
        Assert.False(((ushort)1087).IsValidSpecies());
    }
}
