using JsonValidationProject.Contracts;
using JsonValidationProject.Model;
using JsonValidationProject.Model.Ranges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonValidationProject
{
    public class Execution : IExecution
    {
        private readonly string mainPath;

        public Execution()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            mainPath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
        }

        public void FirstJob(string fileName)
        {
            Console.WriteLine("JsonSchema Validation. Task 1");
            Console.WriteLine();
            Console.Clear();

            if (fileName == null || fileName == "")
            {
                Console.WriteLine("Wrong file name");
                Console.WriteLine("End of program");
                return;
            }

            var obj = new Validators();
            var jsonTrack2Enter = obj.JsonTrack2EnterCut(fileName);
            if (jsonTrack2Enter.Count>0)
            {
                Console.WriteLine("Following errors has been found:");
                Console.WriteLine();
                foreach (var validationEvent in jsonTrack2Enter)
                {
                    Console.WriteLine(validationEvent);
                }
                Console.WriteLine("End of validation: failed");
                return;
            }

            Console.WriteLine("Starting Validation - Json Cards");
            Console.WriteLine("1-stage. Start JsonContent validation");
            var firstStageResult = obj.JsonContentValidationCards(fileName);
            if (firstStageResult.Count > 0)
            {
                Console.WriteLine("Following errors in JsonSchema has been found:");
                Console.WriteLine();
                foreach (var validationEvent in firstStageResult)
                {
                    Console.WriteLine(validationEvent);
                }
                Console.WriteLine("End of validation: failed");
                return;
            }
            else
            {
                Console.WriteLine("1-stage validation successful. Json structure , first name, last name and city string length has been checked.");
                Console.WriteLine();
            }

            Console.WriteLine("2-stage. Starting the track2 validation");

            var secondStageResult = obj.Track2Validation(fileName);
            if (secondStageResult.Count > 0)
            {
                Console.WriteLine("Following errors in for track2 has been found:");
                Console.WriteLine();
                foreach (var validationEvent in secondStageResult)
                {
                    Console.WriteLine(validationEvent);
                }
                Console.WriteLine("End of validation: failed");
                return;
            }
            else
            {
                Console.WriteLine("2-stage validation successful: track2 validation successful");
                Console.WriteLine();

            }
            Console.WriteLine("3-stage. Starting the zipCode validation");
            var zipCodeValidationResult = obj.ZipCodeValidation(fileName);
            if (zipCodeValidationResult.Count > 0)
            {
                Console.WriteLine("Following errors in for zipCode has been found:");
                Console.WriteLine();
                foreach (var validationEvent in zipCodeValidationResult)
                {
                    Console.WriteLine(validationEvent);
                }
                Console.WriteLine("End of validation: failed");
                return;
            }
            else
            {
                Console.WriteLine("3-stage validation successful: zipCode validation successful");
                Console.WriteLine("End of validation: successful");
            }
        }


        public void SecondJob(string fileName)
        {
            Console.WriteLine("JsonSchema Validation. Task 2");
            Console.WriteLine();
            Console.Clear();

            if (fileName == null || fileName == "")
            {
                Console.WriteLine("Wrong file name");
                Console.WriteLine("End of program");
                return;
            }
            var obj = new Validators();
            Console.WriteLine("Starting Validation - Json Ranges");
            Console.WriteLine("Start JsonContent validation");
            var firstStageResult = obj.JsonContentValidationRanges(fileName);
            if (firstStageResult.Count > 0)
            {
                Console.WriteLine("Following errors in JsonSchema has been found:");
                Console.WriteLine();
                foreach (var validationEvent in firstStageResult)
                {
                    Console.WriteLine(validationEvent);
                }
                Console.WriteLine("End of validation: failed");
                return;
            }
            else
            {
                Console.WriteLine("Json structure has been checked.");
                Console.WriteLine("End of validation: successful");
                Console.WriteLine();
            }            
        }


        public void ThirdJob(string jsonCardsfileName,string jsonRagesfileName,string fileLogName)
        {
            using StreamWriter data = new($"{mainPath}\\Logs\\{fileLogName}.txt");
            Console.WriteLine("JsonSchema Validation. Task 3");
            Console.WriteLine();            
            if (jsonCardsfileName == null || jsonCardsfileName == ""
                || jsonRagesfileName == null || jsonRagesfileName == ""
                || fileLogName == null || fileLogName == ""
                )
            {
                Console.WriteLine("Wrong file name");
                Console.WriteLine("End of program");
                return;
            }
            var validators = new Validators();
            Console.WriteLine("1). Start removal of new line character from track2 value if exists");
            data.WriteLine("1). Start removal of new line character from track2 value if exists");
            var jsonTrack2Enter = validators.JsonTrack2EnterCut(jsonCardsfileName);
            if (jsonTrack2Enter.Count > 0)
            {
                data.WriteLine("1). Validation failed");
                data.WriteLine("Following errors occured:");
                foreach (var validationEvent in jsonTrack2Enter)
                {
                    data.WriteLine(validationEvent);
                    Console.WriteLine(validationEvent);
                }
                return;
            }
            Console.WriteLine("1). End removal of new line character from track2 value");
            Console.WriteLine("2). JsonCards content validation started");
            data.WriteLine("1). End removal of new line character from track2 value");
            data.WriteLine("2). JsonCards content validation started");
            var jsonCardValidation = validators.JsonContentValidationCards(jsonCardsfileName);
            if (jsonCardValidation.Count > 0)
            {
                Console.WriteLine("2). JsonCards content validation failed");
                Console.WriteLine("Following errors occured:");
                data.WriteLine("2). JsonCards content validation failed");
                data.WriteLine("Following errors occured:");
                foreach (var validationEvent in jsonCardValidation)
                {
                    data.WriteLine(validationEvent);
                    Console.WriteLine(validationEvent);
                }
                return;
            }
            Console.WriteLine("2). JsonCards content validation sucessfull");
            Console.WriteLine("3). JsonRanges content validation started");
            data.WriteLine("2). JsonCards content validation sucessfull");
            data.WriteLine("3). JsonRanges content validation started");
            var jsonRangesValidation = validators.JsonContentValidationRanges(jsonRagesfileName);
            if (jsonRangesValidation.Count > 0)
            {
                Console.WriteLine("3). JsonRanges content validation failed");
                Console.WriteLine("Following errors occured:");
                data.WriteLine("3). JsonRanges content validation failed");
                data.WriteLine("Following errors occured:");
                foreach (var validationEvent in jsonRangesValidation)
                {
                    data.WriteLine(validationEvent);
                    Console.WriteLine(validationEvent);
                }
                return;
            }
            Console.WriteLine("3). JsonRanges content validation sucessfull");
            Console.WriteLine("4). Ranges interval validation started");
            data.WriteLine("3). JsonRanges content validation sucessfull");
            data.WriteLine("4). Ranges interval validation started");
            var rangesInput = new List<RangeOverlapValidatorInput>();
            var ranges = new RootRanges();
            try
            {
                var content = System.IO.File.ReadAllText($"{mainPath}\\Source\\{jsonRagesfileName}.json");
                ranges = JsonSerializer.Deserialize<RootRanges>(content);

            }
            catch (Exception)
            {
                data.WriteLine("4). Ranges interval validation failed due to wrong file format");
                Console.WriteLine("4). Ranges interval validation failed due to wrong file format");
                return;
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
                        data.WriteLine("4). Ranges interval validation failed due to not correct casting string to long");
                        Console.WriteLine("4). Ranges interval validation failed due to not correct casting string to long");
                        return;

                    }
                    rangesInput.Add(new RangeOverlapValidatorInput
                    {
                        aStart = fromCast,
                        aEnd = toCast
                    });
                }
                var rangesValidation = validators.ValidateRangesIntervalAndOverlap(rangesInput);
                if (rangesValidation.Count > 0)
                {
                    data.WriteLine("4). Ranges interval validation failed");
                    data.WriteLine("Following errors occured:");
                    Console.WriteLine("4). Ranges interval validation failed");
                    Console.WriteLine("Following errors occured:");
                    foreach (var validationEvent in rangesValidation)
                    {
                        data.WriteLine(validationEvent);
                        Console.WriteLine(validationEvent);
                    }
                    return;
                }
            }
            Console.WriteLine("4). Ranges interval validation successful");
            Console.WriteLine("5). PAN validation started");
            data.WriteLine("4). Ranges interval validation successful");
            data.WriteLine("5). PAN validation started");
            var panValidation = validators.PANValidation(jsonCardsfileName);
            if (panValidation.Item1.Count > 0)
            {
                Console.WriteLine("5). PAN validation failed");
                Console.WriteLine("Following errors occured:");
                data.WriteLine("5). PAN validation failed");
                data.WriteLine("Following errors occured:");
                foreach (var validationEvent in panValidation.Item1)
                {
                    data.WriteLine(validationEvent);
                    Console.WriteLine(validationEvent);
                }
                return;
            }
            if (panValidation.Item2.Count > 0)
            {
                data.WriteLine("PAN:");
                Console.WriteLine("PAN:");
                foreach (var validationInformation in panValidation.Item2)
                {
                    data.WriteLine(validationInformation);
                    Console.WriteLine(validationInformation);
                }
            }      
            Console.WriteLine("5). PAN validation sucessfull");
            Console.WriteLine("6). Expiry date verification started");
            data.WriteLine("5). PAN validation sucessfull");
            data.WriteLine("6). Expiry date verification started");
            var expirationValidation = validators.IsExpired(jsonCardsfileName);
            if (expirationValidation.Count > 0)
            {
                data.WriteLine("Following card are expired:");
                Console.WriteLine("Following card are expired:");
                foreach (var validationInformation in expirationValidation)
                {
                    data.WriteLine(validationInformation);
                    Console.WriteLine(validationInformation);
                }
            }        
            Console.WriteLine("6). Expiry date verification ended");
            Console.WriteLine("7). Card authentication method verification started");
            data.WriteLine("6). Expiry date verification ended");
            data.WriteLine("7). Card authentication method verification started");
            var authMethodCheck = validators.AuthMethod(jsonCardsfileName);
            if (authMethodCheck.Count > 0)
            {
                data.WriteLine("Card authentiaction method checked:");
                Console.WriteLine("Card authentiaction method checked:");
                foreach (var validationInformation in authMethodCheck)
                {
                    data.WriteLine(validationInformation);
                    Console.WriteLine(validationInformation);
                }
            }          
            data.WriteLine("7). Card authentication method verification ended");
            data.WriteLine("8). Card matching stated");
            Console.WriteLine("7). Card authentication method verification ended");
            Console.WriteLine("8). Card matching stated");

            var cardMatching = new Matching().MatchCardToRange(jsonCardsfileName, jsonRagesfileName);
            if (cardMatching.Item1.Count > 0)
            {
                Console.WriteLine("8). Matching failed");
                Console.WriteLine("Following errors occured:");
                data.WriteLine("8). Matching failed");
                data.WriteLine("Following errors occured:");
                foreach (var validationEvent in cardMatching.Item1)
                {
                    data.WriteLine(validationEvent);
                    Console.WriteLine(validationEvent);
                }
                return;
            }
            if (cardMatching.Item2.Count > 0)
            {
                Console.WriteLine("8). Matching done:");
                data.WriteLine("8). Matching done:");

                foreach (var validationInformation in cardMatching.Item2)
                {
                    data.WriteLine(validationInformation);
                    Console.WriteLine(validationInformation);
                }
            }
            data.WriteLine("Ended validation/verfication process");
            Console.WriteLine("Ended validation/verfication process");          
        }
    }
}
