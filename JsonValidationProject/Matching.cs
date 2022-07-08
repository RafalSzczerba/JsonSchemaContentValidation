using JsonValidationProject.Contracts;
using JsonValidationProject.Model.Cards;
using JsonValidationProject.Model.CardsRanges;
using JsonValidationProject.Model.Ranges;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JsonValidationProject
{
    public class Matching : IMatching
    {
        private readonly string mainPath;
        public Matching()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            mainPath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
        }
        public Tuple<IList<string>, IList<string>> MatchCardToRange(string jsonCardsfileName, string jsonRagesfileName)
        {
            RootCards rootCards = new();
            RootRanges rootRanges = new();
            IList<string> errors = new List<string>();
            IList<string> informations = new List<string>();
            try
            {
                string contentRootCards = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jsonCardsfileName}.json");
                string contentRootRanges = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jsonRagesfileName}.json");
                rootCards = JsonSerializer.Deserialize<RootCards>(contentRootCards);
                rootRanges = JsonSerializer.Deserialize<RootRanges>(contentRootRanges);

            }
            catch (Exception)
            {
                errors.Add("Wrong files format");
                return Tuple.Create(errors, informations);
            }
            Dictionary<int, Regex> regExp = new();
            List<MatchedCardWithRange> result = new();
            List<MatchedCardWithRange> matchedCards = new();
            List<string> cardsName = new();
            regExp.Add(1, new Regex(@"^(4[0-2])$", RegexOptions.Compiled));
            regExp.Add(2, new Regex(@"^(541000[0-9]|54100[1-9][0-9]|5410[1-9][0-9]{2}|541[1-9][0-9]{3}|54[2-9][0-9]{4}|55[0-6][0-9]{4}|557[0-4][0-9]{3}|5575[01][0-9]{2})$", RegexOptions.Compiled));
            regExp.Add(3, new Regex(@"^(4[5-8])$", RegexOptions.Compiled));
            regExp.Add(4, new Regex(@"^(49|5[0-9]|6[0-7])$", RegexOptions.Compiled));
            regExp.Add(5, new Regex(@"^(670000[1-9]|67000[1-9][0-9]|6700[1-9][0-9]{2}|670[1-9][0-9]{3}|67[1-9][0-9]{4}|68[0-9]{5})$", RegexOptions.Compiled));
            if (rootCards != null && rootRanges != null && rootCards.Cards != null && rootRanges.ranges != null)
            {               
                var regExpPANatLeast7 = new Regex(@"^\;[0-9]{9,19}\=", RegexOptions.Compiled);
                var regExpPANatLeast2 = new Regex(@"^\;[0-9]{4,19}\=", RegexOptions.Compiled);
                foreach (var card in rootCards.Cards)
                {                                 
                    var cardNo67Case = card.Track2.Substring(1, 2);
                    var cardNo4967Case = card.Track2.Substring(1, 2);
                    var cardNo = card.Track2.Substring(1, 2);
                    if (regExpPANatLeast7.IsMatch(card.Track2))
                    {
                         cardNo67Case = card.Track2.Substring(1, 7);
                         cardNo4967Case = card.Track2.Substring(1, 7);
                         cardNo = card.Track2.Substring(1, 7);
                    }      
                    if (cardNo67Case.StartsWith("67"))
                    {
                        var otherNumberAfter67 = cardNo67Case.Substring(2, 5);
                        if (otherNumberAfter67.Equals("00000"))
                        {
                            cardNo = card.Track2.Substring(1, 2);
                        }
                    }
                    else if (regExp.ElementAt(1).Value.IsMatch(cardNo4967Case))
                    {
                        var otherNumberAfter4967 = cardNo67Case.Substring(2, 5);
                        if (otherNumberAfter4967.Equals("00000"))
                        {
                            cardNo = card.Track2.Substring(1, 2);
                        }

                    }
                    else
                    {
                        cardNo = card.Track2.Substring(1, 2);

                    }
                    foreach (var regex in regExp)
                    {
                        if (regex.Value.IsMatch(cardNo))
                        {
                            result.Add(new MatchedCardWithRange
                            {                                
                                FirstName = card.FirstName,
                                LastName = card.LastName,
                                Track2 = card.Track2,
                                Matching = regex.Key
                            });
                        }
                    }
                }
                foreach (var range in rootRanges.ranges)
                {
                    string cardNo1 = range.from;
                    string cardNo2 = range.to;
                    try
                    {
                        var temp = cardNo1.Substring(0, 2);
                        var temp1 = cardNo2.Substring(0, 2);
                        var xx = regExp.ElementAt(0).Value.IsMatch(temp);
                        var y = regExp.ElementAt(0).Value.IsMatch(temp1);

                        if (regExp.ElementAt(0).Value.IsMatch(temp) && regExp.ElementAt(0).Value.IsMatch(temp1))
                        {
                            cardNo1 = cardNo1.Substring(0, 2);
                            cardNo2 = cardNo2.Substring(0, 2);

                        }
                        foreach (var regex in regExp)
                        {
                            if (regex.Value.IsMatch(cardNo1) && regex.Value.IsMatch(cardNo2))
                            {
                                result.Add(new MatchedCardWithRange
                                {
                                    Name = range.name,
                                    Min = range.from,
                                    Max = range.to,
                                    Matching = regex.Key
                                });
                            }
                        }
                    }
                    catch (Exception)
                    {

                        errors.Add("Range had to small characters");
                        return Tuple.Create(errors, informations);
                    }

                }
                var orderedByKey = result.GroupBy(x => x.Matching).OrderBy(x => x.Key).ToList();
                foreach (var matchedGroup in orderedByKey)
                {
                    var group = matchedGroup.LastOrDefault();
                    if (group != null)
                    {
                        if (group.Name != null && group.Min != null && group.Max != null)
                        {
                            for (int i = 0; i < matchedGroup.Count() - 1; i++)
                            {
                                var card = matchedGroup.ElementAt(i);
                                matchedCards.Add(new MatchedCardWithRange
                                {
                                    FirstName = card.FirstName,
                                    LastName = card.LastName,
                                    Max = group.Max,
                                    Min = group.Min,
                                    Track2 = card.Track2,
                                    Name = group.Name,
                                    Matching = card.Matching
                                });
                            }
                        }
                    }                   
                }                
                if (matchedCards.Count > 0)
                {
                    foreach (var matchedCard in matchedCards)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine($"PAN: {matchedCard.Track2}");
                        sb.AppendLine($"FirstName: {matchedCard.FirstName}");
                        sb.AppendLine($"LastName: {matchedCard.LastName}");
                        sb.AppendLine($"Card distributor: {matchedCard.Name}");
                        sb.AppendLine($"Min range: {matchedCard.Min}");
                        sb.AppendLine($"Max range: {matchedCard.Max}");
                        var matchedCardsListed = sb.ToString();
                        informations.Add(matchedCardsListed.ToString());
                    }
                }
            }
            return Tuple.Create(errors, informations);
        }
    }
}
