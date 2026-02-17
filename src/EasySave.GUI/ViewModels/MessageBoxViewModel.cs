using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{
    public partial class MessageBoxViewModel : ObservableObject
    {
        public string Title { get; }
        public string Message { get; }
        public string Ok { get; }

        public ICommand OkCommand { get; }

        public MessageBoxViewModel(string title, string message)
        {
            //Title = Text.Title;
            Title = "Information";
            //Message = Text.Message;
            Message = "Settings saved";
            //Ok = Texts.Ok;
            Ok = "Ok";
        }
    }
}