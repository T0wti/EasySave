using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace EasySave.GUI.ViewModels
{
    public partial class MessageBoxViewModel : ObservableObject
    {
        public string Title { get; }
        public string Message { get; }
        public string Ok { get; }

        public bool IsError { get; } //To display text in red if error message

        public ICommand OkCommand { get; }

        public MessageBoxViewModel(string title, string message, string ok, bool isError=false)
        {
            Title = title;
            Message = message;
            Ok = ok;
            IsError = isError;
        }
        public void OkButton_Click(Window window)
        {
            window.Close();
        }
    }
}