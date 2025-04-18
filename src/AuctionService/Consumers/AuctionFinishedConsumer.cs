using Contracts;
using MassTransit;
using AuctionService.Data;
using AuctionService.Entities;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContext _dbContext;

    public AuctionFinishedConsumer(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine($"-- > Consuming AuctionFinished message for AuctionId: {context.Message.AuctionId}");
        var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);
        
        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }

            auction.Status = auction.SoldAmount > auction.ReservePrice ?
            Status.Finished : Status.ReserveNotMet;

            await _dbContext.SaveChangesAsync();
    }
}