using System.Text;
using System.Text.Json;
using NLog;
using Super.Guacamole.Image.Model;

namespace Super.Guacamole.Image;

public class SkinProvider : IProvider<Guid, byte[]>
{
    private readonly List<string> _defaultSkins =
    [
        "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAV1BMVEUAAAD////8uWH5p4b7qFf3mUzxk27yjkXxgW7lf2HbeinbcFB8fHyjRyNcXFydRSKTPx4faHgaYG+HNxhFR0gVWGaAMBINTlwJRVIzNTYmKClEHA0kAwPF9A5PAAAAAXRSTlMAQObYZgAAArJJREFUeNrtluFSozAUhbEJNybokjUSKPr+z7nnXIittA7Zdhz/+AHhduz5uEkdoCmk2MfU90npuq75X3oKUlzytwgiro4jMn6TgFcv+bs66G4V6CJiv0MQ0UJCBx7cIuijzgF5Ui9IKcWMS+c+Ro8zy6hDzjnlVNE6Nzq8iOc5roJUJ8gI5B4hMUBQ9Lk0hbFCkHpjenzZKFE/M8rGINkVYPLGo/308G7M+0MSfDB5mQMNuwJ8Rwz2x0cDMEoyktelgGVfwIuJCNLeYxDgs/ee6Vj5K+TceTGK+O6DBHJTwwSOx3k+rUtyzlo7rjQrrbWBPV4VzPPxeJpWTtk7N07Kh8BSIF232wFXr7smONR2YMV7uTqFyw7GFb0U9qBQcDiwsm3rXBtCizTq4EBA1RSGFYZ5tCCEgyBvGWXM4cpLHucQHOtPghcwTNMwAoQoAFYFGtTdau0oaLeCvxCM48BjEVjwWcAzSxVcTAF5hkkRaJy0CKMuUEC2a7ARtC0jRUHChi8FjoQlpGkLUErBeyFNYRy4cViAXNNlvjTAFMQADDAIi6aAH39UUJD0zO356emZBWslUUAEfBIcweuf19fjigu6StMUlLJyDqGSp+EkmJk65WenMcxoUlVhFXAFuJ0LNgSi/xJTOMcsMF46+JpU8acfE8TvFyRy8bVcnKnqThsvW8incwU5NvcR852Ct7dv6MAqemvBYO3OIjS//Djbhy3vq1LuQre8cFjQdWJu7sAerL2rA06BHVSHxwJKB9bnjI67AoaGFdZ8H2CYBh67Ar4nvAAVAEeBGki7L0BqeV8ArClgeJXUCQbkCxScUyU4J2wo9wMFxa5AvJdzajqgYuRGxC+Pcx6k4mfEBngmAkpYagTb9wUha5imfQFZ8vNMgRfvTz34XcG8gQKGlSsd/AMLulgNFVXqIAAAAABJRU5ErkJggg==", // Ari
        "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAZlBMVEUAAAD////+/f3YuJTPrYXIpqy4mXW5jpaBnLWhfGeTbVGSZ0yGX0jbL0yFWj9+UzfKJkK7JD1RUVFtSDCiKDx1PChsPidfPil/JTQ8PDxKMSE0NDRBHgIiIiIrHRgiFhEbEQ0OBAF2MqrOAAAAAXRSTlMAQObYZgAAAsdJREFUeNrtlutymzAQhZNwC7joUikRluPavP9L9pzd8YCNO2D3X9oPFGvInG92ZQN6uTCezuN5HM85J5Dzy6MgLopMQ35CwPwJZWTyjABp6eFpwYgS2MPTAuZpeFqAFfiFMT7/LZwgYAVJyNuDIzvH6ukaEP1B6Me4pXeepzEFa0NiTDwnivGvdQFBGnES4BAhTZytC07jzmr+YrA7xJmnZX0Nzlnzr1/Wfr2GQEOWJtjNuiAENA/Bzgq7gHmAkQ4Y8qog5ZQYAh8f6IAkvUjSqiADksTBrPLQD6oGR8A5GZoEmqGuf4C6vlx3fe+sMd1dweF4PMwEOVMgTALkow13qmrBPUFRFHWNP5NAK7gvAJMgpBSWArawqKBu67IsirLkBMRh6PsqgKbvhyHGRojeOxbgIq5559y8/1LgBLhhiK6vqqbpnacAsRg98t5jCeIAovNzQckKxMDTx+hZqvMgRk5JTwVQgY9+EkheDYSCHiDjfGRMo8MNk0DzdUEBhmOIEUhgYJwqH5QkYHLVAsOSB6yAcV2xyOqZdwHIzcq4DXYuIO1FEAUP4gWZW+ZxUmDngvaGmhRCPQcZFYTE2Z8Fn8r7++c1CBMTgFkKllWQdobGkSfGTALcAof9z/3+cDgKNfKlQgMXRobU33EFeHSTYM+sDEVC6L8tilJliggMDDbZ0KGC//yTVG9KA6rqrXr5RhQ3PCzg00xvOZ38fQXfgeV+gU/2zmx+iCz3CxGGnMNWwXK/gHyPCh4UgHkLmyvgNqG93i9EfVHKO9Zv6B+C2X5hAHzLI84jbhAgO+0XVOCAB9FvEJQUqIEMNzTgTakqnPcEhUABRiIBKGlTCwhrHgSJ6x7Y4tgkIHOBVYHl5+rd2N6g+wFmlbvPAwygz4OFAJhJYB6vgBijCmPW1mC5XwiJq9CpoFv9Fpb7BZss8sYKpltU8BtigWz/SNcCVAAAAABJRU5ErkJggg==", // Zuri
        "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAmVBMVEUAAAD////Y2Ni6urryn1+jo6PYhEuFqTeCk6txkLx6mzKFhYV3h53Bcj92myZZh8pkhLBvf5RsjiNbfKiwZTdOfMFqhyufZj9mhyFBcrtqamo6ccJceh6MWzpbexR+WT1fX18vY69/UTNPbQ9XV1dyTjVMZxEnWKB3QyBYRTdJSUlISEhXQjIaS5RQOy04ODgvLy8iIiIAAABcma8NAAAAAXRSTlMAQObYZgAAAsxJREFUeNrtlo1ymzAQhJUEqw2E4Co4rYiRrUKjJHYLzvs/XPcuKKUoA/7pdNppP419EuZWK8kDJzy7tmmemnaXd4hDaZ6Qjs/RAm2L6Zu2/QMcpDlIT3CQpsg/QoAPoWnaXKZAHi6AY3yiVaSSSQ+YGRO3DfI7hYa9tOjycFqA72vJwIsApXYCNNrDOs8PkN4gwAPp7JC8l8B2u318fKTbWQYdDLfgG7MVU6xWqyiSq508e5by+UzuVjKKVj8QU5RliUMr86gj/4KDLInPoCzFFPy3kSCKHh7gBaR87SMzfZw5k0IkAkhGdh+xD3qx0OvNZu3HsZXAxK5DdCijDJl8U2AD/NhYWoQy7ivjXoVVrCKZ55MOFAsY5QX2crAGvSVgT5RfwrgD62yHsyRkKlMpG0sZG0V9o6hZs7xf3qPDEUH1BO6s1draO8eRbjLaxBS5xdSsul3eLtHhSFf6Ajyzg4DOshcHWkEh3tuB8wIWsSIuqsp0Dipqla7uB/wskGV+CdoB6kFCVR24bEcELK+dNxE9GvNaLGyQ1pXWCzjD5kvCR+FxA3hPMkqxGED0yvIAxx8RJEBxXGCRZZiWl6WdXcCDjjqkHAhIAtcJmoUF2LTWvC9OU78nIAMB/wtDAnYBC7DdCZCe9flMX2BDrNcbH2kRMA4sd50mNe2i1x2g1hO4IM7PL3wkB5TNX92fiz7eAaWzg//8k3wC74CP4lCWQF1fKx/FL2NOXF7OfRRDrsQ4N8R8fuOjGPJenMbpAh/EX0j4tjbKJEWSHCSwAf23cl0Xs9/rYA2OcBDWC8FbeZywXgjqglHCeuEoB84LIAav9annAgtYzGwR3qoLJp4LYb2QEzUoihrkk88DN6CQsqiLpKOoJ58HbkBdUP4sIRAKMU5YL9RMQUvgIMYJ64W6hvHEM+kgrBcKTBvswQhBvZDUCeXPZsnsTQffAZn9sZj9h5orAAAAAElFTkSuQmCC", // Sunny
        "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAgVBMVEUAAAAoEysjECYfDCJMLUhuRGuRYJCym67oum3upWb2043yz4j///9DZ1OlcoCHQmBIHClnLUMRCR0LCBUHBQ9PLS3rv87yztqONzfflljOiE60cjyqZzD31Xb74or77ZH89KpsFS6BLi53QyBLKSlIIyNCHBwlEysMBxcJBhIIBQ8XqODZAAAAAXRSTlMAQObYZgAAA6pJREFUeNqd1sGOFccZBeDv/HMhCorNMAwMIRgW2Zh5/3eJkl2khHCDZA/Y8sIwdTywuVLLl8F8rVZ3VXcf/dWqkgrA8xcvvnvx/Lvnz549vfHsmS+0AwhZ0zDgDwdUGqk/aABSKfnqgNJQgK/6ByVfHYBOwwLjCwXghQbeA175MnmBBhpAA2uw5t8+b6pAw34PVE3puE3uwWOF9/Wnxv/BI2N584vP24mF9H1BqFFvLmr2bjPh9Kzk3c/8/BN6dkqibcUtdpx2Nc03+IumXTm9IqVxmzl9IPmwlHv30H5IPDg9VerULfIMyPgVd60CVpH5j88LnA3BG8AlQDvoPwAvG1filYMB1dMHD04BXL199+7tFbCVGduAscBWDVUAKV2Lgx3IWlc4KCWsabYVwMH5w7OH52cPTs/Oz88f3jQuL19+f3n5txuXl5ff39x/PD5eXt60b86b1qc3AKZlYSypWLT3v/32G4VaIymr6KJYADtCulN0SkgVSyNto1NaaMfBZNJxUizaZonGYBAUNGqCFMAoUgtNiCwEWSmByDAqCR2A3UrTNJhmhSAra6Q0iFwBRDkYkVbGQLKsIEQShDomT+Fgu3oawBWAFF4BdtvsV4LrDCoo4s8AioPdEkDBCX3iv5Mn9r05L+ybi70C2WZMs9rea1daqhfs1d6jx2t5zZMevk9EAsCu18pPuKZcvBadPtmvsX988Xpfn3pBBa3jksnuzt27d+7sdjZOJrGxw9Z4f+zZLBtjY9QxZW4NEP6+g92y1bp9CElJhSQLqaCzpivSNO2xCtZCICAClQgSQhwLQACh5OTkzl0qlEgMjgfUvwKBwYf3RTU6yuJ4BaECkaKlkrZKtVqOz4MsAFOpk5CTitEQieujASGFIriGVFQRiToaYE0LXaEJKdeIklY5GtBEizSaVoIuG8cCdvQkkBEhEv31SwOoAKFEovWlAdfS4jDqoI6JjYfRaII26we8xFv1P0DMKmBsdSk+7hdqAWomAMRwfCLRAukAsZSD5XgFBFc//nhVAdhUUACxcb7GShNrSh43a9ppWGb9c/qpI53qrLEVlaGkAWVZKjXAQEfGxsJUmVWqkgbtAE0jEWpsRVuqOlRhBDFaARUytgqNEm2mkYgO1YLVVsrORpRpliiuohzstMYJKjI2IkMZRGy1IqiI2HhqK+XgdShEITb+estieXXbavyd/QI9mmdsNKu9tw77hUgCkNiIjUeA+97ivl/StCkg2yH8BnIO7kauG9F9AAAAAElFTkSuQmCC", // Kai
        "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAeFBMVEUAAAD////397Dq3lOwz8upycibvLuLrq7XnTTBgFScc6mrckyVa56sdDCTaZymaEmkaECiaEKKYpSGXI6aYTuBWIl4T35mP2l3QyBuRS1pKzRZNiBjHTZeGTI+JTY7JDQ4ITJSFSswGyorFSUgDhwQFBkNDxULDxMR3V+9AAAAAXRSTlMAQObYZgAAAvVJREFUeNrtlm1T4yAQgLmGlyq5M7e81FhNkRj7///h7QI1Uh2T9oszNz6lQDPZh4WMZtkJB2AAnLEFdikGyGDA2h65QgAADhDbJ64SkMFdLTAp3l8vcOC9cwZMFhh2Kd61AMY7nw/RX74FD4gHYxPrM4D8/KlrUVB+gTHGpTlbIp+dAeP+4GRHvuxxnubLAlqcYsB1GnwHCePBkBXcCoGHmxuDMZ0mHDj8DTSAM9hWCIzu6fZfD1o//ALwba+NMw6wM8avOH1jdevaVifa1t9qa/zOG4dthcCh4e+th/ZO39/ru1vw0Bi363am887vOraE1l1nYWdAJwAwsus00SGarSGEMDw9Pz+xAudCSsF5OIRhwI4V8LLW9lPBMDw/Pj7PN/b7fS94CAckzAIptN2zj2B8nYEigfqQgZD88wwG5BFhBaFU3yslhgIrSME/ZhDiMBNC4IiQx2k64u2EUAJbuiqE4DiTXLKZYcSw2VAEI0ICCpAYQgMJJE8zwWZo1fhOwQkpj0dJN5OAOiStL9NYCcIYhhBjNuCYs5bTJFUSUESmBEu8JutHiJR4hCN0W942thzI8+qkPBfQGRQDTTiB8TwhUqOtqwo2EwMyTac8oiSEULSk5PihTBSXNWxmmjB8u0nDzGuzeS0zsgn+aisqAdLMoXQUYYjY522NObfxC0HNgYhjxEaz8WUMI7YXnbAEjl8IykMNZRwOOYNDju9721tbCZrtVNg2zWbT0BN59x1i/oOKJYMssOyHH74JpaTMX4JdDq9g30Cz2VBrttvf26Zh/z30tqre1lJYrS8RoKGqF6TYN/YiwVBnIBYyWKoXuOB7ixmsrpcyMdAcgxFRXvRrBKdCoRQOmP5buBRsmRIecyZZkAsLhY51gvJqGQOOSSAFfRR2qzMoaSC0/9P6Yu0WyhkEGlRF+b8glUojws4JpV5AYkTDeV3Az2DnTFOs6gVbwZaZ6oKBBHpmlaCGBG8Ke42gp6KgxPdsmfN6gQRfZfAPt0h8Qrb+cvwAAAAASUVORK5CYII=", // Efe
        "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAdVBMVEUAAADTy8bgxbzJsannqjm1n5jgnyfblRHTkRbJiA2+gA20egyichiuU1CUZQuaSkeaSUeGQD5aRTR0NTNOPC1ENSg7NzQ6LSI4KBs4JxstKCUwIhgyGR4rHhUqHg8uGAAmEhYgFwseDhEaEQofCg4QCgYWBQn8zoeeAAAAAXRSTlMAQObYZgAAA2lJREFUeNrtltF2ojAQhtmWQhgiFTFrbGMaKe77P+L+MxEPGz0Fujd7sX+EMDnMl0nUyWSj3BCCG5wLYZBPtlYuEsK3AUNwrg+MGaAwrAawLy64f0LDekB0793gGeDXA9ibl2A9CN7bb+1BcCFY6yFrv7WEwBGIVgB6F/BxfRgcblcDH6GhnwWIF9zw8mCMY4pgMCCYeQATfrG/DYOxwcFg/54bHmYBmM7yTNawQICBW1ixBHc4423z48OYjx8GfucDBhFDv2wJwR0OztmdEe2sYztIA2zJEvrzJYBgzPu7MfAPl3PvZCP5WrKE/nI+e28NZL0/ny8cu3ijZUvUbrfbZ2i0T6dP6KQx3HW4jeMlNWa/Xww4Nhhu2wlA5Q1ifAxoHwD0drPZ4roBqOAIlgKOxyYF5OWDCNqubRnQ4mGDlxvWCToetdK6aq5SpKuSXA+5khIAz8KALdSI4H7KSTcNjYCqVqTyXqTyFABNAVCRwx8AXFEIoKaih3RDxX0ErQDwGB2Ip6MaAIoDGgHovCRmok8AGwA6NJaGEDfeLbSoUBIHAlCFyhlAhUoB2GsBYCWVJqUJbjkRbxyTFG4NYwgkieqLPSBVY72xNbWK3jx1patKUcW9qu4BbdfdNpGD4Cbf4Q1GDVFFhB7dBNBBLTT2Gu5lJyKCoSg2I9pbCP0E8JYIMwhJmDBKik3c4Q/t91PAy8vPq15enqEOGv9EnSiyYrbx3nprd1PAM+vpKXbQ7ZfJF4zx/2CNFYLxBslrLkvFUy5LVVJYeNhAj5KRyt1SwONxKv4SkJdhgXMavRz7wSX5YG79bjIglY9L8sGXgORIjCk+yQczq3CJHfosyQczAiFVkg/Wq6mrJB/8M3ptX1830CseuM/+S7RNDtuCyj/SGJX5OkCZK28ngFwVMxVLAlAFSQSTiNYBELJEMIkodXlcLyCZbviUwYycDmRitgEc7VT39QKpusL7nA5kYraxB6Od6v6srLUmKojk30hZxjamHu1HADREMKkXal3mnA7k22MbU4/2YwCvnQEsOGiEzBOiE4DG1KM9UVIvCAAriQlgLA8AiCf2NT0s2INYEMgdXTYvKRQmgFgQyB3dgnyQ1gsW2ptR2bzeEo0VAcRl7rzSesF6Lgp2AtjZBYC0XjDeiH+sD+7f/w0kuYUGQiisiwAAAABJRU5ErkJggg==", // Makena
        "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAAYFBMVEUAAAD////NeVu5Z0qnXEOpVjqRRzEldFG5MRyCOiSnKRVzOyceWj+WIhBjNiBZNCBYLB0VSDF4FgZKJhREJRVkEgUPNSQ4HhEuGA4wFQgVGyEmFg0oEQYjEgoQFBkGCAq2w9khAAAAAXRSTlMAQObYZgAAAo5JREFUeNrtluli6iAQRqkgmjS0FYngFt//Le98LGZBxZi/9xBC2mZORrBkWMJZZ6xx1rrQ2FxC3DKB8ZGuItyHArNAgHBLfYHA/lIzTkIgPxcISQYpxUcCfAwRmbd84fiNKxgHY6m5d5fvagVHs9e0nmEsC+g2HFLwFSeEtMbN+kZS8GplJOfrdejSrFbW53B9UyC4pL7++uP872vNhZHUKRztHQGyt7xHWHyOa2qsxNVKKQDn398UDiQJHDU6vyOoAFnCHMoKBAEae4dd0+yOl+OFRTABdLhm42lYpFZKIclcQFzIMBA4B4E3NGOBqKpHgnEG+JekUxOZn4EQmNlcUD/IoBlC+bZACkK2Q2qPjtQ1S6TABP21bfVNyg6j1kVBiI+G3S4Kuu40Q7DxIJ5QSrWtovgbRqWiQddA6YBSveCAqBB7ALrVXnCCAD/Qga4CSaB6wYSQwa3rOoweH60nvBRoEpw6BIKt0ts5AoAMbiqw3WrqzwViQkgaC6tHAjFhKOAeEdCgPRyaQ5smzHfe3+UjWGIPfn72ifTF2lAfEuOlFGgDwRHs98dEEqDHawABQHifwWywyS7CmcUCy5ZhTfkhRLzLOCJtrymFM+EKefQCOqXCJc3imbGzdTMEiLbGjASOzRAw52ERy4og5D5m5L/aTCgIQFnw6HFbUG9rD12wRDOBPUFNYP8p1gvhZdtPYI1dpLgt5K/7XlBVoheYsuBlBtbMzqAeZZBtK6/qBe2pI1qfiT6BbDKyeiETjPeDbGvJ6oWCgPVXz+oF7VG1Ryld2hTyegGoOzp77GbCYcLDt/ISQdoPPHTBmgklAXi5L4gCrEBWL3j4nZIgrxfwPpd3g5CsQFYvQPAqg3+nKHCSbVy27AAAAABJRU5ErkJggg==" // Noor
    ];

    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public async Task<byte[]> Provide(Guid key)
    {
        try
        {
            var textureUrl = await GetTextureUrl(key);
            if (textureUrl == null) return Fallback();
            var textureData = await GetTextureFromUrl(textureUrl.textures["SKIN"].url);
            return textureData;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to provide texture for '{key}'", key);
            return Fallback();
        }
    }

    private byte[] Fallback()
    {
        return Convert.FromBase64String(_defaultSkins[new Random().Next(_defaultSkins.Count)]);
    }

    private async Task<ProfileResponse?> GetProfile(Guid key)
    {
        var client = new HttpClient();
        var response =
            await client.GetAsync($"https://sessionserver.mojang.com/session/minecraft/profile/{key.ToString()}");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ProfileResponse>(responseBody);
    }

    private async Task<TextureResponse?> GetTextureUrl(Guid key)
    {
        var profile = await GetProfile(key);
        var textureValue = profile?.properties.FirstOrDefault(p => p.name == "textures")?.value;
        if (textureValue == null) return null;
        var texture = Encoding.UTF8.GetString(Convert.FromBase64String(textureValue));
        return JsonSerializer.Deserialize<TextureResponse>(texture);
    }

    private Task<byte[]> GetTextureFromUrl(string url)
    {
        var client = new HttpClient();
        return client.GetByteArrayAsync(url);
    }
}