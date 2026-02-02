using EasySave.Console.Resources;

namespace EasySave.Console.ConsoleUI;

public class ChangeLanguageMenu : BaseMenu
{
    private readonly ITextProvider _texts;

    public ChangeLanguageMenu(ITextProvider texts) : base(texts)
    {
        _texts = texts;
    }

    public void Display()
    {
        
    }
    
    private void DisplayChangeLanguageBaseMenu()
    {
        
    }
}