using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
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