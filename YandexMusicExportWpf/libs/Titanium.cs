using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoNotUse
{
	/// <summary>
	/// <list type="bullet">
	/// <item><term>05.05.24</term> <description> <see langword="Function new"/> <see cref="Titanium.IO.GetAllFiles"/> </description></item>
	/// <item><term>04.05.24</term>  <description><see langword="!Function renamed"/> <see langword="Function new"/> <see cref="Titanium.TypesFuncs.ToDefault_IfNot"/> is now renamed and separated to <see cref="Titanium.TypesFuncs.ToNull_IfNotAny"/> with the same behaviour and <see cref="Titanium.TypesFuncs.ToDefault_IfNotAny"/> that actually returns default value</description></item>
	/// <item><term>26.11.23</term>  <description><see cref="Titanium.TypesFuncs.IndexOfT"/> (and its private mini-functions) changed: <see langword="!Parametr replaced"/> Replaced <see langword="bool"/> RightDirection with <see langword="!bool"/> InvertDirection</description></item>
	/// <item><term>24.11.23</term>  <description><see cref="Titanium.TypesFuncs.Slice"/> (and its overloads) changed: <see langword="!Behaviour"/> <see langword="!Parametr order"/> <see langword="!Parametr replaced"/> Replaced <see langword="bool"/> AlwaysReturnString to <see cref="Titanium.TypesFuncs.SliceReturn"/> SliceReturnSourceIfNotFound</description></item>
	/// <item><term>24.11.23</term>  <description>Added <see langword="enum"/> <see cref="Titanium.TypesFuncs.SliceReturn"/></description></item>
	/// <item><term> 24.11.23</term> <description> History started </description></item>
	/// </list>
	/// </summary>
	static class History
	{
		private const bool oldHistory = false;
	}
}

//TODO: make unit tests for all functions where it's possible



///Namespace contains library classes containing add-on to basic c# tools
namespace Titanium {
	/// <summary>
	/// Just my library of small functions that makes c# programming easier. Written from when I write "Hello Wrold" first time, so some code may be shitty, uncommented, buggy or not optimal.
	/// <para /> <see langword="WARNING"/>! It is NOT backward compatible, so don't update it after you started using it in your project. Some function may be deleted, reworked (parametr removal, renaming or even just order changing; behaviour changing) even if there's no serious need for it. There's no "that strange shitty костыльный spaghetti code is here because of historical reasons, for compatibility purposes" and never will be. If function become obsolete, it's usually gets immidiatly mercilessly removed. If I see that some change will make function more convinient, but change its behaviour I'll always do it – don't expect that after update all functions will work the same even if there's no syntax errors. Never update without reading changelog.
	/// <para /> This is comment to the namespace comment btw, becouse I can't make xml comment to the namespace
	/// <para> Despite of the other code license, THIS file is <see href="https://creativecommons.org/licenses/by-sa/4.0">CC BY-SA</see></para>
	/// <list type="table">
	/// <item>
	///		<term>Author</term>
	///		<see href="https://github.com/TuTAH1">Титан</see>
	/// </item>
	/// </list>
	/// </summary>   
	public static class TypesFuncs
	{
		#region Parsing

			#region IsType
			public static bool IsDigit(this char c) => c >= '0' && c <= '9';

			public static bool IsDoubleT(this char c) => (c >= '0' && c <= '9')||( c == '.' || c == ','||c=='-');
			#endregion

			#region ToType
				#region Int

				
					/// <summary>
					/// Преобразует число в char в Int
					/// </summary>
					/// <param name="str"></param>
					/// <returns>Число, если содержимое Char – число:
					/// -1 в противном случае</returns>
					public static int ToIntT(this char ch) =>
						ch is >= '0' and <= '9'?
							ch-'0' : -1;

					/// <summary>
					/// "534 Убирает все лишние символы и читает Int. 04" => 53404. Максимально оптимизирован, даже лучше стандартного
					/// </summary>
					/// <param name="str"></param>
					/// <param name="CanBeNegative">Может ли число быть отрицательным (если нет, то - будет игнорироваться, иначе он будет учитываться только если стоит рядом с числом)</param>
					/// <param name="StopIfNumberEnded">If true, stops parsing if int already found, but current symbol isn't a number</param>
					/// <param name="ExceptionValue">Возвращаемое значение при исключении (если не указывается, то при исключении... вызывается исключение</param>
					/// <returns>Числа, если они содержаться в строке</returns>
					public static int ToIntT(this string str, bool CanBeNegative = true, bool StopIfNumberEnded = false, int? ExceptionValue = null) //:16.05.2022 removed reduntant ischar check
					{
						int Number = 0;
						bool isContainsNumber = false;
						bool negative = false;
						foreach (var ch in str)
						{
							if (CanBeNegative&&!isContainsNumber) //:multiple ifs each cycle isn't affects perfomance; CanBeNegative не проверялось
							{
								if (ch == '-') negative = true; 
								else if (ch != ' ') negative = false;
							}
							if (ch.IsDigit())
							{
								isContainsNumber = true;
								if (Number > int.MaxValue / 10) return negative ? int.MinValue : int.MaxValue; //:none or positive perfomance effect
								//TODO: make options for ValueOutOfRange ↑ – exceptoin; custom value; min/max value (current); overflow (system default)
								Number = Number * 10 + (ch - '0'); 
							} else if(StopIfNumberEnded && isContainsNumber) break;
						}

						if (!isContainsNumber)
						{
							if (ExceptionValue == null)
							{
								throw new ArgumentException("Строка не содержит числа");
							}
							else return (int)ExceptionValue;
						}
						
						return negative? -Number:Number;
					}

					public static long ToLongT(this string str, bool CanBeNegative = true, bool StopIfNumberEnded = false, long? ExceptionValue = null)
					{
						long Number = 0;
						bool isContainsNumber = false;
						bool negative = false;
						foreach (var ch in str)
						{
							if (CanBeNegative&&!isContainsNumber) //:multiple ifs each cycle isn't affects perfomance; CanBeNegative не проверялось
							{
								if (ch == '-') negative = true; 
								else if (ch != ' ') negative = false;
							}
							if (ch.IsDigit())
							{
								isContainsNumber = true;
								if (Number > long.MaxValue / 10) return negative ? long.MinValue : long.MaxValue; //:none or positive perfomance effect
								//TODO: make options for ValueOutOfRange ↑ – exceptoin; custom value; min/max value (current); overflow (system default)
								Number = Number * 10 + (ch - '0'); 
							} else if(StopIfNumberEnded && isContainsNumber) break;
						}

						if (!isContainsNumber)
						{
							if (ExceptionValue == null)
							{
								throw new ArgumentException("Строка не содержит числа");
							}
							else return (long)ExceptionValue;
						}
						
						return negative? -Number:Number;
					}


					public static int? ToNIntT(this string str, bool CanBeNegative = true,  bool StopIfNumberEnded = false) //:08.10.2021 new func
					{
						try
						{
							return ToIntT(str, CanBeNegative,StopIfNumberEnded);
						}
						catch (Exception)
						{
							return null;
						}
					}

		
					/// <summary>
					/// Searches for INT in string. Throws IndexOutOfRangeException if no any int found in this string.
					/// </summary>
					/// <param name="str"></param>
					/// <param name="thousandSeparator"></param>
					/// <returns>first INT number found in string</returns>
					public static int FindInt(this string str, char thousandSeparator = ' ') //! legacy Obsolete
					{
						int end = 0, start = 0;
						bool minus = false;
						while (!str[start].IsDigit())
						{
							minus = str[start] == '-';
							start++;
						}

						end = minus ? start - 1 : start;

						do
						{
							end++;
						} while ((str[end].IsDigit() || str[end] == thousandSeparator) && end < str.Length - 1);

						return Convert.ToInt32(str.Substring(start,end-start).Replace(thousandSeparator.ToString(), ""), new CultureInfo("en-US", true));
					}
					public static int FindIntBetween(this string MainStr, string LeftStr, string RightStr = null)
					{
						if (RightStr == null) RightStr = LeftStr;
						int right = MainStr.IndexOf(RightStr),
							left = MainStr.LastIndexOf(LeftStr, 0, right - 1);
						int number;

						do
						{ //TODO: Сначала искать IndexOf Right, потом LastIndexOf Left, 
							if (int.TryParse(MainStr.Substring(left+1,right-(left+1)), out number)) return number;
							right = MainStr.IndexOf(RightStr);
							left = MainStr.LastIndexOf(LeftStr, left, (right - left) - 1);
						} while (left > 0 && right > 0);

						return -1;
					}

					#endregion

				#region String

				 //!17.06.2023

				/// <summary>
				/// Int to subscript numbers string
				/// </summary>
				/// <param name="Number">Number that should be converted to index</param>
				/// <param name="Lower">Lower index or upper?</param>
				/// <returns></returns>
				public static string ToIndex(this string Number, bool Lower = true) //: [19.11.2023] Added "Lower" parametr; added int overload
				{
																				
					string index = "";

					if (Lower)
						foreach (var t in Number)
						{
							switch (t)
							{
								case '1': index += Lower? '₁' : '¹'; break;
								case '2': index += Lower? '₂' : '²'; break;
								case '3': index += Lower? '₃' : '³'; break;
								case '4': index += Lower? '₄' : '⁴'; break;
								case '5': index += Lower? '₅' : '⁵'; break;
								case '6': index += Lower? '₆' : '⁶'; break;
								case '7': index += Lower? '₇' : '⁷'; break;
								case '8': index += Lower? '₈' : '⁸'; break;
								case '9': index += Lower? '₉' : '⁹'; break;
								case '0': index += Lower? '₀' : '⁰'; break;
							}
						}
					return index;
				}

				// <summary>
				/// Int to subscript numbers string
				/// </summary>
				/// <param name="Number">Number that should be converted to index</param>
				/// <param name="Lower">Lower index or upper?</param>
				/// <returns></returns>
				public static string ToIndex(this int Number, bool Lower = true) //: [19.11.2023] new
					=> ToIndex(Number.ToString(), Lower);   
				

				/// <summary>
				/// Removes all controls characters
				/// </summary>
				/// <param name="s"></param>
				/// <returns></returns>
				
				public static string ToVisibleString(this string s) //!03.10.2021
				{
					string a = "";
					foreach (char c in s)
					{
						if (!char.IsControl(c)) a += c;
					}

					return a;
				}
				

				/// <summary>
				/// Преобразовывает string в double, обрезая дробную часть до момента, когда начинаются нули
				/// </summary>
				/// <param name="Double"></param>
				/// <param name="Accuracy"></param>
				/// <returns></returns>
				public static string ToStringT(this double Double, int Accuracy = 3) //:10.10.2021 Created
				{
					var String = Double.ToString(CultureInfo.InvariantCulture); //: Пока я не умею преобразовывать double в String с минимально возможными затратами самостоятельно
					var temp = String.Split('.');
					if (temp.Length == 1)
					{
						return temp[0];
					}
					else
					{
						string intPart = temp[0], decPart = temp[1];
						int superfluity = decPart.IndexOf('0'.Multiply(Accuracy)); //TODO: вместо нулей можно пробегать по строке, пока символ не начнет повторятся несколько раз
						if (superfluity == -1) return String;

						decPart = decPart.Slice(0, superfluity);
						return intPart + "." + decPart;
					}

				}

				public static string ToStringT<T>(this IEnumerable<T> Array, string Separator = "") //:21.04.2022 Added separator parametr, replaced [foreach] by [for]
				{
					string s = "";
					var Count = Array.Count();
					for (int i = 0; i < Count; i++)
					{
						s += Array.ElementAt(i);
						if (i != Count - 1) s += Separator;
					}

					return s;
				}

				public static string ToStringT<TKey, TValue> (this IDictionary<TKey, TValue> Dictionary, bool Vertical = true, string ItemSeparator = default, string PairSeparator = "\n", string BeforePair = null, string AfterPair = null)
				{
					ItemSeparator??= Vertical? " = " : ", ";

					if(Vertical) return BeforePair + string.Join(PairSeparator, Dictionary.Select(kv => kv.Key + ItemSeparator + kv.Value).ToArray()) + AfterPair;
					else
					{
						string result = BeforePair;
						foreach (var key in Dictionary.Keys)
						{
							result += key + ItemSeparator;
						}

						result = result.Replace(-ItemSeparator.Length,null,AfterPair + PairSeparator + BeforePair);
						
						foreach (var value in Dictionary.Values)
						{
							result += value + ItemSeparator;
						}

						return result + AfterPair;
					}

				}


					#region Bytes

				/// <summary>
				/// Reads String and parses to Short until meets a letter
				/// </summary>
				/// <param name="str"></param>
				/// <returns></returns>

				public static string ToHex(this byte[] bytes)
				{
					char[] c = new char[bytes.Length * 2];

					byte b;

					for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
					{
						b = ((byte)(bytes[bx] >> 4));
						c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

						b = ((byte)(bytes[bx] & 0x0F));
						c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
					}

					return new string(c);
				}

				#endregion

				#endregion

				#region Double

				/// <summary>
				/// "534 Убирает все лишние символы и читает double. 0.4" => 5340.4
				/// </summary>
				/// <param name="str"></param>
				/// <param name="CanBeNegative"></param>
				/// <param name="StopIfNumberEnded"></param>
				/// <param name="CanBeShortcuted">Может ли нуль целой части быть опущен (".23" вместо "0.23")</param>
				/// <param name="DotShouldBeAttachedToNumber"></param>
				/// <param name="Separator">Разделитель целой и дробной части</param>
				/// <param name="ExceptionValue"></param>
				/// <returns></returns>
				public static double ToDoubleT(this string str, bool CanBeNegative = true, bool StopIfNumberEnded = false, bool CanBeShortcuted = true, bool DotShouldBeAttachedToNumber = true, char Separator = '.', double? ExceptionValue = Double.NaN) //:23.05.2022 Behaviour merged from ToIntT()
				{
					double Double = 0;
					bool? IntPart = true;
					int FractionalPart = -1;
					bool isContainsDouble = false;
					bool negative = false;
					foreach (var ch in str)
					{
						if (IntPart == true)
						{
							if (CanBeNegative&&!isContainsDouble && ch == '-') negative = true; //:multiple ifs per cycle isn't affects perfomance; CanBeNegative не проверялось
							else if (!isContainsDouble && ch != ' ') negative = false;
							else if (ch.IsDigit())
							{
								if (Double > double.MaxValue / 10) return negative ? double.MinValue : double.MaxValue;
								//TODO: make options for ValueOutOfRange ↑ – exceptoin; custom value; min/max value (current); overflow (system default)
								Double = Double * 10 + (ch - '0');
								isContainsDouble = true;
							}
							else if (ch == Separator) IntPart = null; //: Состояние квантовой запутанности. Целая часть и закончилась или нет одновременно. Ну а на самом деле просто чтобы не парсил точку или запятую в предложении как разделитель
							else if(StopIfNumberEnded && isContainsDouble) break;
						}
						else
						{
							if (ch.IsDigit())
							{
								if(FractionalPart==15) break;
								IntPart = false;
								//:на случай строки вроде ".23" (=="0.23")
								if (!isContainsDouble && !CanBeShortcuted)
								{
									Double = Double * 10 + ch.ToIntT();
									isContainsDouble = true;
									IntPart = true;
									continue;
								}
								Double += ch.ToIntT()*Math.Pow(10, FractionalPart--);
							}
							else if (IntPart == null && DotShouldBeAttachedToNumber) IntPart = true;
							else if(StopIfNumberEnded && isContainsDouble) break;
						}
					}

					if (!isContainsDouble)
					{
						if (ExceptionValue == null)
						{
							throw new ArgumentException("Строка не содержит числа");
						}
						else return (int)ExceptionValue;
					}
					return negative? -Double:Double;
				}

				public static double? ToNDoubleT(this string str, char Separator = '.', bool CanBeNegative = true, bool CanBeShortcuted = true, bool DotShouldBeAttachedToNumber = true, bool StopIfNumberEnded = false) //:08.10.2021 new func
				{
					try
					{
						return ToDoubleT(str, CanBeNegative, StopIfNumberEnded, CanBeShortcuted, DotShouldBeAttachedToNumber, Separator);
					}
					catch (Exception)
					{
						return null;
					}
				}

		
				#endregion

				#region Float

				public static float FindFloat(this string str, char DecimalSeparator = '.', char thousandSeparator = ' ')
				{
					int start = 0;
					bool minus = false;
					while (!str[start].IsDigit())
					{
						minus = str[start] == '-';
						start++;
					}

					int end = minus ? start - 1 : start;
					do
					{
						end++;
					} while ((str[end].IsDigit() || str[end] == thousandSeparator || str[end] == DecimalSeparator) && end < str.Length - 1);

					return Convert.ToSingle(str.Substring(start, end - start).Replace(DecimalSeparator, '.').Replace(thousandSeparator.ToString(), ""), new CultureInfo("en-US", true));
				}


				#endregion

				#region Short

				public static short ReadShortUntilLetter(this string str)
				{
					short Short = 0;
					foreach (var ch in str)
					{
						if (ch.IsDigit())
						{
							Short = (short)(Short* 10 + (short)ch);
						}
						else break;
					}

					return Short;
				}

				#endregion

				#region Bool


				/// <summary>
				/// Converts string to bool. If it's "yes" in any language, "1" or "true" returns true. Optimized version
				/// </summary>
				/// <param name="S"></param>
				/// <returns>True, if string is "yes" in any language</returns>
				public static bool ToBool(this string S)
				{
					return S.ToLower() is "1" or "true" or "yes" or "да" or "是" or "si" or "sì" or "da" or "sim" or "ja" or "ya";
				}

				/// <summary>
				/// Converts string to bool. If it's "yes" in any language, "1" or "true" returns true. Unoptimized, but better readable version
				/// </summary>
				/// <param name="S"></param>
				/// <returns>True, if string is "yes" in any language</returns>

				public static bool YesToBool(this string S)
				{
					return new[]
					{
						"1", // true (English)
						"是", // Chinese (Simplified)
						"sí", // Spanish
						"sim", // Portuguese
						"ya", // Indonesian
						"हाँ", // Hindi
						"نعم", // Arabic
						"да", // Russian, Bulgarian
						"はい", // Japanese
						"ja", // German, Dutch, Swedish, Norwegian, Danish
						"oui", // French
						"sì", // Italian
						"evet", // Turkish
						"ใช่", // Thai
						"네", // Korean
						"tak", // Polish
						"так", // Ukrainian
						"igen", // Hungarian
						"ano", // Czech
						"vâng", // Vietnamese
						"ναι", // Greek
						"כן", // Hebrew
						"kyllä", // Finnish
						"áno", // Slovak
						"taip", // Lithuanian
						"da", // Croatian, Slovenian
						"jah", // Estonian
						"jā", // Latvian
						"sí", // Catalan
						"bəli", // Azerbaijani
						"დიახ", // Georgian
						"иә", // Kazakh
						"ஆம்", // Tamil
						"ਹਾਂ", // Punjabi
					}.ContainsAny(S.ToLower());
				}

				//TODO: Add other languages; Refactor to use ToBool(this string S, CultureInfo lang)
				public static string ToRuString(this bool Bool)
				{
					return Bool? "Да" : "Нет";
				}

				public static bool RandBool(Int32 TrueProbability = Int32.MaxValue/2) {
					Random rand = new Random((int)DateTime.Now.Ticks);
					return rand.Next() <= TrueProbability;
				}

				#endregion

				#region Array

				public static double[,] ToDoubleT(this string[,] Strings, char Separator = '.')
				{
					int d0 = Strings.GetLength(0), d1 = Strings.GetLength(1);
					double[,] doubles = new double[d0, d1];
					for (int i = 0; i < d0; i++)
					{
						for (int j = 0; j < d1; j++)
						{
							doubles[i, j] = Strings[i, j].ToDoubleT(Separator:Separator);
						}
					}

					return doubles;
				}

				public static string[,] ToStringT(this double[,] Doubles, string Format)
				{
					int d0 = Doubles.GetLength(0), d1 = Doubles.GetLength(1);
					string[,] strings = new string[d0, d1];
					for (int i = 0; i < d0; i++)
					{
						for (int j = 0; j < d1; j++)
						{
							strings[i, j] = Doubles[i, j].ToString(Format);
						}
					}

					return strings;
				}

				public static string[,] ToStringMatrix(this double[,] Doubles)
				{
					int d0 = Doubles.GetLength(0), d1 = Doubles.GetLength(1);
					string[,] strings = new string[d0, d1];
					for (int i = 0; i < d0; i++)
					{
						for (int j = 0; j < d1; j++)
						{
							strings[i, j] = Doubles[i, j].ToString();
						}
					}

					return strings;
				}

				public static T[,,] ToSingleArray<T>(this T[][,] list)
				{
					int[] dimensions = new[] { list.Length, list[0].GetLength(0), list[0].GetLength(1) };

					T[,,] result = new T[dimensions[0],dimensions[1],dimensions[2]];

					try
					{

						for (int i = 0; i < dimensions[0]; i++)
					{
						for (int j = 0; j < dimensions[1]; j++)
						{
							for (int k = 0; k < dimensions[2]; k++)
							{
								result[i, j, k] = list[i][j, k];
							}
						}
					}

					}
					catch (ArgumentOutOfRangeException e)
					{
						throw new ArgumentException("iternal arrays' dimensions should be the same", e);
					}

					return result;
				}

				#endregion

				#region Color

				//: Should be moved to Titanium.WPF/Titanium.Forms

				/*//!03.09.2023
				/// <summary>
				/// Converts a 32-bit ARGB color value to a <see cref="Color"/> structure.
				/// </summary>
				/// <param name="argbValue"> A 32-bit ARGB color value. </param>
				/// <returns></returns>
				public static Color FromArgb(uint argbValue)
				{
					byte a = (byte)((argbValue >> 24) & 0xFF);
					byte r = (byte)((argbValue >> 16) & 0xFF);
					byte g = (byte)((argbValue >> 8) & 0xFF);
					byte b = (byte)(argbValue & 0xFF);

					return Color.FromArgb(a, r, g, b);
				}

				/// <summary>
				/// Converts a 24-bit RGB color value to a <see cref="Color"/> structure.
				/// </summary>
				/// <param name="rgbValue"> A 24-bit RGB color value. </param>
				/// <returns></returns>
				public static Color FromRgb(uint rgbValue)
				{
					byte r = (byte)((rgbValue >> 16) & 0xFF);
					byte g = (byte)((rgbValue >> 8) & 0xFF);
					byte b = (byte)(rgbValue & 0xFF);

					return Color.FromRgb(r, g, b);
				}*/
				

				#endregion

			#endregion

			#endregion


		#region OtherTypeFuncs

			#region Int

			public static bool IsOdd(this int value) => value % 2 != 0;
			public static bool IsEven(this int value) => value % 2 == 0;

			public static int DivRem(int dividend, int divisor, out int reminder)
			{
				int quotient = dividend / divisor;
				reminder = dividend - divisor * quotient;
				return quotient;
			}

			
			/// <summary>
			/// Maxes input value be in range between MinValue and MaxValue
			/// </summary>
			public static int Fit(this int i, int MinValue = int.MinValue, int MaxValue = Int32.MaxValue) => i < MinValue ? MinValue : i > MaxValue ? MaxValue : i;

			/// <summary>
			/// Maxes input value be in range between 0 and MaxValue
			/// </summary>
			public static int FitPositive(this int i, int MaxValue = int.MaxValue) =>  i < 0 ? 0 : i > MaxValue ? MaxValue : i;
			/// <summary>
			/// Maxes input value be in range between MinValue and 0
			/// </summary>
			public static int FitNegative(this int i, int MinValue = int.MinValue) =>  i > 0 ? 0 : i < MinValue ? MinValue : i;


			#endregion

			#region String

			
			#region SplitN

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <returns></returns>
				public static string[]? SplitN(this string s, string Separator)
				{
					var res = s.Split(Separator);
					return res[0]==s? null : res;
				}
				
				//:thanks GitHub Copilot for all other variations of this func. I hope he copied the summary right
				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, string Separator, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, StringSplitOptions);
					return res[0]==s? null : res;
				}
		
				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				public static string[]? SplitN(this string s, string Separator, int count)
				{
					var res = s.Split(Separator, count);
					return res[0]==s? null : res;
				}
				
				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, string Separator, int count, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, count, StringSplitOptions);
					return res[0]==s? null : res;
				}

				
				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or null. </param>
				public static string[]? SplitN(this string s, string[] Separator)
				{
					var res = s.Split(Separator, StringSplitOptions.None);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, string[] Separator, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, StringSplitOptions);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				public static string[]? SplitN(this string s, string[] Separator, int count)
				{
					var res = s.Split(Separator, count, StringSplitOptions.None);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or null. </param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, string[] Separator, int count, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, count, StringSplitOptions);
					return res[0]==s? null : res;
				}


				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string</param>
				public static string[]? SplitN(this string s, char Separator)
				{
					var res = s.Split(Separator);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string</param>
				/// <param name="count"> The maximum number of substrings to return. </param>
				public static string[]? SplitN(this string s, char Separator, int count)
				{
					var res = s.Split(Separator, count);
					return res[0]==s? null : res;
				}

				/// <summary>
				/// String.Split() but returns null if no any split found
				/// </summary>
				/// <param name="s"></param>
				/// <param name="Separator"> A character that delimits the substrings in this string</param>
				/// <param name="StringSplitOptions"> RemoveEmptyEntries to omit empty array elements from the array returned; or None to include empty array elements in the array returned. </param>
				public static string[]? SplitN(this string s, char Separator, StringSplitOptions StringSplitOptions)
				{
					var res = s.Split(Separator, StringSplitOptions);
					return res[0]==s? null : res;
				}

				#endregion


			public static bool ContainsAny(this string s, IEnumerable<string> sequence) => sequence.Any(s.Contains); 
			public static bool ContainsAny(this string s, params string[] sequence) => sequence.Any(s.Contains); //: 24.10.2022 IEnumerable replaced with params
			public static bool ContainsAll(this string s, IEnumerable<string> sequence) => sequence.All(s.Contains);//: 24.10.2022 IEnumerable replaced with params

			public static bool ContainsAll(this string s, params string[] sequence) => sequence.All(s.Contains);//: 24.10.2022 IEnumerable replaced with params

			public static int SymbolsCount(this string s)
			{
				int i = s.Length;
				foreach (var c in s)
				{
					if (char.IsControl(c)) i--;
				}

				return i;
			}

			/// <summary>
			/// Makes s.Length be equal to <paramref name="FixedLength"/> by adding <paramref name="Filler"/> symbols if it's too short or cutting it if it's too long
			/// </summary>
			/// <param name="d"></param>
			/// <param name="FixedLength"></param>
			/// <param name="Align"></param>
			/// <param name="Filler"></param>
			/// <returns></returns>
			public static string FormatToString(this double d, int FixedLength, Positon Align, char Filler = ' ') //:Ленивый и неоптимизированный способ
			{
				d = Math.Round(d, FixedLength);
				string s = d.ToString();
				if (s.Length < FixedLength) {
					switch (Align) {
					case Positon.left: {
						for (int i = s.Length; i < FixedLength; i++)
						{
							s += Filler;
						}
					} break;
					case Positon.center: {
						int halfString = (FixedLength - s.Length) / 2;
						for (int i = 0; i < halfString; i++)
						{
							s=s.Insert(0, Filler.ToString());
						}
						for (int i = s.Length; i < FixedLength; i++)
						{
							s += Filler;
						}
					}break;
					case Positon.right: {
						for (int i = 0; i < (FixedLength - s.Length); i++)
						{
							s = s.Insert(0, Filler.ToString());
						}
					}break;
					}
				}
				else if (s.Length > FixedLength) {
					int Eindex = s.LastIndexOf('e');

					if (Eindex > 0) { //если в строке есть Е+хх
						string e = s.TrimStart('e');
						s = s.Substring(0, FixedLength - e.Length);
						s += e;
					}
					else {
						s = s.Substring(0, FixedLength);
					}
				}
				return s;
			}

			/// <summary>
			/// Makes s.Length be equal to <paramref name="FixedLength"/> by adding <paramref name="Offset"/> symbols if it's too short or cutting it if it's too long
			/// </summary>
			/// <param name="s"></param>
			/// <param name="FixedLengtharam">
			/// <param name="Align">The position of <see cref="s"/> if it's too short . If it's too long, it will be always aligned Left</param>
			/// <param name="Filter"></param>
			/// <param name="Offset">Positive is right, negative is left. Will be sliced if out of range</param>
			/// <returns></returns>
			public static string FormatString(this string s, int FixedLength, Positon Align, char Filter = ' ', int Offset = 0)
			{
				if (s.Length<FixedLength)
				{
					int NumberOfFillerSymbols = FixedLength - s.Length;
					switch (Align)
					{
						case Positon.left:
						{
							Offset = Offset.FitPositive(NumberOfFillerSymbols);
							NumberOfFillerSymbols -= Offset;
							s = Filter.Multiply(Offset) + s + Filter.Multiply(NumberOfFillerSymbols);
						}
							break;
						case Positon.center:
						{
							int firstHalf = NumberOfFillerSymbols.IsEven()? NumberOfFillerSymbols/2 : Offset>0? NumberOfFillerSymbols/2 : NumberOfFillerSymbols/2 +1,
								secondHalf = NumberOfFillerSymbols-firstHalf;
							if (NumberOfFillerSymbols.IsOdd()) Offset += (Offset > 0) ? -1 : 1;
									
							firstHalf += Offset;
							secondHalf -= Offset;
							s = Filter.Multiply(firstHalf) + s + Filter.Multiply(secondHalf);
						}
							break;
						case Positon.right:
						{
							Offset.FitNegative(NumberOfFillerSymbols);
							NumberOfFillerSymbols += Offset;
							s = Filter.Multiply(NumberOfFillerSymbols) + s + Filter.Multiply(Offset);
						}
							break;
					}
				}
				else if (s.Length>FixedLength)
				{
					int Eindex = s.LastIndexOf('e');

					if (Eindex>0)
					{ //если в строке есть Е+хх
						string e = s.TrimStart('e');
						s=s.Substring(0, FixedLength-e.Length);
						s+=e;
					}
					else
					{
						s=s.Substring(0, FixedLength);
					}
				}
				return s;
			}

			/// <summary>
			/// Inserts <paramref name="Value"/> between <paramref name="Start"> and <paramref name="end"/>
			/// </summary>
			/// <param name="s"></param>
			/// <param name="Value"></param>
			/// <param name="Start"></param>
			/// <param name="end"></param>
			/// <returns></returns>
			/// <exception cref="ArgumentException"></exception>
			public static string Insert(this string s, string Value, int Start, int? End = null)
			{
				int end=  End?? Start;
				if (s.IsNullOrEmpty()) return Value;
				if (Start < 0) Start = s.Length - Start;
				if (end < 0) end = s.Length - Start;
				if (Start > end) Swap(ref Start, ref end);

				if (Start < 0 || Start > s.Length) throw new ArgumentException("Incorrect start " + Start, nameof(Start));
				if (end < 0 || end > s.Length) throw new ArgumentException("Incorrect end " +end, nameof(end));

				return s.Slice(0,Start) + Value + s.Slice(Start);
			}

			public enum Positon: byte { left,center,right}

			public enum SliceReturn: byte { Always, Start, End, Never}

			///  <summary>
			///  Slices the string form <paramref name="Start"/> to <paramref name="End"/> <para></para>
			///  Supported types: <typeparamref name="int"></typeparamref>, <typeparamref name="string"></typeparamref>, <typeparamref name="Regex"></typeparamref>, <typeparamref name="Func&lt;char,bool&gt;"></typeparamref> (isCharValid func)/>.
			///  </summary>
			///  <typeparam name="Ts">Type of the <paramref name="Start"/></typeparam>
			///  <typeparam name="Te">Type of the <paramref name="End"/></typeparam>
			///  <param name="s"></param>
			///  <param name="Start"> Start of the result string
			///		<list type="table">
			///			<item><typeparamref name="default"/>: 0 (don't cut start)</item>
			///			<item><typeparamref name="int"/>: Start index of the result string (inverse direction if negative)</item>
			///			<item><typeparamref name="string"/>: The string inside <paramref name="s"/> that will be the start position of the result</item>
			///			<item><typeparamref name="Regex"/>: The string inside <paramref name="s"/> matches Regex that will be the start position of the result</item>
			///			<item><typeparamref name="Func&amp;lt;char,bool&amp;gt;"/>: Условия, которым должны удовлетворять символы начала строки (по функции на 1 символ)</item>
			///		 </list>
			///  </param>
			///  <param name="End"> End of the result string 
			///		<list type="table">
			///			<item><typeparamref name="default"/>: Max (don't cut end)</item>
			///			<item><typeparamref name="int"/>: End index of the result string (inverse direction if negative). Shrinks to <paramref name="s"/><see langword=".length"/> if it's more than it. Swaps with <paramref name="Start"/> if they're mixed up (only if both are <see cref="int"/>)</item>
			///			<item><typeparamref name="string"/>: The string inside <paramref name="s"/> that will be the end position of the result</item>
			///			<item><typeparamref name="Regex"/>: The string inside <paramref name="s"/> matches Regex that will be the end position of the result</item>
			///			<item><typeparamref name="Func&amp;lt;char,bool&amp;gt;"/>: Условия, которым должны удовлетворять символы конца строки (по функции на 1 символ)</item>
			///		</list>
			///  </param>
			///  <param name="SliceReturnSourceIfNotFound">if <see cref="SliceReturn.Always"/>, return <paramref name="s"/> if <paramref name="Start"/> or <paramref name="End"/> not found. <br />Legacy: AlwaysReturnString = false</param>
			///  <param name="DefaultValueIfNotFound">Set <paramref name="Start"/> or/and <paramref name="End"/> to thier default values (0 and ^1) if not found. </param>
			///  <param name="LastStart">if <see langword="true"/>, the last occurance of the <paramref name="Start"/> will be searched <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
			///  <param name="LastEnd">if <see langword="true"/>, the last occurance of the <paramref name="End"/> will be searched <para/> (doesn't do anything if <paramref name="End"/> is <typeparamref name="int"/>)</param>
			///  <param name="IncludeStart">Include <paramref name="Start"/> symbols <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
			///  <param name="IncludeEnd">Include <paramref name="End"/> symbols <para/> (doesn't do anything if <paramref name="End"/> is <typeparamref name="int"/>)</param>
			///  <param name="throwException">If <see langword="false"/>, returns <see langword="null"/> on expected errors: if source is null or empty; If start or end not found</param>
			///  <exception cref="TypeInitializationException">Type of Start or End is not supported by slice. Ignores <paramref name="throwException"/> value</exception>
			///  <exception cref="ArgumentNullException">If input is null or empty</exception>
			///  <exception cref="ArgumentOutOfRangeException">Start or end not found</exception>
			///  <returns>string between <paramref name="Start"/> and <paramref name="End"/>
			///  null if <paramref name="throwException"/> is <see langword="false"/> and start or end not found
			/// </returns>
			public static string? Slice<Ts, Te>(this string s, Ts? Start, Te? End, SliceReturn SliceReturnSourceIfNotFound = SliceReturn.Never, SliceReturn DefaultValueIfNotFound = SliceReturn.Never, bool LastStart = false, bool LastEnd = true, bool IncludeStart = false, bool IncludeEnd = false, bool throwException = false) 
			{
				if (s.IsNullOrEmpty()) return throwException ? throw new ArgumentNullException(nameof(s)) : null;

				int start = 0; //: slice cursor start position
				int end = int.MaxValue; //: slice cursor end position
				bool basicSlice = Start is int or null && End is int or null; //: detects if input is basic slice (start and end are ints)
				
				switch (Start)
				{
					case null: //: default value (don't slice start)
							start = 0;
						break;

					case int startIndex:
						start = startIndex;
						if (start < 0) start = s.Length + start; //: count from end if negative
						if (start >= s.Length) return "";
						break;

					case string startsWith:
						start = s.IndexOfT(startsWith, IndexOfEnd: !IncludeStart //: if IncludeStart, start will be moved to the end of startsWith
							, InvertDirection: LastStart //: if LastStart, search direction will be inverted (right to left)
							) + (IncludeStart? 0 : 1); //: go out of last letter of s2 if IncludeStart
							
						break;

					case Regex startRegex:
						var match = LastStart? startRegex.Matches(s).Last() :  startRegex.Match(s);
						start = match.Index>=0? 
							(match.Index + (IncludeStart ? 0 : match.Length)) : -1;
						break;

					case Func<char,bool>[] startConditions:
						start = startConditions?.Any()==true? 
							s.IndexOfT(startConditions, IndexOfEnd: !IncludeStart, InvertDirection: LastStart) : -1;
				
						break;

					default:
						throw new TypeInitializationException(typeof(Ts).FullName, new ArgumentException($"Type of {nameof(Start)} is not supported"));
				}

				if (!basicSlice) //: don't do pre-slice if it's a basic slice
				{
					if (start < 0)
						if (DefaultValueIfNotFound is SliceReturn.Never or SliceReturn.End)
							return //: if start not found
								SliceReturnSourceIfNotFound is SliceReturn.Always or SliceReturn.Start ? s : //: return source if SliceReturnSourceIfNotFound is Always or Start
								throwException ? throw new ArgumentOutOfRangeException(nameof(Start), start, "Start not found") : null; //: throw exception if throwException is true ELSE return null
						else
							start = 0; //: if start not found and DefaultValueIfNotFound is Always or Start, set start to 0

					s = s.Slice(start); //: Slice source string to start (to prevent finding end before the start)
				}

				switch (End)
				{
					case null: //: default value (don't slice end)
							end = s.Length;
						break;

					case int endIndex:
						end = endIndex;
						if (end < 0) end = s.Length + end; //: count from end if negative
						if (basicSlice && start > end) Swap(ref start, ref end); //: Swap(start,end) if they're mixed up and it's a basic slice
						if (end > s.Length) end = s.Length; //: normalize end
						break;

					case string endsWith:
						end = (LastEnd ? s.LastIndexOf(endsWith) : s.IndexOf(endsWith));
					
						end = end < 0? -1 : //: if end not found
							IncludeEnd ? end + endsWith.Length : end; //: move end to the end of endsWith if IncludeEnd
						break;

					case Regex endregex:
						var match = LastEnd? endregex.Matches(s).Last() :  endregex.Match(s);
						end = match.Index>=0? 
							(match.Index + (LastEnd ? 0 : match.Length)) : 0;
						break;

					case Func<char,bool>[] endConditions:
						end = endConditions?.Any()!=true? 
							s.IndexOfT(endConditions,IndexOfEnd: IncludeEnd, InvertDirection: LastEnd) : -1;
					
						break;

					default:
						throw new TypeInitializationException(typeof(Te).FullName, new ArgumentException($"Type of {nameof(End)} is not supported"));
				}

				if (basicSlice) return s.Substring(((int)start), ((end) - (start)));

				if (end < 0)
					if(DefaultValueIfNotFound is SliceReturn.Never or SliceReturn.Start)
						return //: if end not found
							SliceReturnSourceIfNotFound is SliceReturn.Always or SliceReturn.End ? s : //: return source if SliceReturnSourceIfNotFound is Always or End
							throwException ? throw new ArgumentOutOfRangeException(nameof(End), end, "End not found") : null; //: throw exception if throwException is true ELSE return null
					else
						end = s.Length; //: if end not found and DefaultValueIfNotFound is Always or End, set end to s.Length

				return s.Slice(0, end);
			}

			/// <summary>
			/// Removes <paramref name="s"/> symbols from 0 to <paramref name="Start"/><para></para>
			/// Supported types: <typeparamref name="int"></typeparamref>, <typeparamref name="string"></typeparamref>, <typeparamref name="Regex"></typeparamref>, <typeparamref name="Func&lt;char,bool&gt;"></typeparamref>;
			/// </summary>
			/// <typeparam name="Ts">Type of the <paramref name="Start"/></typeparam>
			/// <param name="s"></param>
			/// <param name="Start"> Start of the result string <para/>
			///<list type="table"></list>
			/// /// <item><typeparamref name="default"/>: 0 (don't cut start)</item>
			/// <item><typeparamref name="int"/>: Start index of the result string (inverse direction if negative)</item>
			/// <item><typeparamref name="string"/>: The string inside <paramref name="s"/> that will be the start position of the result</item>
			/// <item><typeparamref name="Regex"/>: The string inside <paramref name="s"/> matches Regex that will be the start position of the result</item>
			/// <item><typeparamref name="Func&amp;lt;char,bool&amp;gt;"/>: Условия, которым должны удовлетворять символы начала строки (по функции на 1 символ)</item>
			/// </param>
		
			/// <param name="AlwaysReturnString">return <paramref name="s"/> if <paramref name="Start"/> or <paramref name="End"/> not found (may be half-cutted)</param>
			/// <param name="LastStart">if true, the last occurance of the <paramref name="Start"/> will be searched <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
			/// <param name="IncludeStart">Include <paramref name="Start"/> symbols <para/> (doesn't do anything if <paramref name="Start"/> is <typeparamref name="int"/>)</param>
		
			/// <returns></returns>
			/// <exception cref=""></exception>
			public static string? Slice<Ts>(this string s, Ts? Start, SliceReturn SliceReturnSourceIfNotFound = SliceReturn.Never, SliceReturn DefaultValueIfNotFound = SliceReturn.Never, bool LastStart = false, bool IncludeStart = false, bool IncludeEnd = false, bool throwException = false)
				=> s.Slice(Start, null as string, SliceReturnSourceIfNotFound, DefaultValueIfNotFound, LastStart, true, IncludeStart, IncludeEnd, throwException);


			public static string SliceFromEnd(this string s, string StartsWith = null, string EndsWith = null, bool AlwaysReturnString = false, bool LastStart = false, bool LastEnd = true, bool IncludeStart = false, bool IncludeEnd = false) //:25.08.2022 includeStart, includeEnd
			{
				var end = EndsWith==null? s.Length-1 : LastEnd? s.LastIndexOf(EndsWith) : s.IndexOf(EndsWith);
				if (end < 0) return  AlwaysReturnString? s : null;

				s = s.Slice(0, end);

				var start = StartsWith == null? 0 : LastStart? s.LastIndexOfEnd(StartsWith) : s.IndexOfEnd(StartsWith);
				if (start < 0) return  AlwaysReturnString? s : null;

				return IncludeStart? StartsWith : "" + s.Slice(start) + (IncludeEnd? EndsWith : "");
			}

			private static int IndexOfEnd(this string s, string s2)
			{
				if (s == null)
					if (s2.Length == 0)
						return 0;
				int i = s.IndexOf(s2);
				return i == -1 ? -1 : i + s2.Length;
			}

			/// <summary>
			/// Reports the position of a symbol next to last occurance of a string <paramref name="s2"/> (position after the end of <paramref name="s2"/>) 
			/// </summary>
			/// <param name="s"></param>
			/// <param name="s2"></param>
			/// <returns></returns>  
			private static int LastIndexOfEnd(this string s, string s2)
			{
				if (s == null) 
					if (s2.Length == 0) return 0;
				int i = s.LastIndexOf(s2);
				if (i == -1) return -1;
				else return i + s2.Length;
			}

			public enum DirectionEnum
			{
				Custom,
				Right,
				Left
			}

				/// <summary>
			/// Return an index of (<paramref name="IndexOfEnd"/>? end : start) of <paramref name="s2"/> in <paramref name="s"/>
			/// </summary>
			/// <param name="s"></param>
			/// <param name="s2"></param>
			/// <param name="Start">Start checking from</param>
			/// <param name="InvertDirection">if true, search from end to start</param>
			/// <param name="IndexOfEnd">Return index of end of s2 (last letter) instead of start (first letter)</param>
			/// <returns> -2 if End == Start; -1 if not found; else result </returns>
			/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static int IndexOfT(this string s, string s2, int Start = 0, int End = Int32.MaxValue, bool InvertDirection = false, bool IndexOfEnd = false) //: 26.11.23 Replaced "RightDirection" with "InvertDirection"
			{
				if (End == Int32.MaxValue) End = s.Length-1;
				if (Start < 0) Start = s.Length + Start;
				if (Start < 0) new ArgumentOutOfRangeException(nameof(Start),Start,$"incorrect negative Start ({Start - s.Length}). |Start| should be ≤ s.Length ({s.Length})");
				if (End < 0) End = s.Length + End;
				if (End < 0) throw new ArgumentOutOfRangeException(nameof(End),End,$"incorrect negative End ({End - s.Length}). |End| should be ≤ s.Length ({s.Length})");
				if (s == null) throw new ArgumentNullException(nameof(s));
				if (s2 == null)	throw new ArgumentNullException(nameof(s2));
				if (s2.Length == 0) return InvertDirection? s.Length-1 : 0;


				if (End == Start) return -2;

				if(!InvertDirection && End < Start ||
				   InvertDirection && End > Start)
					Swap(ref Start, ref End);

				int defaultCurMatchPos = InvertDirection? s2.Length-1 : 0;
				int curMatchPos = defaultCurMatchPos;
				int Result = -1;

				if (InvertDirection)
					for (int i = Start; i >= End; i--)
					{
						if (s[i] == s2[curMatchPos])
						{
							curMatchPos--;
							if (curMatchPos != 0) continue; //: if not last letter
							Result = i;
							curMatchPos = defaultCurMatchPos;
							break;
						}
						else
						{
							i += ((s2.Length - 1) - curMatchPos);
							curMatchPos = defaultCurMatchPos;
						}
					}
				else
					for (int i = Start; i <= End; i++)
					{
						if (s[i] == s2[curMatchPos])
						{
							curMatchPos++;
							if (curMatchPos != s2.Length) continue; //: if not last letter
							Result = i;
							curMatchPos = defaultCurMatchPos;
							break;
						}
						else
						{
							i -= curMatchPos;
							curMatchPos = defaultCurMatchPos;
						}
					}

				return Result = Result == -1 || IndexOfEnd ^ InvertDirection?
					Result : (Result - s2.Length) +1;
			}

			/// <summary>
			/// Return an index of (<paramref name="IndexOfEnd"/>? end : start) of <paramref name="s2"/> in <paramref name="s"/>
			/// </summary>
			/// <param name="s">Source string</param>
			/// <param name="Conditions"> Conditions that should be met by the symbols of <paramref name="s"/> to be counted as <paramref name="searched_string"/> </param>
			/// <param name="Start"> Start checking from (default: 0)</param>
			/// <param name="End"> End checking at (default: end of s)</param>
			/// <param name="InvertDirection"> if true, search from end to start</param>
			/// <param name="IndexOfEnd"> Return index of end of s2 (next symbol to s2) instead of start (first letter)</param>
			/// <returns></returns>
			/// <exception cref="ArgumentOutOfRangeException"></exception>
			public static int IndexOfT(this string s, Func<char,bool>[] Conditions, int Start = 0, int End = Int32.MaxValue, bool InvertDirection = false, bool IndexOfEnd = false) //: 26.11.23 Replaced RightDirection with opposite bool "InvertDirection"
			{
				if (End == Int32.MaxValue) End = s.Length-1;
				if (Start < 0) Start = s.Length + Start;
				if (Start < 0) new ArgumentOutOfRangeException(nameof(Start),Start,$"incorrect negative Start ({Start - s.Length}). |Start| should be ≤ s.Length ({s.Length})");
				if (End < 0) End = s.Length + End;
				if (End < 0) throw new ArgumentOutOfRangeException(nameof(End),End,$"incorrect negative End ({End - s.Length}). |End| should be ≤ s.Length ({s.Length})");

				if (End == Start) return -2;

				if(!InvertDirection && End < Start ||
				   InvertDirection && End > Start)
					Swap(ref Start, ref End);
				
				int defaultCurMatchPos = InvertDirection? Conditions.Length-1 : 0;
				int curCondition = defaultCurMatchPos;
				int Result = -1;

				if (InvertDirection)
					for (int i = Start; i >= End; i--)
					{
						if (Conditions[curCondition](s[i]))
						{
							curCondition--;
							if (curCondition != 0) continue;
							Result = i;
							curCondition = defaultCurMatchPos;
							//if(!LastOccuarance) 
							break;
						}
						else
						{
							i += ((Conditions.Length - 1) - curCondition);
							curCondition = defaultCurMatchPos;
						}
					}
				else
					for (int i = Start; i < End; i++)
					{
						if (Conditions[curCondition](s[i]))
						{
							curCondition++;
							if (curCondition != Conditions.Length) continue;
							Result = i;
							curCondition = defaultCurMatchPos;
							//if(!LastOccuarance)
							break;
						}
						else
						{
							i -= curCondition;
							curCondition = defaultCurMatchPos;
						}
					}

				return Result = (Result == -1 || IndexOfEnd ^ InvertDirection)?
					Result : (Result - Conditions.Length) +1;
			}


			public static string Multiply(this string str, int count)
			{
				StringBuilder sb = new StringBuilder(str.Length*count);
				for (int i = 0; i < count; i++)
				{
					sb.Append(str);
				}

				return sb.ToString();
			}

			public static string Multiply(this char ch, int count)
			{
				StringBuilder sb = new StringBuilder(count);
				for (int i = 0; i < count; i++)
				{
					sb.Append(ch);
				}

				return sb.ToString();
			}


			/// <summary>
			/// <paramref name="s"/>[..<paramref name="Start"/>] + <paramref name="NewString"/> + s[<paramref name="End"/>..]
			/// </summary>
			/// <param name="s">original string</param>
			/// <param name="Start">String from 0 to Start will be added before NewString</param>
			/// <param name="End">String from End to s.Length-1 will be added after NewString. If null, End = NewString.Length, or null if there's no NewString</param>
			/// <param name="NewString">String that will be between Start and End. If null, you'll just cut the text between Start and End</param>
			/// <param name="Exception">if false, its returns original string instead of exception</param>
			/// <returns></returns>
			public static string Replace(this string s, int Start, int? End = null, string NewString = null, bool Exception = true)
			{
				if (Start < 0) Start = s.Length - Start;
				if (Start < 0 || Start > s.Length) 
					if (Exception) throw new ArgumentOutOfRangeException($"There's no position {Start} in string s[{s.Length}]"); 
					else return s;
				
				if (End != null)
				{
					if (End < 0) End = s.Length - End;
					if (End < 0 || End > s.Length)
						if (Exception) throw new ArgumentOutOfRangeException($"There's no position {End} in string s[{s.Length}]");
						else if (End > s.Length) End = s.Length - 1;
							else return s;
				}
				else End = Start + NewString?.Length?? 0;

				if (Start == End && string.IsNullOrEmpty(NewString)) return s;
				if (End < s.Length) return s[..Start] + NewString + s[(int)End..];
				else return s[..Start] + NewString;
			}


			public static string ReplaceFromLast(this string s, string OldString, string NewString, bool Exception = true)
			{
				if (OldString is null)
				{
					if (Exception) throw new ArgumentNullException(nameof(OldString));
					else return s;
				}
				var start = s.LastIndexOf(OldString, StringComparison.Ordinal);
				return s.Replace(start, start+OldString.Length, NewString, Exception);
			}

			public static string Replace(this string s, Dictionary<string, string> ReplaceRule) => s.Replace(ReplaceRule.Keys, ReplaceRule.Values);

			public static string Replace(this string s, IEnumerable<string> OldStrings, IEnumerable<string> NewStrings)
			{
				var oldStrings = OldStrings.ToArray();
				var newStrings = NewStrings.ToArray();
				if (oldStrings.Length != newStrings.Length) throw new ArgumentException("OldStrings and NewStrings should have the same length");
				for (int i = 0; i < oldStrings.Length; i++)
				{
					s = s.Replace(oldStrings[i], newStrings[i]);
				}

				return s;
			}

			/// <summary>
			/// Escapes string
			/// </summary>
			/// <param name="s"></param>
			/// <param name="Characters">characters need to be escaped</param>
			/// <returns></returns>
			public static string Escape(this string s, string Characters, string EscapeSymbol = "\\") //:26.08.2022
			{
				string result = "";
				foreach (var c in s)
				{
					if (Characters.Contains(c)) result += EscapeSymbol;
					result += c;
				}

				return result;
			}

			public static IEnumerable<string> Add(this IEnumerable<string> strings, string addiction, bool ToEnd = true)
			{
				var list = strings.ToList();
				for (var index = 0; index < list.Count; index++)
				{
					list[index] = list[index].Add(addiction, ToEnd);
				}

				return list;
			}

			/// <summary>
			/// Добавляет ту часть <paramref name="addiction"/> к <paramref name="s"/>, которая не содержится в конце <paramref name="s"/>. Например, "Test".Add("stop") = "Testop"; "Test".Add("rest") = "Testrest"
			/// </summary>
			/// <param name="s"></param>
			/// <param name="addiction"></param>
			/// <param name="ToEnd">Добавить к концу строки (иначе -- к началу)</param>
			/// <returns></returns>
			public static string Add(this string s, string addiction, bool ToEnd = true) => ToEnd ? AddToEnd(s, addiction) : AddToStart(s, addiction); //TODO: ToEnd лучше заменить с bool на enum Position{Start,End}

			private static string AddToStart(this string s, string addiction)
			{
				if (addiction.Length>1) throw new NotImplementedException("AddToEnd with more than 1 character isn't supported yet");

				if (s[0] != addiction[0]) return addiction + s;
				else return s;
			}

			private static string AddToEnd(this string s, string addiction)
			{
				if (s.IsNullOrEmpty()) return addiction;
				int offset = 0;

				for (; offset < addiction.Length; offset++)
				{
					if (s[^1] != addiction[offset]) continue;

					int aPosition = offset;
					for (int sPosition = s.Length-1; sPosition >=0;)
					{

						if (s[sPosition--] != addiction[aPosition--])
						{
							Debug.Print($"{s[..(sPosition+1)]}|{s[sPosition+1]}|{s[(sPosition+2)..]} ≠ {addiction[..(aPosition+1)]}|{addiction[aPosition+1]}|{addiction[(aPosition+2)..]}");
							break;
						}
						if (aPosition < 0)
						{
							return s + addiction[(offset+1)..];
						}
					}
				}

				return s+addiction;
			}

			/// <summary>
			/// Removes (<paramref name="FromLeft"/>? first : last) occurance of <paramref name="RemovableString"/>
			/// </summary>
			/// <param name="S"></param>
			/// <param name="RemovableString"></param>
			/// <param name="FromLeft"></param>
			/// <param name="ComparisonType"></param>
			/// <returns></returns>
			public static string Remove(this string S, string RemovableString, bool FromLeft = true, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				if (RemovableString is null or "") return S;
				int startPos = FromLeft? S.IndexOf(RemovableString) : S.LastIndexOf(RemovableString);
				return startPos == -1 ? S : S.Remove(startPos, RemovableString.Length);
			}

			public static string RemoveFrom(this string Source, TypesFuncs.Side FromWhere = Side.End, params string[] RemovableStrings)
			{
				foreach (var rem in RemovableStrings) 
					Source = Source.RemoveFrom(FromWhere, rem);
				return Source;
			}

			public static string RemoveFrom(this string Source, Side FromWhere, string RemovableString)
			{
				if (FromWhere!= Side.End && Source.StartsWith(RemovableString)) Source = Source.Slice(RemovableString.Length);
				if (FromWhere!= Side.Start && Source.EndsWith(RemovableString)) Source = Source.Slice(0, -RemovableString.Length);
				return Source;
			}

			public static string RemoveAll(this string S, string RemovableString, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				if (RemovableString is null or "") return S;

				while (true)
				{
					int startPos = S.IndexOf(RemovableString,ComparisonType);
					if (startPos == -1) return S;

					S = S.Remove(startPos, RemovableString.Length);
				} 
			}

			public static string RemoveAll(this string S, string[] RemovableStrings, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				foreach (var s in RemovableStrings)
				{
					S = S.RemoveAll(s, ComparisonType);
				}

				return S;
			}

			public static string RemoveAll(this string S, char[] RemovableChars)
			{
				foreach (var c in RemovableChars)
				{
					S = S.Replace(c.ToString(), "");
				}

				return S;
			}

			public enum Side
			{
				Start,
				End,
				Both
			}

			public static string RemoveAllFrom(this string S, string RemovableChars, Side FromWhere = Side.Both, StringComparison ComparisonType = StringComparison.Ordinal)
			{
				int start = 0, end = 0;

				if (FromWhere != Side.End)
					foreach (var C in S)
					{
						if (RemovableChars.Contains(C)) start++;
						else break;
					}

				if (FromWhere != Side.Start)
					for (int i = S.Length -1; i >=0; i--)
					{
						if (RemovableChars.Contains(S[i])) end++;
						else break;
					}

				return S[start..^end];
			}


			#endregion

			#region Enumerable

			public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] values) => source.Any(values.Contains);
			public static bool ContainsAll<T>(this IEnumerable<T> source, params T[] values) => source.Any(values.Contains);
			public static bool Empty<T>(this IEnumerable<T> s) => !s.Any(); 
			public static Tgt[] ToArray<Tgt, Src>(this IEnumerable<Src> source, Func<Src, Tgt> Convert)
			{
				Tgt[] result = new Tgt[source.Count()];
				int i = 0;
				foreach (var s in source)
				{
					result[i++] = Convert(s);
				}

				return result;
			}

			/// <summary>
			/// Случайным образом перемешивает массив. Obsolete in .NET 8
			/// </summary>
			public static List<T> RandomShuffle<T>(this IEnumerable<T> list)
			{
				Random random = new Random();
				var shuffle = new List<T>(list);
				for (var i = shuffle.Count() - 1; i >= 1; i--)
				{
					int j = random.Next(i + 1);

					(shuffle[j], shuffle[i]) = (shuffle[i], shuffle[j]);
				}
				return shuffle;
			}

			public static List<T> RandomShuffle<T>(this IEnumerable<T> list, Random random)
			{
				var shuffle = new List<T>(list);
				for (var i = shuffle.Count() - 1; i >= 1; i--)
				{
					int j = random.Next(i + 1);

					(shuffle[j], shuffle[i]) = (shuffle[i], shuffle[j]);
				}
				return shuffle;
			}

			public static List<int> RandomList(int start, int count)
			{
				var List = new List<int>(count);
				var Empty = new List<bool>();
				for (int i = 0; i < count; i++)
				{
					List.Add(0);
					Empty.Add(true);
				}
				Random Random = new Random();

				int End = start + count;
				for (int i = start; i < End;)
				{
					int Index = Random.Next(0, count); //C#-повский рандом гавно. Надо заменить чем-то

					if (Empty[Index])
					{
						List[Index] = i;
						Empty[Index] = false;
						i++;
					}
				}

				return List;
			}

			public static T Pop<T>(this List<T> list)
			{
				T r = list[^1];
				list.RemoveAt(list.Count-1);
				return r;
			}
		
			public static void Swap<T>(this List<T> list, int aIndex, int bIndex) 
				=> (list[aIndex], list[bIndex]) = (list[bIndex], list[aIndex]);

			public static void Swap<T>(this T[] list, int aIndex, int bIndex) 
				=> (list[aIndex], list[bIndex]) = (list[bIndex], list[aIndex]);
			public static int IndexOf<T>(this T[] array, T value) 
				=> Array.IndexOf(array, value);

			public static T[][] Split<T>(this T[] array, int arraysCount)
			{
				var arraysSize = DivRem(array.Length,arraysCount,out int lastArraySize);
				if (lastArraySize == 0) lastArraySize = arraysSize;

				T[][] resultArrays = new T[arraysCount][];
				int k = 0;
				for (int i = 0; i < arraysCount-1; i++)
				{
					resultArrays[i] = new T[arraysSize];
					for (int j = 0; j < arraysSize; j++)
					{
						resultArrays[i][j] = array[k++];
					}
				}

				resultArrays[^1] = new T[lastArraySize];
				for (int j = 0; j < lastArraySize; j++)
				{
					resultArrays[^1][j] = array[k++];
				}

				return resultArrays;
			}
			public static int GetMaxIndex(this double[] array)
			{
				int MaxIndex = 0;
				double MaxValue = array[0];

				for (int i = 1; i < array.Length; i++)
				{
					if (array[i] > MaxValue)
					{
						MaxValue = array[i];
						MaxIndex = i;
					}
				}

				return MaxIndex;
			}
			public static T[] Concat<T>(T[] Array1, T[] Array2)
			{
				T[] res = new T[Array1.Length + Array2.Length];
				for (int i = 0; i < Array1.Length; i++)
				{
					res[i] = Array1[i];
				}

				for (int i = 0; i < Array2.Length; i++)
				{
					res[i + Array1.Length] = Array2[i];
				}

				return res;
			}

			public static T[] ReduceDimension<T>(this T[][] Arrays)
			{
				int arraySize = 0;
				for (int i = 0; i < Arrays.Length; i++)
				{
					arraySize += Arrays[i].Length;
				}

				T[] res = new T[arraySize];
				int k = 0;
				foreach (var array in Arrays)
				{
					foreach (var item in array)
					{
						res[k++] = item;
					}
				}

				return res;
			}

			public static string ToStringLine<T>(this IEnumerable<T> Array, string Separator = " ")
			{
				string result = "";
				foreach (var el in Array)
				{
					result += el + Separator;
				}

				return result[..^Separator.Length];
			}

			public static string ToStringLine<T>(this IEnumerable<T> Array, string Separator, string LastSeparator)
			{
				string result = "";
				foreach (var el in Array)
				{
					result += el + Separator;
				}

				return result[..^Separator.Length];
			}

			public static T[] FillAndGet<T>(this T[] source, T value)
			{
				for (int i = 0; i < source.Length; i++)
					source[i] = value;

				return source;
			}

			public static T[] AddRangeAndGet<T>(this T[] array, T[] summand)
			{
				var newArray = new T[array.Length + summand.Length];
				for (int i = 0; i < array.Length; i++)
				{
					newArray[i] = array[i];
				}
				for (int i = 0; i < summand.Length; i++)
				{
					newArray[i + array.Length] = summand[i];
				}

				return newArray;
			}

			public static List<T> AddRangeAndGet<T>(this List<T> list, List<T> summand)
			{
				list.AddRange(summand);
				return list;
			}

			public static bool AllEquals<T>(this IEnumerable<T> array) => array.All(x => Equals(array.First(), x));

			public static string Enumerate(this IEnumerable<string> strings, string Separator = ", ", string LastSeparator = " and ") //: [19.11.2023] new
			{
				string result = "";

				for (int i = 0; i < strings.Count(); i++)
				{
					result+= strings.ElementAt(i) + (i == 0? "" : //: First element doesn't need a separator
						i == strings.Count() - 2? LastSeparator //: Last element with last separator
						: Separator); //: Other elements
				}

				return result;
			}

			#endregion

			#region Size

				// returns the highest dimension
				public static int Max(this Size s) => Math.Max(s.Height, s.Width);
				// returns the lowest dimension 
				public static int Min(this Size s) => Math.Min(s.Height, s.Width);

				/// <summary>
				/// Returns the new Size with same aspect ratio
				/// </summary>
				/// <param name="s">original size</param>
				/// <param name="NewDimensionValue"></param>
				/// <param name="FixedDimension">Dimension you just wrote</param>
				/// <returns></returns>
				public static Size Resize(this Size s, int NewDimensionValue, Dimension FixedDimension = Dimension.Height)
				{
					if ((FixedDimension is Dimension.Height && s.Height == 0) || (FixedDimension is Dimension.Width && s.Width == 0)) throw new ArgumentOutOfRangeException(nameof(s), $"original {FixedDimension} can't be 0");

					return FixedDimension switch
					{
						Dimension.Height => new Size((s.Width * NewDimensionValue) / s.Height, NewDimensionValue),
						Dimension.Width => new Size(NewDimensionValue, (s.Height * NewDimensionValue) / s.Width),
						_ => throw new ArgumentOutOfRangeException(nameof(FixedDimension), FixedDimension, null)
					};
				}

				public enum Dimension
				{
					Height,
					Width
				}
			#endregion

			#region Regex

			public static bool IsMatchT(this Regex r, string s, int start = 0) => s != null && r.IsMatch(s, start);
			/// <summary>
			/// Is any of Regex matches the string
			/// </summary>
			/// <param name="r"></param>
			/// <param name="s"></param>
			/// <param name="start"></param>
			/// <returns></returns>
			public static bool IsMatchAny(this IEnumerable<Regex> r, string s, int start = 0) => s != null && r.Any(x => x.IsMatch(s));
			public static bool IsMatchAll(this IEnumerable<Regex> r, string s, int start = 0) => s != null && r.All(x => x.IsMatch(s));
			
			/// <summary>
			/// Swithes between "includes" and "excludes" modes
			/// </summary>
			/// <param name="r">The regular expression to modify.</param>
			/// <returns>The modified regular expression.</returns>
			public static Regex Reverse(this Regex r)
			{
				var regStr = r.ToString();
				return regStr.StartsWith(@"^((?!") && regStr.EndsWith(@").)*")? new Regex(regStr[5..^4]) : new Regex(@"^((?!" + r + @").)*");
			}

			public static List<Regex> ReverseRegexes(this List<Regex> r)
			{
				for (int i = 0; i < r.Count; i++)
				{
					r[i] = Reverse(r[i]);
				}

				return r;
			}

			/// <summary>
			/// Swaps "equal" mode to "contains" mode (Добавляет $/^ в начало/конец, если их нет; Убирает их, если они есть)
			/// </summary>
			/// <param name="S"></param>
			/// <returns></returns>
			internal static Regex InvertRegex(string S)
			{
				bool anyStart = S.StartsWith("^");
				bool anyEnd = S.EndsWith("$");
				S = anyStart ? S[1..] : $"^{S}";
				S = anyEnd ? S[..^1] : $"{S}$";
				return new Regex(S);
			}

			#endregion

			#region Dictionary

			/*public static Dictionary Reverse<Tkey, TValue>(this Dictionary<Tkey, TValue> source)
			{
				for (int i = 0; i < source.Count; i++)
				{
					source.
				}
			}*/

			#endregion

			#region Color

			static Color Change(this Color c, byte? A = null, byte? R = null, byte? G = null, byte? B = null) => Color.FromArgb(A?? c.A, R??c.R, G??c.G, B??c.B);

			#endregion
			
			#region Generic Type

			public static void Swap<T>(ref T a, ref T b)
			{
				T c = a;
				a = b;
				b = c;
			}

			public static bool IsDefault<T>(this T value) where T : struct
			{
				bool isDefault = value.Equals(default(T));

				return isDefault;
			}													 

			/// <summary>
			/// Returns null if at least one of the <paramref name="s"/> characters does not satisfy the /<param name="predicate"></param>
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="s"></param>
			/// <param name="predicate"></param>
			/// <returns></returns>
			public static IEnumerable<T>? ToNull_IfNotAny<T>(this IEnumerable<T>?  s, Func<T, bool> predicate)  => s?.Any(predicate) is true ? s : null; 
			/// <summary>
			/// Returns default (null or <c>Enumerable.Empty&lt;int&gt;()</c>) if at least one of the <paramref name="s"/> characters does not satisfy the /<param name="predicate"></param>
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="s"></param>
			/// <param name="predicate"></param>
			/// <returns></returns>
			public static IEnumerable<T>? ToDefault_IfNotAny<T>(this IEnumerable<T>?  s, Func<T, bool> predicate)  => s?.Any(predicate) is true ? s : default; 
			public static T ToDefault_IfNot<T>(this T s, Func<T, bool> predicate) where T : struct => predicate(s)? s : default;

			#endregion

			#region Process

			/// <summary>
			/// Kills all processes with the same name or path
			/// </summary>
			/// <param name="Path">Path to the process</param>
			/// <param name="Name">Name of the process</param>
			/// <exception cref="Exception">Error while gathering processes</exception>
			/// <exception cref="InvalidOperationException">Can't kill process. Is inside AggregateException</exception>
			/// <exception cref="AggregateException">Collection of failed attempts to kill processes (one pre process)</exception>
			public static void KillProcesses(string? Path = null, string? Name = null)
			{
				List<Process> processes;
				Name ??= Path?.Slice(new[] { new Func<char, bool>(x => x is '\\' or '/') }, LastStart:true);
				try
				{
					processes = (
						from proc in Name == null ? Process.GetProcesses() : Process.GetProcessesByName(Name)
						where Path == null || proc.MainModule.FileName == Path
						select proc
					).ToList();
				}
				catch (Exception e)
				{
					throw new Exception("Error while gathering processes", e);
				}

				var Exceptions = new List<InvalidOperationException>();

				foreach (var proc in processes)
				{
					try
					{
						proc.Kill();
					}
					catch (Exception e)
					{
						Exceptions.Add(new InvalidOperationException($"Can't kill process {proc.ProcessName}", e));
					}
				}

				if (Exceptions.Count > 0) throw new AggregateException(Exceptions);
			}

			#endregion

		#endregion
	}

	public static class IO
	{
		/// <summary>
		/// Copies all files, directories, subdirectories and it's content to the new folder
		/// </summary>
		/// <param name="SourcePath"></param>
		/// <param name="TargetPath"></param>
		/// <param name="KillRelatedProcesses"></param>
		/// <param name="DisableSyntaxCheck">All paths should end on "\" and contains only "\" (not "/)</param>
		public static void CopyAll(string SourcePath, string TargetPath, bool KillRelatedProcesses = false, List<Regex>? ExceptList = null, bool DisableSyntaxCheck = false)
		{
			ExceptList ??= new List<Regex>();
			var ErrorList = new List<Exception>();
			if (!DisableSyntaxCheck)
			{
				SourcePath = SourcePath.Replace("/", "\\").Add("\\"); //: Replaces all "/" with "\" and adds "\" at the end if it's not there
				TargetPath = TargetPath.IsNullOrEmpty()? "" : TargetPath.Replace("/", "\\").Add("\\");
			}

			//Now Create all of the directories
			foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
			{
				if (ExceptList.IsMatchAny(dirPath)) continue;
				try
				{
					Directory.CreateDirectory(dirPath.Replace(SourcePath, TargetPath));
				}
				catch (Exception e)
				{
					ErrorList.Add(e);
				}
				
			}

			//Copy all the files & Replaces any files with the same name
			foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
			{
				if (ExceptList.IsMatchAny(newPath)) continue;
				try
				{
					var destination = newPath.Replace(SourcePath, TargetPath);
					if (KillRelatedProcesses && destination.EndsWith(".exe"))
						TypesFuncs.KillProcesses(Path: destination);
					File.Copy(newPath, destination , true);
				}
				catch (Exception e)
				{
					ErrorList.Add(e);
				}
				
			}

			if (ErrorList.Count > 0) throw new AggregateException("Unable to copy files" ,ErrorList);
		}

		public static void CopyAllTo(this DirectoryInfo di, string TargetPath, bool KillRelatedProcesses = false, List<Regex> ExceptList = null, bool DisableSyntaxCheck = false)
		{
			CopyAll(di.FullName, TargetPath, KillRelatedProcesses, ExceptList, DisableSyntaxCheck);
		}
		
		/// <summary>
		/// Removes all files and directories in the folder
		/// </summary>
		/// <param name="FolderPath"></param>
		/// <param name="RemoveSelf"></param>
		/// <param name="ExceptList"></param>
		/// <exception cref="AggregateException"></exception>
		public static void RemoveAll(string FolderPath, bool RemoveSelf = true, List<Regex>? ExceptList = null)
		{
			var ErrorList = new List<Exception>();

			foreach (var dir in  Directory.GetDirectories(FolderPath))
			{
				try
				{
					if(ExceptList?.IsMatchAny(dir)?? false) continue;
					RemoveAll(dir, ExceptList:ExceptList);
				}
				catch (Exception e)
				{
					ErrorList.Add(e);
				}
			}

			foreach (var file in Directory.GetFiles(FolderPath))
			{
				if(ExceptList?.IsMatchAny(file)?? false) continue;
				File.Delete(file);
			}

			if (RemoveSelf) 
				try { Directory.Delete(FolderPath, false);}
				catch (Exception e) {ErrorList.Add(e);}

			if (ErrorList.Count > 0) throw new AggregateException("Unable to copy files" ,ErrorList);
		}

		/// <summary>
		///  Renames all files in the Directory (not recursive)
		/// </summary>
		/// <param name="FolderPath"></param>
		/// <param name="Rename"> Function where input is file's name (not path) and the output is new file's name</param>
		/// <param name="ExceptList">Regular expression of filePATHs that won't be renamed</param>
		public static void RenameAll(string FolderPath, Func<string, string> Rename, List<Regex>? ExceptList = null)
		{
			ExceptList ??= new List<Regex>();
			foreach (var file in Directory.GetFiles(FolderPath))
			{
				if(ExceptList.IsMatchAny(file)) continue;

				var fileInfo = new FileInfo(file);
				var path = fileInfo.Directory.FullName;
				var fileName = fileInfo.Name;
				File.Move(file,Path.Combine(path, Rename(fileName)));	
			}
		}

		public static void MoveAllTo(string SourcePath, string TargetPath, bool DeleteSourceDir = true,  bool KillRelatedProcesses = false, List<Regex> ExceptList = null, bool DisableSyntaxCheck = false) 
			=> new DirectoryInfo(SourcePath).MoveAllTo(TargetPath, DeleteSourceDir, KillRelatedProcesses, ExceptList, DisableSyntaxCheck);

		public static void MoveAllTo(this DirectoryInfo di, string TargetPath, bool DeleteSourceDir = true, bool KillRelatedProcesses = false, List<Regex> ExceptList = null, bool DisableSyntaxCheck = false)
		{
			var sourcePath = di.FullName;
			CopyAll(sourcePath,TargetPath,KillRelatedProcesses, ExceptList, DisableSyntaxCheck);
			if (DeleteSourceDir) Directory.Delete(sourcePath, true);
			else
			{
				foreach (var dir in di.GetDirectories())
				{
					if (ExceptList.IsMatchAny(dir.FullName))
					dir.Delete(true);
				}
			}
		}

		/// <summary>
		/// Returns all files in the folder and it's subfolders
		/// </summary>
		/// <param name="FolderPath"></param>
		/// <param name="Recursive"> If true, it will also return all files in the subfolders</param>
		/// <param name="Whitelist">Regular expression of filePATHs that will be included (all by default)</param>
		/// <returns></returns>
		public static List<string> GetAllFiles(string FolderPath, bool Recursive = true, Regex? Whitelist = null)
		{
			var files = new List<string>();
			foreach (var file in Directory.GetFiles(FolderPath))
			{
				if(Whitelist?.IsMatch(file)?? true)
					files.Add(file);
			}

			if (Recursive)
			{
				foreach (var dir in Directory.GetDirectories(FolderPath))
				{
					if(Whitelist?.IsMatch(dir)?? true)
						files.AddRange(GetAllFiles(dir, true, Whitelist));
				}
			}

			return files;
		}
	}

	public static class ClassicFuncs
	{
		public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

		public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);
		/// <summary>
		/// Retrieves reference to specified string
		/// </summary>
		/// <param name="s"></param>
		/// <returns>A reference to s if it is in the common language runtime intern pool; otherwise null</returns>
		public static string IsInterned(this string s) => string.IsInterned(s);

	}

	public static class Classes
	{
		public class FileSize
		{
			private long longSize;

			public double Size;
			public SizeUnit Unit;

			public enum SizeUnit
			{
				Bit,
				Byte,
				KiloByte,
				MegaByte,
				GigaByte,
				TeraByte,
				PetaByte,
				ExaByte,
				ZettaByte,
				YottaByte,
				KiloBit,
				MegaBit,
				GigaBit,
				TeraBit,
				PetaBit,
				ExaBit,
				ZettaBit,
				YottaBit
			}

			private static string[][] UnitNames = new[]
			{
				new[] { "bit" },
				new[] { "byte", "b" },
				new[] { "kilobyte", "kb" },
				new[] { "megabyte", "mb" },
				new[] { "gigabyte", "gb" },
				new[] { "terabyte", "tb" },
				new[] { "petabyte", "pb" },
				new[] { "exabyte", "eb" },
				new[] { "zettabyte", "zb" },
				new[] { "yottabyte", "yb" },
				new[] { "kilobit", "kbit" },
				new[] { "megabit", "mbit" },
				new[] { "gigabit", "gbit" },
				new[] { "terabit", "tbit" },
				new[] { "petabit", "pbit" },
				new[] { "exabit", "ebit" },
				new[] { "zettabit", "zbit" },
				new[] { "yottabit", "ybit" }
			};

			private void CalculateLongSize()
			{
				int intUnit = (int)Unit;
				longSize = (long)(
					(SizeUnit)(Size * (double)Unit) ==
					SizeUnit.Bit ? 1 :
					Unit == SizeUnit.Byte ? 8 :
					Math.Pow(2, (intUnit - 2) % 8 + 1) * (Unit > SizeUnit.YottaByte ? 1 : 8)
				);
			}

			private static string getUnitName(SizeUnit SU) => UnitNames[(int)SU][0];

			private static SizeUnit getSizeUnit(string UnitName, bool strictlyEqual = false)
			{
				var lowerUnitName = UnitName.ToLower();

				for (int i = UnitNames.Length-1; i >=0; i--)
				{
					//Debug.Write(UnitNames[i].ToStringT(", "));
					if (strictlyEqual? UnitNames[i].Contains(lowerUnitName) : UnitNames[i].Any(lowerUnitName.Contains))
						return (SizeUnit)i;
				}

				throw new ArgumentOutOfRangeException(nameof(UnitName), "can't find unit named " + UnitName);
			}

			public FileSize(long BitsCount)
			{
				longSize = BitsCount;
				Size = BitsCount;
				Unit = 0;
				if (BitsCount < 8) return;
				Size /= 8;
				Unit++;
				for (; Size > 1024 && Unit<SizeUnit.YottaByte; Unit++)
				{
					Size /= 1024;
				}
			}

			private FileSize(double Size, SizeUnit Unit)
			{
				this.Size = Size;
				this.Unit = Unit;
				CalculateLongSize();
			}

			public static FileSize? Get(string Text)
			{
				if(Text.IsNullOrEmpty()) return null;

				int unitIndex = 0;
				while (Text[unitIndex].IsDoubleT())
				{
					unitIndex++;
					if (unitIndex == Text.Length) return null;
				}

				var size = Text[..unitIndex].ToDoubleT();
				var unit = getSizeUnit(Text[unitIndex..]);

				return new FileSize(size, unit);
			}
		}
	}
}