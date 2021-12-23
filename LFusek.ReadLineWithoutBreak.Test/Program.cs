using LFusek.ReadLineWithoutBreak;

Console.Write("Message: ");
var message = ConsoleReader.ReadLineWithoutBreak();

Console.WriteLine($"\nEntered input: \"{message}\"");
Console.ReadLine();
