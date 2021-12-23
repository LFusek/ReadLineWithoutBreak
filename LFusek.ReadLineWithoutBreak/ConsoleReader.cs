using System;
using System.Text;

namespace LFusek.ReadLineWithoutBreak
{
	public static class ConsoleReader
	{
		public static string ReadLineWithoutBreak()
		{
			var startLeft = Console.CursorLeft;
			var endLeft = startLeft;
			var output = new StringBuilder();

			while (true)
			{
				var c = Console.ReadKey(true);
				if (c.Key == ConsoleKey.Enter)
				{
					break;
				}
				else if (c.Key == ConsoleKey.LeftWindows || c.Key == ConsoleKey.RightWindows) { }
				else if (c.Key == ConsoleKey.LeftArrow)
				{
					if (startLeft < Console.CursorLeft) Console.CursorLeft--;
				}
				else if (c.Key == ConsoleKey.RightArrow)
				{
					if (endLeft > Console.CursorLeft) Console.CursorLeft++;
				}
				else if (c.Key == ConsoleKey.UpArrow)
				{
					Console.CursorLeft = startLeft;
				}
				else if (c.Key == ConsoleKey.DownArrow)
				{
					Console.CursorLeft = endLeft;
				}
				else if (c.Key == ConsoleKey.Backspace)
				{
					if (startLeft < Console.CursorLeft)
					{
						if (!c.Modifiers.HasFlag(ConsoleModifiers.Control))
						{
							string charsToMoveAndCleanWith;

							if (endLeft == Console.CursorLeft)
							{
								charsToMoveAndCleanWith = "\0";
							}
							else
							{
								charsToMoveAndCleanWith = output.ToString(Console.CursorLeft - startLeft, endLeft - Console.CursorLeft);
								charsToMoveAndCleanWith = charsToMoveAndCleanWith + '\0';
							}

							output.Remove(Console.CursorLeft - startLeft - 1, 1);

							var currentLeft = --Console.CursorLeft;
							Console.Write(charsToMoveAndCleanWith);
							Console.CursorLeft = currentLeft;

							endLeft--;
						}
						else
						{
							var zeroBasedCurrentLeft = Console.CursorLeft - startLeft;
							string charsToMoveAndCleanWith;

							if (endLeft == Console.CursorLeft)
							{
								charsToMoveAndCleanWith = new string('\0', output.Length);
							}
							else
							{
								charsToMoveAndCleanWith = output.ToString(zeroBasedCurrentLeft, endLeft - Console.CursorLeft);
								charsToMoveAndCleanWith = charsToMoveAndCleanWith.PadRight(output.Length, '\0');
							}

							output.Remove(0, zeroBasedCurrentLeft);

							Console.CursorLeft = startLeft;
							Console.Write(charsToMoveAndCleanWith);
							Console.CursorLeft = startLeft;

							endLeft -= zeroBasedCurrentLeft;
						}
					}
				}
				else if (c.Key == ConsoleKey.Delete)
				{
					if (endLeft > Console.CursorLeft)
					{
						if (!c.Modifiers.HasFlag(ConsoleModifiers.Control))
						{
							var zeroBasedCurrentLeft = Console.CursorLeft - startLeft;
							var charsToMoveAndCleanWith = output.ToString(zeroBasedCurrentLeft + 1, endLeft - Console.CursorLeft - 1);
							charsToMoveAndCleanWith = charsToMoveAndCleanWith + '\0';

							output.Remove(zeroBasedCurrentLeft, 1);

							var currentLeft = Console.CursorLeft;
							Console.Write(charsToMoveAndCleanWith);
							Console.CursorLeft = currentLeft;

							endLeft--;
						}
						else
						{
							var lengthOfDeletedChars = endLeft - Console.CursorLeft;
							var charsToCleanWith = new string('\0', lengthOfDeletedChars);

							output.Remove(Console.CursorLeft - startLeft, lengthOfDeletedChars);

							var currentLeft = Console.CursorLeft;
							Console.Write(charsToCleanWith);
							Console.CursorLeft = currentLeft;

							endLeft -= lengthOfDeletedChars;
						}
					}
				}
				else
				{
					var charsToMove = output.ToString(Console.CursorLeft - startLeft, endLeft - Console.CursorLeft);

					output.Insert(Console.CursorLeft - startLeft, c.KeyChar);

					Console.Write(c.KeyChar);
					var currentLeft = Console.CursorLeft;
					Console.Write(charsToMove);
					Console.CursorLeft = currentLeft;

					endLeft++;
				}

#if DEBUG
				DummyLogger.WriteLine($"KEY: {c.Key,-10} | CTRL: {c.Modifiers.HasFlag(ConsoleModifiers.Control).ToString().ToLowerInvariant(),-6} | SHIFT: {c.Modifiers.HasFlag(ConsoleModifiers.Shift).ToString().ToLowerInvariant(),-6} | START: {startLeft,-3} | END: {endLeft,-3} | RET: \"{output}\"");
#endif
			}

			return output.ToString();
		}
	}
}
