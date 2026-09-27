using PKHeX.Core;
using Pkmds.Core.Utilities;
using Xunit;

namespace Pkmds.Tests;

public class LocalPokemonSpritesTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryCustomSpeciesUsesBundledArtInEveryStyle(bool shiny)
    {
        for (ushort species = 1076; species <= 1086; species++)
        {
            var relative = LocalPokemonSprites.GetRelativePath(species, shiny);
            Assert.NotNull(relative);
            var expected = SpriteSource.BundledBaseUrl + relative;
            Assert.Equal(expected, PokeApiSpriteUrls.GetPokeApiHomeSpriteUrl(species, isShiny: shiny));
            Assert.Equal(expected, PokeApiSpriteUrls.GetPokeApiVersionSpriteUrl(species,
                isShiny: shiny, version: GameVersion.HG));
            var pokemon = new PK4 { Species = species, PID = shiny ? 0u : 0x10000000u, ID32 = 0 };
            Assert.Equal(relative, SpritePaths.GetPokemonSprite(pokemon));
            Assert.Equal(LocalPokemonSprites.GetRelativePath(species),
                SpritePaths.GetPokemonSpriteForForm(species, EntityContext.Gen4, 0));
        }
    }

    [Fact]
    public void RetailSpeciesKeepTheirExistingSources()
    {
        Assert.Null(LocalPokemonSprites.GetRelativePath(25));
        Assert.Equal("a/a_25.png", SpritePaths.GetPokemonSprite(new PK4 { Species = 25 }));
        Assert.Equal(SpriteSource.HomeBaseUrl + "25.png", PokeApiSpriteUrls.GetPokeApiHomeSpriteUrl(25));
        Assert.Contains("heartgold-soulsilver/25.png",
            PokeApiSpriteUrls.GetPokeApiVersionSpriteUrl(25, version: GameVersion.HG));
        Assert.Equal(SpritePaths.PokemonFallbackFile, SpritePaths.GetPokemonSprite(null));
        Assert.Null(LocalPokemonSprites.GetRelativePath(1075));
        Assert.Null(LocalPokemonSprites.GetRelativePath(1087));
    }
}
