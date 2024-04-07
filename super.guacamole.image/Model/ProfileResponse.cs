// ReSharper disable InconsistentNaming

namespace super.guacamole.image.Model;

public class ProfileResponse
{
    public string? id { get; set; }
    public string? name { get; set; }
    public List<ProfileProperty> properties { get; set; } = [];

    public class ProfileProperty
    {
        public string? name { get; set; }
        public string? value { get; set; }
    }
}