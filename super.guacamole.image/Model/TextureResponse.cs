// ReSharper disable InconsistentNaming

namespace super.guacamole.image.Model;

public class TextureResponse
{
    public long timestamp { get; set; }
    public string profileId { get; set; }
    public string profileName { get; set; }
    public Dictionary<string, TextureData> textures { get; set; }

    public class TextureData
    {
        public string url { get; set; }
        public Dictionary<string, string>? metadata { get; set; }
    }
}