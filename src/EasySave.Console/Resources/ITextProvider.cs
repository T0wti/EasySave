namespace EasySave.Console.Resources;

public interface ITextProvider
{
    string Header { get; }
    string Footer { get; }
    string MainMenuTitle { get; }
    string BackupOption { get; }
    string LanguageOption { get; }
    string ExitOption { get; }
    string AskEntryFromUser { get; }
}