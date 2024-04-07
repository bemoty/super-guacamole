using System.Text;
using System.Text.Json;
using super.guacamole.image.Model;

namespace super.guacamole.image;

public class SkinProvider : IProvider<Guid, string>
{
    private List<string> _defaultSkins =
    [
        "https://s.namemc.com/i/015b8af09770d2d7.png", // Ari
        "https://s.namemc.com/i/55d873354554d952.png", // Zuri
        "https://s.namemc.com/i/069f740e11523d1d.png", // Sunny
        "https://s.namemc.com/i/74a241cff2ecf0fa.png", // Kai
        "https://s.namemc.com/i/b8c61cf07b087b66.png", // Efe
        "https://s.namemc.com/i/64cf8da7dac26f7c.png", // Makena
        "https://s.namemc.com/i/f5e7b3b473ca8846.png" // Noor
    ];

    private string Fallback()
    {
        return _defaultSkins[new Random().Next(_defaultSkins.Count)];
    }

    private async Task<ProfileResponse?> GetProfile(Guid key)
    {
        var client = new HttpClient();
        var response =
            await client.GetAsync($"https://sessionserver.mojang.com/session/minecraft/profile/{key.ToString()}");
        response.EnsureSuccessStatusCode(); // todo: add fallback profile response
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ProfileResponse>(responseBody); // todo: invalid json exception
    }

    private async Task<TextureResponse?> GetTexture(Guid key)
    {
        var profile = await GetProfile(key);
        var textureValue = profile?.properties.FirstOrDefault(p => p.name == "textures")?.value;
        if (textureValue == null) return null;
        var texture = Encoding.UTF8.GetString(Convert.FromBase64String(textureValue));
        return JsonSerializer.Deserialize<TextureResponse>(texture); // todo: invalid json exception
    }

    public string Provide(Guid key)
    {
        var texture = GetTexture(key).Result;
        return texture == null ? Fallback() : texture.textures["SKIN"].url;
    }
}