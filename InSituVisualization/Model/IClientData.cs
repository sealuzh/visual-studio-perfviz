namespace InSituVisualization.Model
{
    public interface IClientData
    {
        string Browser { get; }
        string City { get; }
        string CountryOrRegion { get; }
        string Ip { get; }
        string Model { get; }
        string Os { get; set; }
        string StateOrProvince { get; }
        string Type { get; }
    }
}