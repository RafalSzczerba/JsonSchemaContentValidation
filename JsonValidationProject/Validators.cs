using JsonValidationProject.Contracts;
using JsonValidationProject.Enums;
using JsonValidationProject.Model;
using JsonValidationProject.Model.Cards;
using JsonValidationProject.Model.DTOs;
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


        public Tuple<IList<string>, IList<string>> PANValidation(string jobFileName)
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


        public IList<string> IsExpired(string jobFileName)
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


        public IList<string> AuthMethod(string jobFileName)
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
    }
}
