using MassTransit;
using Contracts;
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _dbContext;

    public BidPlacedConsumer(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var bidPlaced = context.Message;

        // Check if the auction exists in the database
        var auction = await _dbContext.Auctions.FindAsync(bidPlaced.AuctionId);
        if (auction == null)
        {
            Console.WriteLine($"Auction with ID {bidPlaced.AuctionId} not found.");
            return;
        }

        // Check if the current high bid is null or if the bid status is accepted and the bid amount is less than the current high bid
        if (auction.CurrentHighBid == null || (bidPlaced.BidStatus == "Accepted" && bidPlaced.BidAmount > auction.CurrentHighBid))
        {
            auction.CurrentHighBid = bidPlaced.BidAmount;
            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"Auction {bidPlaced.AuctionId} updated with new high bid: {bidPlaced.BidAmount} by bidder {bidPlaced.BidderId}.");
        }
        else
        {
            Console.WriteLine($"Bid {bidPlaced.Id} did not meet the criteria to update the auction.");
        }
    }
}