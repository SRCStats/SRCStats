using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SRCStats.Models.SRC
{
    public class UserApiData
    {
        public UserAPI Data { get; set; }
    }

    public class UserAPI
    {
        public string Id { get; set; }
        public Names Names { get; set; }
        public bool SupporterAnimation { get; set; }
        public string Pronouns { get; set; }
        public string Weblink { get; set; }
        [JsonPropertyName("name-style")]
        public NameStyle NameStyle { get; set; }
        public string Role { get; set; }
        public DateTime Signup { get; set; }
        public Location Location { get; set; }
        public Assets Assets { get; set; }
        public Link[] Links { get; set; }
    }

    public class Names
    {
        [Key]
        public int Id { get; set; }
        public string? International { get; set; }
        public string? Japanese { get; set; }
        public int CountryId { get; set; }
    }

    public class NameStyle
    {
        [Key]
        public int Id { get; set; }
        public string? Style { get; set; }
        public Color? Color { get; set; }
        [JsonPropertyName("color-from")]
        public ColorFrom? ColorFrom { get; set; }
        [JsonPropertyName("color-to")]
        public ColorTo? ColorTo { get; set; }
        public int UserId { get; set; }
    }

    public class Color
    {
        [Key]
        public int Id { get; set; }
        public string? Light { get; set; }
        public string? Dark { get; set; }
        public int NameStyleId { get; set; }
    }

    public class ColorFrom
    {
        [Key]
        public int Id { get; set; }
        public string? Light { get; set; }
        public string? Dark { get; set; }
        public int NameStyleId { get; set; }
    }

    public class ColorTo
    {
        [Key]
        public int Id { get; set; }
        public string? Light { get; set; }
        public string? Dark { get; set; }
        public int NameStyleId { get; set; }
    }

    public class Location
    {
        [Key]
        public int Id { get; set; }
        public Country? Country { get; set; }
        public int UserId { get; set; }
    }

    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string? Code { get; set; }
        public Names? Names { get; set; }
        public int LocationId { get; set; }
    }

    public class Assets
    {
        [Key]
        public int Id { get; set; }
        public Icon? Icon { get; set; }
        public SupporterIcon? SupporterIcon { get; set; }
        public Image? Image { get; set; }
    }

    public class Icon
    {
        [Key]
        public int Id { get; set; }
        public Uri? Uri { get; set; }
    }

    public class Image
    {
        [Key]
        public int Id { get; set; }
        public Uri? Uri { get; set; }
    }
    public class SupporterIcon
    {
        [Key]
        public int Id { get; set; }
        public Uri? Uri { get; set; }
    }

    public class Link
    {
        [Key]
        public int Id { get; set; }
        public string? Rel { get; set; }
        public string? Uri { get; set; }
    }

}