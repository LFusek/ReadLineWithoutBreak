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
					HandleBackspace(c.Modifiers, startLeft, ref endLeft, output);
				}
				else if (c.Key == ConsoleKey.Delete)
				{
					HandleDelete(c.Modifiers, startLeft, ref endLeft, output);
				}
				else if (!char.IsControl(c.KeyChar))
				{
					var charsToShift = output.ToString(Console.CursorLeft - startLeft, endLeft - Console.CursorLeft);

					output.Insert(Console.CursorLeft - startLeft, c.KeyChar);

					Console.Write(c.KeyChar);
					var currentLeft = Console.CursorLeft;
					Console.Write(charsToShift);
					Console.CursorLeft = currentLeft;

					endLeft++;
				}

#if DEBUG
				DummyLogger.WriteLine($"KEY: {c.Key,-10} | CTRL: {c.Modifiers.HasFlag(ConsoleModifiers.Control).ToString().ToLowerInvariant(),-6} | SHIFT: {c.Modifiers.HasFlag(ConsoleModifiers.Shift).ToString().ToLowerInvariant(),-6} | START: {startLeft,-3} | END: {endLeft,-3} | RET: \"{output}\"");
#endif
			}

			return output.ToString();
		}

		private static void HandleBackspace(ConsoleModifiers modifiers, int startLeft, ref int endLeft, StringBuilder output)
		{
			if (startLeft >= Console.CursorLeft) return;

			var lengthOfCharsOnLeft = Console.CursorLeft - startLeft;
			var lengthOfCharsOnRight = endLeft - Console.CursorLeft;
			string charsToShiftAndCleanWith;

			if (!modifiers.HasFlag(ConsoleModifiers.Control))
			{
				if (endLeft == Console.CursorLeft)
				{
					charsToShiftAndCleanWith = "\0";
				}
				else
				{
					charsToShiftAndCleanWith = output.ToString(lengthOfCharsOnLeft, lengthOfCharsOnRight);
					charsToShiftAndCleanWith = charsToShiftAndCleanWith + '\0';
				}

				output.Remove(lengthOfCharsOnLeft - 1, 1);

				var currentLeft = --Console.CursorLeft;
				Console.Write(charsToShiftAndCleanWith);
				Console.CursorLeft = currentLeft;

				endLeft--;
			}
			else
			{
				int charGroupLength;

				if (endLeft == Console.CursorLeft)
				{
					charGroupLength = GetCharGroupLength(output.ToString(), false);
					charsToShiftAndCleanWith = new string('\0', charGroupLength);
				}
				else
				{
					charGroupLength = GetCharGroupLength(output.ToString(0, lengthOfCharsOnLeft), false);
					charsToShiftAndCleanWith = output.ToString(lengthOfCharsOnLeft, lengthOfCharsOnRight);
					charsToShiftAndCleanWith = charsToShiftAndCleanWith + new string('\0', charGroupLength);
				}

				output.Remove(lengthOfCharsOnLeft - charGroupLength, charGroupLength);

				Console.CursorLeft -= charGroupLength;
				var currentLeft = Console.CursorLeft;
				Console.Write(charsToShiftAndCleanWith);
				Console.CursorLeft = currentLeft;

				endLeft -= charGroupLength;
			}
		}

		private static void HandleDelete(ConsoleModifiers modifiers, int startLeft, ref int endLeft, StringBuilder output)
		{
			if (endLeft <= Console.CursorLeft) return;

			var lengthOfCharsOnRight = endLeft - Console.CursorLeft;

			if (!modifiers.HasFlag(ConsoleModifiers.Control))
			{
				var lengthOfCharsOnLeft = Console.CursorLeft - startLeft;
				var charsToShiftAndCleanWith = output.ToString(lengthOfCharsOnLeft + 1, lengthOfCharsOnRight - 1);
				charsToShiftAndCleanWith = charsToShiftAndCleanWith + '\0';

				output.Remove(lengthOfCharsOnLeft, 1);

				var currentLeft = Console.CursorLeft;
				Console.Write(charsToShiftAndCleanWith);
				Console.CursorLeft = currentLeft;

				endLeft--;
			}
			else
			{
				var charsToCleanWith = new string('\0', lengthOfCharsOnRight);

				output.Remove(Console.CursorLeft - startLeft, lengthOfCharsOnRight);

				var currentLeft = Console.CursorLeft;
				Console.Write(charsToCleanWith);
				Console.CursorLeft = currentLeft;

				endLeft -= lengthOfCharsOnRight;
			}
		}

		private static int GetCharGroupLength(string text, bool rightDirection)
		{
			if (text.Length == 0) return 0;

			int iterationStep;
			int start;
			int end;

			if (rightDirection)
			{
				iterationStep = 1;
				start = 0;
				end = text.Length;
			}
			else
			{
				iterationStep = -1;
				start = text.Length - 1;
				end = -1;
			}

			if (start == end) return 1;

			var nonSeparatorCharMet = false;
			var groupLength = 0;

			for (var i = start; i != end; i += iterationStep)
			{
				if (!nonSeparatorCharMet)
				{
					nonSeparatorCharMet = !char.IsSeparator(text[i]);
					groupLength++;
					continue;
				}

				if (char.IsSeparator(text[i]))
				{
					return groupLength;
				}

				groupLength++;
			}

			return groupLength;
		}
	}
}
