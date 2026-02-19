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
        public string Yes { get; }
        public string No { get; }
        public string Ok { get; }
        public bool IsError { get; } //To display text in red if error message
        public bool IsConfirmation { get; } //To display 'yes' or 'no' instead of 'ok'

        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }
        public ICommand OkCommand { get; }

        public MessageBoxViewModel(string message, string yes, string no, string ok, bool isError, bool isConfirmation)
        {
            Message = message;
            Yes = yes;
            No = no;
            Ok = ok;
            IsError = isError;
            IsConfirmation = isConfirmation;
        }
        public void YesButton_Click(Window window)
        {
            window.Close(true);
        }

        public void NoButton_Click(Window window)
        {
            OkButton_Click(window);
        }

        public void OkButton_Click(Window window)
        {
            window.Close();
        }
    }
}