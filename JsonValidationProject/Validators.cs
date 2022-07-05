using JsonValidationProject.Model.Cards;
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
    public class Validators
    {
        public IList<string> JsonTrack2EnterCut(string jobFileName)
        {           
            const string quote = "\"";
            string searchText = "\"track2\": \";\r\n";
            string desiredText = "\"track2\": \";";
            string textFile;
            IList<string> validationEvents = new List<string>();

            try
            {
                 textFile = File.ReadAllText($"{jobFileName}.txt");

            }
            catch (Exception)
            {
                validationEvents.Add($"Wrong file name: {jobFileName}");
                return validationEvents;
            }
            textFile = File.ReadAllText($"{jobFileName}.txt");
            textFile = textFile.Replace(searchText, desiredText);
            File.WriteAllText($"{jobFileName}.json", textFile);
            return validationEvents;
        }


        public IList<string> JsonContentValidationCards(string jobFileName)
        {
            IList<string> validationEvents = new List<string>();
            string jsonString;
            JObject jsonObject;
            try
            {
                 jsonString = System.IO.File.ReadAllText($"{jobFileName}.json");
                 jsonObject = JObject.Parse(jsonString);


            }
            catch (Exception)
            {

                validationEvents.Add("Wrong file format");
                return validationEvents;
            }
            JSchema schema = JSchema.Parse(System.IO.File.ReadAllText($"JsonSchemaCards.json"));
            jsonObject.IsValid(schema, out validationEvents);
            return validationEvents;
        }
        public IList<string> Track2Validation(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{jobFileName}.json");
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
                //var regExp = new Regex(@"^[;][0-9]{1,19}[=]{1}[0-9]*[?]$", RegexOptions.Compiled);
                var regExp = new Regex(@"^\;([0-9]{16,19})\=([0-9]{4}|\=)([0-9]{3}|\=)([^\?]+)\?$", RegexOptions.Compiled);

                var matches = regExp.Matches(card.Track2);
                if (matches.Count == 0)
                {
                    errors.Add($"Wrong track2: {card.Track2}");
                }
            }

            return errors;
        }


        public IList<string> ZipCodeValidation(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{jobFileName}.json");
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
                var regExp = new Regex(@"^[0-9]{2}[-]?[0-9]{3}$", RegexOptions.Compiled);
                var matches = regExp.Matches(card.Address.ZipCode);
                if (matches.Count == 0)
                {
                    errors.Add($"Wrong zip code: {card.Address.ZipCode}");
                }

            }
            return errors;
        }
        public IList<string> PANValidation(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{jobFileName}.json");
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
                var regExp = new Regex(@"^\;[0-9]{5,19}\=", RegexOptions.Compiled);
                var matches = regExp.Matches(card.Track2);
                if (matches.Count == 0)
                {
                    errors.Add($"Wrong PAN in: {card.Track2}");
                }

            }
            return errors;
        }
        public IList<string> IsExpired(string jobFileName)
        {
            String content = System.IO.File.ReadAllText($"{jobFileName}.json");
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
        public IList<string> JsonContentValidationRanges(string jobFileName)
        {
            IList<string> validationEvents = new List<string>();
            string jsonString;
            JObject jsonObject;
            try
            {
                jsonString = System.IO.File.ReadAllText($"{jobFileName}.json");
                jsonObject = JObject.Parse(jsonString);

            }
            catch (Exception)
            {

                validationEvents.Add($"Wrong file name or format: {jobFileName}");
                return validationEvents;
            }
            JSchema schema = JSchema.Parse(System.IO.File.ReadAllText($"JsonSchemaRanges.json"));
            jsonObject.IsValid(schema, out validationEvents);
            return validationEvents;
        }
        public IList<string> JsonRangeAndCardsValidationWithLogginToFile(string jsonCardsfileName, string jsonRagesfileName, string fileLogName)
        {
            IList<string> validationEvents = new List<string>();
            var jsonTrack2Enter = JsonTrack2EnterCut(jsonCardsfileName);
            if (jsonTrack2Enter.Count > 0)
            {
                foreach (var validationEvent in jsonTrack2Enter)
                {
                    validationEvents.Add(validationEvent);
                }
                return validationEvents;
            }

            var jsonCardValidation = JsonContentValidationCards(jsonCardsfileName);
            if (jsonCardValidation.Count > 0)
            {
                foreach (var validationEvent in jsonCardValidation)
                {
                    validationEvents.Add(validationEvent);
                }
                return validationEvents;
            }

            var track2Validation = Track2Validation(jsonCardsfileName);
            if (track2Validation.Count > 0)
            {
                foreach (var validationEvent in track2Validation)
                {
                    validationEvents.Add(validationEvent);
                }
            }

            var panValidation = PANValidation(jsonCardsfileName);
            if (panValidation.Count > 0)
            {
                foreach (var validationEvent in panValidation)
                {
                    validationEvents.Add(validationEvent);
                }
            }

            var expirationValidation = IsExpired(jsonCardsfileName);
            if (expirationValidation.Count > 0)
            {
                foreach (var validationEvent in expirationValidation)
                {
                    validationEvents.Add(validationEvent);
                }
            }

            var zipCodeValidation = ZipCodeValidation(jsonCardsfileName);
            if (zipCodeValidation.Count > 0)
            {
                foreach (var validationEvent in zipCodeValidation)
                {
                    validationEvents.Add(validationEvent);
                }
            }

            var rangesContentValidation = JsonContentValidationRanges(jsonRagesfileName);
            if (rangesContentValidation.Count>0)
            {
                foreach (var validationEvent in rangesContentValidation)
                {
                    validationEvents.Add(validationEvent);
                }
            }            
            return validationEvents;
        }
    }
}
