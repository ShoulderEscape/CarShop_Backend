namespace CarShopAPI.Dto;

public class CarDto
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public int MileAge { get; set; }
    public string FuelType { get; set; }
    public string Transmission { get; set; }
    public string ContactName { get; set; }
    public int ContactNumber { get; set; }
    public int Price { get; set; }
    public string? Description { get; set; }
    public string Imagelink { get; set; }
    public DateTime AuctionDateTime { get; set; }
}