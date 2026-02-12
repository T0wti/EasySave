using CommunityToolkit.Mvvm.ComponentModel;

namespace EasySave.Application.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private ViewModelBase _currentView;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public MainWindowViewModel()
    {
        CurrentView = new BaseMenuViewModel(this);
    }

}