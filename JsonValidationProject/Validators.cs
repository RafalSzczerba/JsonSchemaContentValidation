using JsonValidationProject.Contracts;
using JsonValidationProject.Enums;
using JsonValidationProject.Model;
using JsonValidationProject.Model.Cards;
using JsonValidationProject.Model.Ranges;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonValidationProject
{
    public class Validators : IValidators
    {
        private string mainPath { get; set; }
        public Validators()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            mainPath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
        }


        public IList<string> JsonTrack2EnterCut(string jobFileName)
        {           
            string searchText = "\"track2\": \";\r\n";
            string desiredText = "\"track2\": \";";
            string textFile;
            
            IList<string> validationEvents = new List<string>();
            if (!File.Exists($"{mainPath}\\Source\\{jobFileName}.json"))
            {
                try
                {
                    textFile = File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.txt");
                    textFile = textFile.Replace(searchText, desiredText);
                    File.WriteAllText($"{mainPath}\\Source\\{jobFileName}.json", textFile);

                }
                catch (Exception)
                {
                    validationEvents.Add($"Wrong file name: {jobFileName}");
                    return validationEvents;
                }
            }
            else
            {
                try
                {
                    textFile = File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.json");
                    textFile = textFile.Replace(searchText, desiredText);
                    File.WriteAllText($"{mainPath}\\Source\\{jobFileName}.json", textFile);

                }
                catch (Exception)
                {
                    validationEvents.Add($"Wrong file name: {jobFileName}");
                    return validationEvents;
                }

            }                
            return validationEvents;
        }


        public IList<string> JsonContentValidationCards(string jobFileName)
        {
            IList<string> validationEvents = new List<string>();
            string jsonString;
            JObject jsonObject;
            try
            {
                 jsonString = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.json");
                 jsonObject = JObject.Parse(jsonString);


            }
            catch (Exception)
            {

                validationEvents.Add("Wrong file format");
                return validationEvents;
            }
            JSchema schema = JSchema.Parse(System.IO.File.ReadAllText($"{mainPath}\\Schemas\\JsonSchemaCards.json"));
            jsonObject.IsValid(schema, out validationEvents);
            return validationEvents;
        }


        public IList<string> Track2Validation(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.json");
            IList<string> errors = new List<string>();
            RootCards rootCard = new();
            try
            {
                rootCard = JsonSerializer.Deserialize<RootCards>(content);

            }
            catch (Exception)
            {
                errors.Add("Wrong json format");
                throw;
            }
            if(rootCard?.Cards != null)
            {
                foreach (Card card in rootCard.Cards)
                {
                    var regExp = new Regex(@"^\;([0-9]{16,19})\=([0-9]{4}|\=)([0-9]{3}|\=)([^\?]+)\?$", RegexOptions.Compiled);
                    var matches = regExp.Matches(card.Track2);
                    if (matches.Count == 0)
                    {
                        errors.Add($"Wrong track2: {card.Track2}");
                    }
                }
            }         
            return errors;
        }


        public IList<string> ZipCodeValidation(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.json");
            IList<string> errors = new List<string>();
            RootCards rootCard = new();
            try
            {
                rootCard = JsonSerializer.Deserialize<RootCards>(content);

            }
            catch (Exception)
            {
                errors.Add("Wrong json format");
                return errors;
            }
            if (rootCard?.Cards != null)
            {
                foreach (Card card in rootCard.Cards)
                {
                    var regExp = new Regex(@"^[0-9]{2}[-]?[0-9]{3}$", RegexOptions.Compiled);
                    var matches = regExp.Matches(card.Address.ZipCode);
                    if (matches.Count == 0)
                    {
                        errors.Add($"Wrong zip code: {card.Address.ZipCode}");
                    }
                }

            }            
            return errors;
        }


        private Tuple<IList<string>, IList<string>> PANValidation(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.json");
            IList<string> errors = new List<string>();
            IList<string> informations = new List<string>();
            RootCards rootCard = new();
            try
            {
                rootCard = JsonSerializer.Deserialize<RootCards>(content);

            }
            catch (Exception)
            {
                errors.Add("Wrong json format");
                return Tuple.Create(errors, informations);
            }
            if (rootCard?.Cards != null)
            {
                foreach (Card card in rootCard.Cards)
                {
                    var regExp = new Regex(@"^\;[0-9]{5,19}\=", RegexOptions.Compiled);
                    var matches = regExp.Matches(card.Track2);
                    if (matches.Count == 0)
                    {
                        errors.Add($"Wrong PAN for card: {card.Track2}");
                    }
                    else
                    {
                        var result = matches.FirstOrDefault().ToString();
                        informations.Add($"Card:{card.Track2} has follwoing PAN: {result[1..^1]}");
                    }
                }
            }           
            return Tuple.Create(errors,informations);
        }


        private IList<string> IsExpired(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.json");
            IList<string> errors = new List<string>();
            RootCards rootCard = new();
            try
            {
                rootCard = JsonSerializer.Deserialize<RootCards>(content);

            }
            catch (Exception)
            {
                errors.Add("Wrong json format");
                throw;
            }
            foreach (Card card in rootCard.Cards)
            {
                var regExp = new Regex(@"\=[0-9]{4}", RegexOptions.Compiled);
                var matches = regExp.Matches(card.Track2);
                if (matches.Count == 0)
                {
                    errors.Add($"Can't find the expired date in: {card.Track2}");
                }
                else
                {
                    var date = matches.FirstOrDefault().Value.Substring(1);
                    try
                    {
                        DateTime dt =
                        DateTime.ParseExact(date, "yyMM", CultureInfo.InvariantCulture);
                        if (dt < DateTime.Now)
                        {
                            errors.Add($"Card expired: {card.Track2} ");

                        }
                    }
                    catch (Exception)
                    {                        
                        errors.Add($"Date cannot be parsed");
                    }           
                  
                }
            }
            return errors;
        }


        private IList<string> AuthMethod(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.json");
            IList<string> informations = new List<string>();
            RootCards rootCard = new();
            try
            {
                rootCard = JsonSerializer.Deserialize<RootCards>(content);

            }
            catch (Exception)
            {
                informations.Add("Wrong json format");
                throw;
            }
            foreach (Card card in rootCard.Cards)
            {
                var regExp = new Regex(@"\=[0-9]{7}", RegexOptions.Compiled);
                var matches = regExp.Matches(card.Track2);
                if (matches.Count == 0)
                {
                    informations.Add($"Can't find service code: {card.Track2}");
                }
                else
                {
                    var date = matches.FirstOrDefault().Value.Substring(7);
                    switch (date)
                    {
                        case "1":
                            informations.Add($"Card: {card.Track2} authentication method: {AuthenticationMethod.LackOfRestriction}");
                            break;
                        case "6":
                            informations.Add($"Card: {card.Track2} authentication method: {AuthenticationMethod.PinRequired}");
                            break;

                        default:
                            break;
                    }
                }
            }
            return informations;
        }


        public IList<string> ValidateRangesIntervalAndOverlap(List<RangeOverlapValidatorInput> input)
        {
            IList<string> validationEvents = new List<string>();
            var rangeOverlapValidatorInputs = input.OrderBy(x => x.aEnd).ToList();
            for (int i = 0; i < rangeOverlapValidatorInputs.Count() - 1; i++)
            {
                if(rangeOverlapValidatorInputs[i].aEnd <= rangeOverlapValidatorInputs[i].aStart)
                {
                    validationEvents.Add($"Not proper interval: {rangeOverlapValidatorInputs[i].aStart} - {rangeOverlapValidatorInputs[i].aEnd}");
                }
                if (rangeOverlapValidatorInputs[i].aStart >= rangeOverlapValidatorInputs[i + 1].aStart || rangeOverlapValidatorInputs[i].aEnd >= rangeOverlapValidatorInputs[i + 1].aStart)
                {
                    validationEvents.Add($"Interval overlapping between: {rangeOverlapValidatorInputs[i].aStart}-{rangeOverlapValidatorInputs[i].aEnd} and " +
                        $"{rangeOverlapValidatorInputs[i + 1].aStart}-{rangeOverlapValidatorInputs[i + 1].aEnd}");
                    return validationEvents;

                }
            }
            return validationEvents;
        }


        public IList<string> JsonContentValidationRanges(string jobFileName)
        {
            IList<string> validationEvents = new List<string>();
            string jsonString;
            JObject jsonObject;
            try
            {
                jsonString = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jobFileName}.json");
                jsonObject = JObject.Parse(jsonString);

            }
            catch (Exception)
            {
                validationEvents.Add($"Wrong file name or format: {jobFileName}");
                return validationEvents;
            }
            JSchema schema = JSchema.Parse(System.IO.File.ReadAllText($"{mainPath}\\Schemas\\JsonSchemaRanges.json"));
            jsonObject.IsValid(schema, out validationEvents);
            return validationEvents;
        }


        public Tuple<IList<string>,IList<string>> JsonRangeAndCardsValidationWithLogginToFile(string jsonCardsfileName, string jsonRagesfileName, string fileLogName)
        {   
            using StreamWriter log = new($"{mainPath}\\Logs\\{fileLogName}.txt");
            IList<string> validationEvents = new List<string>();
            IList<string> validationInformations = new List<string>();
            log.WriteLine("Start validation/verfication process");
            log.WriteLine("1). Remove new line character from track2 value if exists");
            var jsonTrack2Enter = JsonTrack2EnterCut(jsonCardsfileName);
            if (jsonTrack2Enter.Count > 0)
            {
                log.WriteLine("1). Validation failed");
                log.WriteLine("Following errors occured:");
                foreach (var validationEvent in jsonTrack2Enter)
                {
                    log.WriteLine(validationEvent);
                    validationEvents.Add(validationEvent);
                }
                return Tuple.Create(validationEvents, validationInformations);
            }

            log.WriteLine("1). Proper removal of all new line character from track2 value");
            log.WriteLine("2). JsonCards content validation started");
            var jsonCardValidation = JsonContentValidationCards(jsonCardsfileName);
            if (jsonCardValidation.Count > 0)
            {
                log.WriteLine("2). JsonCards content validation failed");
                log.WriteLine("Following errors occured:");
                foreach (var validationEvent in jsonCardValidation)
                {
                    log.WriteLine(validationEvent);
                    validationEvents.Add(validationEvent);
                }
                return Tuple.Create(validationEvents, validationInformations);
            }

            log.WriteLine("2). JsonCards content validation sucessfull");
            log.WriteLine("3). JsonRanges content validation started");
            var jsonRangesValidation = JsonContentValidationRanges(jsonRagesfileName);
            if (jsonRangesValidation.Count > 0)
            {
                log.WriteLine("3). JsonRanges content validation failed");
                log.WriteLine("Following errors occured:");
                foreach (var validationEvent in jsonRangesValidation)
                {
                    log.WriteLine(validationEvent);
                    validationEvents.Add(validationEvent);
                }
                return Tuple.Create(validationEvents, validationInformations);
            }

            log.WriteLine("3). JsonRanges content validation sucessfull");
            log.WriteLine("4). Ranges interval validation started");

            var rangesInput = new List<RangeOverlapValidatorInput>();
            var ranges = new RootRanges();
            try
            {
                var content = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jsonRagesfileName}.json");
                ranges = JsonSerializer.Deserialize<RootRanges>(content);

            }
            catch (Exception)
            {
                log.WriteLine("4). Ranges interval validation failed due to wrong file format");
                validationEvents.Add($"Wrong file name or format: {jsonRagesfileName}");
                return Tuple.Create(validationEvents, validationInformations);
            }
            if (ranges != null)
            {
                foreach (var range in ranges.ranges)
                {
                    long fromCast = 0;
                    long toCast = 0;
                    try
                    {
                        long.TryParse(range.from, out fromCast);
                        long.TryParse(range.to, out toCast);

                    }
                    catch (Exception ex)
                    {
                        log.WriteLine("4). Ranges interval validation failed due to not correct casting string to long");
                        validationEvents.Add($"During ranges values casting follwoing exception has been found: {ex} ");
                        return Tuple.Create(validationEvents, validationInformations);

                    }
                    rangesInput.Add(new RangeOverlapValidatorInput
                    {
                        aStart = fromCast,
                        aEnd = toCast
                    });
                }
                var rangesValidation = ValidateRangesIntervalAndOverlap(rangesInput);
                if (rangesValidation.Count > 0)
                {
                    log.WriteLine("4). Ranges interval validation failed");
                    log.WriteLine("Following errors occured:");
                    foreach (var validationEvent in rangesValidation)
                    {
                        log.WriteLine(validationEvent);
                        validationEvents.Add(validationEvent);
                    }
                    return Tuple.Create(validationEvents, validationInformations);
                }
            }

            log.WriteLine("4). Ranges interval validation successful");
            log.WriteLine("5). PAN validation started");
            var panValidation = PANValidation(jsonCardsfileName);
            if (panValidation.Item1.Count > 0)
            {
                log.WriteLine("5). PAN validation failed");
                log.WriteLine("Following errors occured:");
                foreach (var validationEvent in panValidation.Item1)
                {
                    log.WriteLine(validationEvent);
                    validationEvents.Add(validationEvent);
                }
                return Tuple.Create(validationEvents, validationInformations);
            }
            if (panValidation.Item2.Count > 0)
            {
                log.WriteLine("PAN:");
                validationInformations.Add("PAN checked:");
                foreach (var validationInformation in panValidation.Item2)
                {
                    log.WriteLine(validationInformation);
                    validationInformations.Add(validationInformation);
                }
            }
            log.WriteLine("5). PAN validation ended");
            log.WriteLine("6). Expiry date verification started");
            var expirationValidation = IsExpired(jsonCardsfileName);
            if (expirationValidation.Count > 0)
            {
                log.WriteLine("Following card are expired:");
                validationInformations.Add("Following card are expired:");
                foreach (var validationInformation in expirationValidation)
                {
                    log.WriteLine(validationInformation);
                    validationInformations.Add(validationInformation);
                }
            }
            log.WriteLine("6). Expiry date verification ended");
            log.WriteLine("7). Card authentication method verification started");
            var authMethodCheck = AuthMethod(jsonCardsfileName);
            if (authMethodCheck.Count > 0)
            {
                log.WriteLine("Card authentiaction method checked:");
                validationInformations.Add("Card authentiaction method checked:");
                foreach (var validationInformation in authMethodCheck)
                {
                    log.WriteLine(validationInformation);
                    validationInformations.Add(validationInformation);
                }
            }
            log.WriteLine("7). Card authentication method verification ended");
            log.WriteLine("7). Card authentication method verification ended");
            log.WriteLine("8). Card matching stated");

            var cardMatching = new Matching().MatchCardToRange(jsonCardsfileName, jsonRagesfileName);
            if (cardMatching.Item1.Count > 0)
            {
                log.WriteLine("8). Matching failed");
                log.WriteLine("Following errors occured:");
                foreach (var validationEvent in cardMatching.Item1)
                {
                    log.WriteLine(validationEvent);
                    validationEvents.Add(validationEvent);
                }
                return Tuple.Create(validationEvents, validationInformations);
            }
            if (cardMatching.Item2.Count > 0)
            {
                validationInformations.Add("8). Matching done:");
                foreach (var validationInformation in cardMatching.Item2)
                {
                    log.WriteLine(validationInformation);
                    validationInformations.Add(validationInformation);
                }
            }
            log.WriteLine("Ended validation/verfication process");
            return Tuple.Create(validationEvents, validationInformations);
        }
    }
}
