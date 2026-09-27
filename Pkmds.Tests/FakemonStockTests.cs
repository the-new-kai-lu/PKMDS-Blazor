using PKHeX.Core;
using Pkmds.Core.Extensions;
using Pkmds.Core.Utilities;
using Pkmds.Rcl.Services;
using Xunit;

namespace Pkmds.Tests;

public class FakemonStockTests
{
    private static SAV4HGSS CreateSave()
    {
        var sav = new SAV4HGSS();
        sav.General[FakemonStockProfile.MarkerOffset] = 0x46;
        sav.General[FakemonStockProfile.MarkerOffset + 1] = 0x4B;
        sav.General[FakemonStockProfile.VersionOffset] = 1;
        return sav;
    }

    [Theory]
    [InlineData((ushort)468, "Wild Charge", 15, MoveType.Electric, GameInfoUtilities.MoveCategory.Physical)]
    [InlineData((ushort)469, "Snarl", 15, MoveType.Dark, GameInfoUtilities.MoveCategory.Special)]
    [InlineData((ushort)470, "Incinerate", 15, MoveType.Fire, GameInfoUtilities.MoveCategory.Special)]
    [InlineData((ushort)471, "Fire Lash", 15, MoveType.Fire, GameInfoUtilities.MoveCategory.Physical)]
    [InlineData((ushort)472, "Icicle Crash", 10, MoveType.Ice, GameInfoUtilities.MoveCategory.Physical)]
    [InlineData((ushort)473, "Bulldoze", 20, MoveType.Ground, GameInfoUtilities.MoveCategory.Physical)]
    [InlineData((ushort)474, "Hurricane", 10, MoveType.Flying, GameInfoUtilities.MoveCategory.Special)]
    public void AddedMovesShowCorrectNamesTypesCategoriesAndPp(ushort move, string name, int pp,
        MoveType type, GameInfoUtilities.MoveCategory category)
    {
        var sav = CreateSave();
        var pk = sav.BlankPKM;
        pk.Species = 1078;
        pk.Move1 = move;
        pk.Move1_PPUps = 3;
        var sources = new FilteredGameDataSource(sav, GameInfo.Sources);
        Assert.Equal(name, sources.Moves.Single(x => x.Value == move).Text);
        Assert.Equal((byte)type, MoveInfo.GetType(move, sav));
        Assert.Equal(category, GameInfoUtilities.GetMoveCategory(move, sav));
        Assert.Equal(pp * 8 / 5, pk.GetMaxPP(0));
    }

    [Fact]
    public void RetailMoveSuggestionLeavesCustomMovesUntouched()
    {
        var pk = CreateSave().BlankPKM;
        pk.Species = 1078;
        pk.Move1 = 468;
        var outcome = new LegalityFixService(null!).SuggestMoves(pk);
        Assert.False(outcome.Changed);
        Assert.Equal(468, pk.Move1);
    }

    [Fact]
    public void StockProfileKeepsRetailPersonalDataAndSparseSpecies()
    {
        var sav = CreateSave();
        Assert.Equal(PersonalTable.HGSS[25].Write(), sav.Personal[25].Write());
        Assert.Equal((byte)MoveType.Electric, sav.Personal[1076].Type1);
        var species = new FilteredGameDataSource(sav, GameInfo.Sources).Species;
        Assert.Contains(species, x => x.Value == 1076);
        Assert.DoesNotContain(species, x => x.Value is >= 494 and < 1076);
    }
}
