using JsonValidationProject.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonValidationProject
{
    public class Execution : IExecution
    {
        public void FirstJob()
        {
            Console.WriteLine("JsonSchema Validation. Task 1");
            Console.WriteLine();
            Console.WriteLine("Intruduce the name of the file which you want to check without extension");
            var fileName = Console.ReadLine();
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
        public void SecondJob()
        {
            Console.WriteLine("JsonSchema Validation. Task 2");
            Console.WriteLine();
            Console.WriteLine("Intruduce the name of the file which you want to check without extension");
            var fileName = Console.ReadLine();
            Console.Clear();

            if (fileName == null || fileName == "")
            {
                Console.WriteLine("Wrong file name");
                Console.WriteLine("End of program");
                return;
            }
            var obj = new Validators();
            Console.WriteLine("Starting Validation - Json Ranges");
            Console.WriteLine("1-stage. Start JsonContent validation");
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
        public void ThirdJob()
        {
            Console.WriteLine("JsonSchema Validation. Task 3");
            Console.WriteLine();
            Console.WriteLine("Intruduce the name of the file which you want to check without extension [ for Json Cards job]");
            string? firstFileName = Console.ReadLine();
            Console.WriteLine("Intruduce the name of the file which you want to check without extension [ for Json Ranges job]");
            string? secondFileName = Console.ReadLine();
            Console.WriteLine("Intruduce the name of the log file");
            string? logFileName = Console.ReadLine();
            Console.Clear();
            if (firstFileName == null || firstFileName == ""
                || secondFileName == null || secondFileName == ""
                || logFileName == null || logFileName == ""
                )
            {
                Console.WriteLine("Wrong file name");
                Console.WriteLine("End of program");
                return;
            }
            var obj = new Validators().JsonRangeAndCardsValidationWithLogginToFile(firstFileName, secondFileName, logFileName);
            Console.WriteLine("Starting Validation");
            if (obj.Item1.Count > 0)
            {
                if(obj.Item2.Count > 0)
                {
                    Console.WriteLine("Folowing information has been logged:");
                    foreach (var validationEvent in obj.Item2)
                    {
                        Console.WriteLine(validationEvent);
                    }
                }                
                Console.WriteLine();                
                Console.WriteLine("Following errors has been found:");
                Console.WriteLine();
                foreach (var validationEvent in obj.Item1)
                {
                    Console.WriteLine(validationEvent);
                }
                Console.WriteLine("End of validation: failed");
                return;
            }
            else
            {
                if (obj.Item2.Count > 0)
                {
                    Console.WriteLine("Folowing information has been logged:");
                    foreach (var validationEvent in obj.Item2)
                    {
                        Console.WriteLine(validationEvent);
                    }
                }
                Console.WriteLine($"End of validation: successful. Check log file under this path- Logs\\{logFileName}.txt");
                Console.WriteLine();
            }
        }
    }
}
