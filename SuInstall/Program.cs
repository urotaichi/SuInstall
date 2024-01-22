using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SuInstall
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				Console.WriteLine("***********************************");
				Console.WriteLine("* Side AutoUpdater");
				Console.WriteLine("* v 1.21");
				Console.WriteLine("***********************************");
				Console.Write("Install Prechecking.Please Wait...");
				string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				UpdateData updateData = UpdateData.ParseXML(Path.Combine(directoryName, UpdateDataFile));
				if (updateData == null)
				{
					Console.WriteLine("Failed!");
					Console.WriteLine("Install Definition File Notfound.");
					Console.WriteLine("Press Any Key To Close...");
					Console.ReadKey(false);
				}
				else
				{
					string update = updateData.Update;
					if (update == "" || update == null || !Directory.Exists(update))
					{
						Console.WriteLine("Failed!");
						Console.WriteLine("Install Destination Notfound.");
						Console.WriteLine("Press Any Key To Close...");
						Console.ReadKey(false);
					}
					else
					{
						string text = updateData.Installer;
						if (text == "" || text == null || !Directory.Exists(text))
						{
							if (File.Exists(text))
							{
								Console.WriteLine("Failed!");
								Console.WriteLine("***********************************");
								Console.WriteLine("この更新プログラムはSide 0.8以降からに対応しています。");
								Console.WriteLine("0.8以前から0.8以降への更新は手動で行う必要があります。");
								Console.Write("公式サイト(https://side.urotaichi.com/)を開きますか?(y/n):");
								ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
								while (consoleKeyInfo.Key != ConsoleKey.Y && consoleKeyInfo.Key != ConsoleKey.N)
								{
									Console.WriteLine();
									Console.Write("YキーかNキーを押してください:");
									consoleKeyInfo = Console.ReadKey(true);
								}
								if (consoleKeyInfo.Key == ConsoleKey.Y)
								{
									Process.Start("https://side.urotaichi.com/");
								}
								else
								{
									Console.WriteLine("何かキーを押すと終了します...");
									Console.ReadKey(false);
								}
								File.Delete(text);
							}
							else
							{
								Console.WriteLine("Failed!");
								Console.WriteLine("Install Directory Notfound.");
								Console.WriteLine("Press Any Key To Close...");
								Console.ReadKey(false);
							}
						}
						else
						{
							Console.WriteLine("OK.");
							Console.Write("Checking File..." + text);
							try
							{
								while (Directory.GetFiles(text, "*.exe", SearchOption.TopDirectoryOnly).Length == 0)
								{
									text = Directory.GetDirectories(text, "*", SearchOption.TopDirectoryOnly)[0];
								}
							}
							catch
							{
								Console.WriteLine("Failed!");
								Console.WriteLine("Install File Detection Failured.");
								Console.WriteLine("Press Any Key To Close...");
								Console.ReadKey(false);
								return;
							}
							Console.WriteLine("OK.");
							Console.WriteLine("***********************************");
							Console.WriteLine("New Files Installing...");
                            MoveDirectory(text, update);
							Directory.Delete(text, true);
							Console.WriteLine("***********************************");
							Console.WriteLine("Install Completed.");
							Console.WriteLine("New Side Executing...");
							if (updateData.Name != null && updateData.Name != "")
							{
								Process.Start(Path.Combine(updateData.Update, updateData.Name));
								Console.WriteLine("Update Successed.This Program Will Be Killed Myself.");
							}
							else
							{
								Console.WriteLine("Sorry,Auto Boot Failure.Please Restart With Your Hand.");
								Console.WriteLine("Press Any Key To Close...");
								Console.ReadKey(false);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("-----------------------------------");
				Console.WriteLine("Exception Thrown!");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				string tempFileName = Path.GetTempFileName();
				try
				{
					using (StreamWriter streamWriter = new(tempFileName))
					{
						streamWriter.WriteLine("*Side AutoUpdater Error Reporting:");
						streamWriter.WriteLine(ex.Message);
						streamWriter.WriteLine("*Call Object");
						streamWriter.WriteLine(ex.Source);
						streamWriter.WriteLine("*Stacktrace:");
						streamWriter.WriteLine(ex.StackTrace);
					}
					Console.WriteLine("-----------------------------------");
					Console.WriteLine("Error reporting file was created.");
					Process.Start("notepad.exe", tempFileName);
				}
				catch
				{
					Console.WriteLine("Error Report File Can't Create!");
				}
				Console.WriteLine("Press Any Key To Close...");
				Console.ReadKey(false);
			}
		}

		public static void MoveDirectory(string sourceDirName, string destDirName)
		{
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
				File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
			}
			if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
			{
				destDirName += Path.DirectorySeparatorChar;
			}
			string[] files = Directory.GetFiles(sourceDirName);
			foreach (string text in files)
			{
				string text2 = destDirName + Path.GetFileName(text);
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
				Console.Write("Copy:" + text2 + "...");
				File.Move(text, text2);
				Console.WriteLine("Success.");
			}
			string[] directories = Directory.GetDirectories(sourceDirName);
			foreach (string text3 in directories)
			{
                MoveDirectory(text3, destDirName + Path.GetFileName(text3));
			}
		}

		private static string UpdateDataFile = "su.xml";
	}
}
