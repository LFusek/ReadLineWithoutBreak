using LFusek.ReadLineWithoutBreak;

Console.Write("Message: ");
var message = ConsoleReader.ReadLineWithoutBreak();

Console.Write($"\nEntered input: \"{message}\"");
Console.ReadLine();
