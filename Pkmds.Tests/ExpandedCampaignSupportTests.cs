using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Pkmds.Rcl.Components.Dialogs;
using static System.Buffers.Binary.BinaryPrimitives;

namespace Pkmds.Tests;

public class ExpandedCampaignSupportTests
{
    // Deliberately synthetic. This is not a game-created save or runtime evidence.
    private static SAV4HGSS SyntheticCampaignSave()
    {
        var data = new byte[0x80000];
        data.AsSpan().Fill(0xFF);
        var general = data.AsSpan(0, 0xFE28);
        var storage = data.AsSpan(0xFF00, 0x12310);
        general.Clear();
        storage.Clear();
        var campaign = general.Slice(0xF614, 0x800);
        "EHGSCAMP"u8.CopyTo(campaign);
        WriteUInt16LittleEndian(campaign[8..], 1);
        WriteUInt16LittleEndian(campaign[10..], 0x800);
        WriteUInt16LittleEndian(campaign[12..], 0x20);
        campaign[0x6F8..].Fill(0xA7);
        WriteFooter(general, 0);
        WriteFooter(storage, 1);
        var save = new SAV4HGSS(data)
        {
            Version = GameVersion.HG,
            Language = 2,
            OT = "SYNTHETIC",
        };
        return new SAV4HGSS(save.Write().ToArray());
    }

    private static void WriteFooter(Span<byte> block, ushort id)
    {
        WriteUInt32LittleEndian(block[^16..], 1);
        WriteUInt32LittleEndian(block[^12..], (uint)block.Length);
        WriteUInt32LittleEndian(block[^8..], 0x20060623);
        WriteUInt16LittleEndian(block[^4..], id);
        WriteUInt16LittleEndian(block[^2..], Checksums.CRC16_CCITT(block[..^16]));
    }

    [Fact]
    public void LoaderAndOrdinaryEditPreserveCampaignData()
    {
        var original = SyntheticCampaignSave();
        var before = original.CampaignData.ToArray();
        SaveFileLoader.TryLoad(original.Write().ToArray(), "SYNTHETIC-campaign.sav",
            out var loaded, out var archive).Should().BeTrue();
        archive.Should().BeNull();
        var save = loaded.Should().BeOfType<SAV4HGSS>().Subject;
        save.IsExpandedCampaign.Should().BeTrue();
        save.IsHGEngine.Should().BeFalse();
        save.Money = 1234;

        SaveFileLoader.TryLoad(save.Write().ToArray(), "SYNTHETIC-campaign.sav",
            out var reopened, out _).Should().BeTrue();
        var result = reopened.Should().BeOfType<SAV4HGSS>().Subject;
        result.Money.Should().Be(1234);
        result.CampaignData.ToArray().Should().Equal(before);
    }

    [Fact]
    public async Task SaveInfoIdentifiesTheExpandedFormat()
    {
        var save = SyntheticCampaignSave();
        var state = new TestAppState { SaveFile = save };
        var refresh = new TestRefreshService();
        var service = new AppService(state, refresh, new LegalizationService(state));
        using var context = BunitTestHelpers.CreateBunitContext(state, refresh, service);

        var provider = context.Render<MudDialogProvider>();
        var dialogs = context.Services.GetRequiredService<IDialogService>();
        var parameters = new DialogParameters { [nameof(SaveFileInfoDialog.SaveFile)] = save };
        await provider.InvokeAsync(async () =>
            await dialogs.ShowAsync<SaveFileInfoDialog>("Save File Info", parameters));

        provider.WaitForAssertion(() =>
        {
            provider.Markup.Should().Contain("Campaign Format");
            provider.Markup.Should().Contain("Expanded HGSS");
            provider.Markup.Should().Contain("matching game/editor forks required");
        });
    }

    [Fact]
    public async Task RetailLegalizationRefusesWithoutChangingThePokemon()
    {
        var save = SyntheticCampaignSave();
        var state = new TestAppState { SaveFile = save };
        var pokemon = new PK4 { Species = 252, Nickname = "SYNTHETIC" };
        pokemon.RefreshChecksum();
        var before = pokemon.Data.ToArray();

        var result = await new LegalizationService(state).LegalizeAsync(
            pokemon, save, ct: Xunit.TestContext.Current.CancellationToken);

        result.Status.Should().Be(LegalizationStatus.Failed);
        pokemon.Data.ToArray().Should().Equal(before);
    }
}
