using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly IMapper _mapper;

    public BidPlacedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("---> Consuming Bid Placed: " + context.Message.Id);

        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);
        
        if (context.Message.BidAmount > auction.CurrentHighBid
            || context.Message.BidStatus.Contains("Accepted"))
        {
            auction.CurrentHighBid = context.Message.BidAmount;
            await auction.SaveAsync();
        }
        else
        {
            Console.WriteLine($"Auction with ID {context.Message.AuctionId} not found.");
            return;
        }
    }
}