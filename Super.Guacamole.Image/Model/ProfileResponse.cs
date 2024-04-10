// ReSharper disable InconsistentNaming

namespace Super.Guacamole.Image.Model;

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