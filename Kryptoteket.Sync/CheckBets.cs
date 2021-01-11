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
            string identifier = "bet";

            foreach (var userBets in betsEndingToday)
            {
                var users = await _userBetRepository.GetUserBets(userBets.id);
                var price = await _coinGeckoAPIService.GetBtcPrice();

                if (price != null)
                {
                    //Find winner
                    var usd = price.Bitcoin.Usd;
                    var firstUserBet = users.Aggregate((x, y) => Math.Abs(int.Parse(x.Price) - usd) < Math.Abs(int.Parse(y.Price) - usd) ? x : y);

                    //Find second place
                    users.Remove(firstUserBet);
                    var secondUserBet = users.Aggregate((x, y) => Math.Abs(int.Parse(x.Price) - usd) < Math.Abs(int.Parse(y.Price) - usd) ? x : y);

                    //Find third place
                    users.Remove(secondUserBet);
                    var thirdUserBet = users.Aggregate((x, y) => Math.Abs(int.Parse(x.Price) - usd) < Math.Abs(int.Parse(y.Price) - usd) ? x : y);

                    //Id without userbet.Id is the discord id of the user
                    var firstId = firstUserBet.id.Replace(userBets.id, string.Empty) + identifier;
                    var secondId = secondUserBet.id.Replace(userBets.id, string.Empty) + identifier;
                    var thirdId = thirdUserBet.id.Replace(userBets.id, string.Empty) + identifier;

                    var firstUser = await _betWinnersRepository.GetBetWinner(firstId);
                    var secondUser = await _betWinnersRepository.GetBetWinner(secondId);
                    var thirdUser = await _betWinnersRepository.GetBetWinner(thirdId);

                    if (firstUser == null) await _betWinnersRepository.AddBetWinner(GetPoints(firstP, userBets.id, firstUserBet, firstId, firstUser));
                    else await _betWinnersRepository.UpdateBetWinner(GetPoints(firstP, userBets.id, firstUserBet, firstId, firstUser));

                    if (secondUser == null) await _betWinnersRepository.AddBetWinner(GetPoints(secondP, userBets.id, secondUserBet, secondId, secondUser));
                    else await _betWinnersRepository.UpdateBetWinner(GetPoints(secondP, userBets.id, secondUserBet, secondId, secondUser));

                    if (secondUser == null) await _betWinnersRepository.AddBetWinner(GetPoints(thirdP, userBets.id, thirdUserBet, thirdId, thirdUser));
                    else await _betWinnersRepository.UpdateBetWinner(GetPoints(thirdP, userBets.id, thirdUserBet, thirdId, thirdUser));

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine($"Bet Name: **{userBets.id}**");
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

        private BetWinner GetPoints(int points, string betName, UserBet first, string firstId, BetWinner winner)
        {
            if (winner == null)
            {
                return new BetWinner
                {
                    id = firstId,
                    Name = first.Name,
                    Points = points,
                    BetsWon = new List<string> { betName }.ToArray()
                };
            }
            else
            {
                var updated = winner;
                updated.Points += points;

                List<string> lst = new List<string>(updated.BetsWon)
                        {
                            betName
                        };
                updated.BetsWon = lst.ToArray();

                return updated;
            }
        }
    }
}
