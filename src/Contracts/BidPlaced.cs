namespace Contracts;

public class BidPlaced
{
    public string Id { get; set; }
    public string AuctionId { get; set; }
    public string BidderId { get; set; }
    public int BidAmount { get; set; }
    public DateTime Timestamp { get; set; }
    public string BidStatus { get; set; }
}