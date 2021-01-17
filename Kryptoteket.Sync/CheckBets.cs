using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Sync
{
    public class CheckBets
    {
        private readonly IUserBetRepository _userBetRepository;
        private readonly IBetRepository _betRepository;
        private readonly ICoinGeckoAPIService _coinGeckoAPIService;
        private readonly IBetWinnersRepository _betWinnersRepository;
        private readonly IDiscordWebhookService _discordWebhookService;

        public CheckBets(IUserBetRepository userBetRepository, IBetRepository betRepository, ICoinGeckoAPIService coinGeckoAPIService, IBetWinnersRepository betWinnersRepository, IDiscordWebhookService discordWebhookService)
        {
            _userBetRepository = userBetRepository;
            _betRepository = betRepository;
            _coinGeckoAPIService = coinGeckoAPIService;
            _betWinnersRepository = betWinnersRepository;
            _discordWebhookService = discordWebhookService;
        }
        //55 23 * * *
        [FunctionName("CheckBets")]
        public async Task Run([TimerTrigger("55 23 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var allBets = await _betRepository.GetBets();
            var betsEndingToday = allBets.Where(x => x.Date.Date == DateTime.Today);

            log.LogInformation($"Found {betsEndingToday.Count()} ending today");

            int firstP = 3;
            int secondP = 2;
            int thirdP = 1;

            foreach (var bet in betsEndingToday)
            {
                var price = await _coinGeckoAPIService.GetBtcPrice();
                var userbets = new List<PlacedBet>();
                userbets.AddRange(bet.PlacedBets);

            if (price != null && userbets.Count >= 3)
                {
                    //Find winner
                    var usd = price.Bitcoin.Usd;
                    var firstUserBet = userbets.Aggregate((x, y) => Math.Abs(x.Price - usd) < Math.Abs(y.Price - usd) ? x : y);

                    //Find second place
                    userbets.Remove(firstUserBet);
                    var secondUserBet = userbets.Aggregate((x, y) => Math.Abs(x.Price - usd) < Math.Abs(y.Price - usd) ? x : y);

                    //Find third place
                    userbets.Remove(secondUserBet);
                    var thirdUserBet = userbets.Aggregate((x, y) => Math.Abs(x.Price - usd) < Math.Abs(y.Price - usd) ? x : y);


                    var firstUser = await _userBetRepository.GetUserBet(firstUserBet.BetUserId);
                    var secondUser = await _userBetRepository.GetUserBet(secondUserBet.BetUserId);
                    var thirdUser = await _userBetRepository.GetUserBet(thirdUserBet.BetUserId);

                    await UpdateUser(firstUserBet, firstP, 1, firstUser);
                    await UpdateUser(secondUserBet, secondP, 2, secondUser);
                    await UpdateUser(thirdUserBet, thirdP, 3, thirdUser);

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine($"Bet Name: **{bet.ShortName}**");
                    sb.AppendLine($"BTC Price: **${usd}**");
                    sb.AppendLine($"First Place: **{firstUserBet.Name}** | Bet: **{firstUserBet.Price}** | Points: **{firstP}**");
                    sb.AppendLine($"Second Place: **{secondUserBet.Name}** | Bet: **{secondUserBet.Price}** | Points: **{secondP}**");
                    sb.AppendLine($"Third Place: **{thirdUserBet.Name}** | Bet: **{thirdUserBet.Price}** | Points: **{thirdP}**");

                    var announceWinner = await _discordWebhookService.PostWinners(new DiscordMessage { Username = "Kryptoteket", AvatarUrl = "", Content = null, Embeds = new List<Embeds> { new Embeds { Color = "13938487", Title = "Bet winners", Description = sb.ToString() } } });

                    if (announceWinner) log.LogInformation($"Published winners");
                    else log.LogInformation("Failed to publsh events");
                }
            }

            log.LogInformation("App finished");
        }

        private async Task UpdateUser(PlacedBet firstUserBet, int point, int place, BetUser user)
        {
            var placement = new FinishedBetPlacement
            {
                BetUserId = firstUserBet.BetUserId,
                BetId = firstUserBet.BetId,
                Place = place
            };

            if (user.Placements == null)
                user.Placements = new List<FinishedBetPlacement> { placement };
            else
                user.Placements.Add(placement);

            user.Points += point;

            await _userBetRepository.UpdateUser(user);
        }

    }
}
